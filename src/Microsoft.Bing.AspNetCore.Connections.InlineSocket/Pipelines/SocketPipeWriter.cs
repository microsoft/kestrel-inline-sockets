// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Logging;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Memory;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Pipelines
{
    public class SocketPipeWriter : PipeWriter, IDisposable, IConnectionOutputControlFeature
    {
        private readonly IConnectionLogger _logger;
        private readonly InlineSocketsOptions _options;
        private readonly INetworkSocket _socket;
        private readonly RollingMemory _buffer;
        private readonly IConnection _connection;

        private bool _isCanceled;
        private bool _isCompleted;
        private int _suspendCount;

        public SocketPipeWriter(
            IConnectionLogger logger,
            InlineSocketsOptions options,
            IConnection connection,
            INetworkSocket socket)
        {
            _logger = logger;
            _options = options;
            _connection = connection;
            _socket = socket;
            _buffer = new RollingMemory(options.MemoryPool);
        }

        public bool IsCanceled => _isCanceled;

        public bool IsCompleted => _isCanceled || _isCompleted;

        bool IConnectionOutputControlFeature.IsSuspended
        {
            get
            {
                // interlocked read (no side-effects. if zero stay zero.)
                var suspendCount = Interlocked.CompareExchange(ref _suspendCount, 0, 0);

                // return true if non-zero outstanding calls to Suspend/Resume
                return suspendCount != 0;
            }
        }

        public void Dispose()
        {
            _buffer.Dispose();
        }

        public override Memory<byte> GetMemory(int sizeHint)
        {
            return _buffer.GetTrailingMemory(sizeHint);
        }

        public override Span<byte> GetSpan(int sizeHint)
        {
            return _buffer.GetTrailingMemory(sizeHint).Span;
        }

        public override void Advance(int bytes)
        {
            _buffer.ConsumeTrailingMemory(bytes);
        }

        public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken)
        {
            // get number of calls to Suspend that are not matched by calls to Resume
            var suspendCount = Interlocked.CompareExchange(ref _suspendCount, 0, 0);
            if (suspendCount == 0)
            {
                // send data only when all calls to Suspend have been matched by calls to Resume
                FlushBufferToSocket();
            }

            return new ValueTask<FlushResult>(new FlushResult(
                isCanceled: IsCanceled,
                isCompleted: IsCompleted));
        }

        public override void CancelPendingFlush()
        {
            _logger.PendingWriteCanceling(_connection.ConnectionId);

            _isCanceled = true;
        }

        public override void Complete(Exception exception = null)
        {
            _logger.PipeWriterComplete(_connection.ConnectionId, exception);

            _isCompleted = true;
        }

        void IConnectionOutputControlFeature.Suspend()
        {
            // increase number suspensions
            var suspendCount = Interlocked.Increment(ref _suspendCount);

            _logger.PipeWriterSuspended(_connection.ConnectionId, suspendCount);
        }

        void IConnectionOutputControlFeature.Resume()
        {
            // decrease number of suspensions
            var suspendCount = Interlocked.Decrement(ref _suspendCount);

            _logger.PipeWriterResumed(_connection.ConnectionId, suspendCount);

            if (suspendCount < 0)
            {
                // must not be called more times than suspend
                throw new InvalidOperationException("Unexpected call to Resume. Must be called exactly once per Suspend.");
            }

            // if the number calls to Resume is now equal the number of calls to Suspend
            if (suspendCount == 0)
            {
                // send any data that was buffered while output was suspended
                FlushBufferToSocket();
            }
        }

        private void FlushBufferToSocket()
        {
            try
            {
                while (!_buffer.IsEmpty)
                {
                    var memory = _buffer.GetOccupiedMemory();

                    if (_options.HighVolumeLogging)
                    {
                        _logger.WriteStarting(_connection.ConnectionId, (int)memory.Length);
                    }

                    var bytes = _socket.Send(memory);

                    if (_options.HighVolumeLogging)
                    {
                        _logger.WriteSucceeded(_connection.ConnectionId, bytes);
                    }

                    _buffer.ConsumeOccupiedMemory(bytes);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.WriteCanceled(_connection.ConnectionId);
                _isCanceled = true;
            }
            catch (Exception ex)
            {
                _logger.WriteFailed(_connection.ConnectionId, ex);

                // Return FlushResult.IsCompleted true from now on
                // because we assume any write exceptions are not temporary
                _isCompleted = true;
            }
        }
    }
}
