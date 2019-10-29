// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Logging;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Memory;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Pipelines
{
    public class SocketPipeReader : PipeReader, IDisposable
    {
        private readonly IConnectionLogger _logger;
        private readonly InlineSocketsOptions _options;
        private readonly IConnection _connection;
        private readonly INetworkSocket _socket;
        private readonly RollingMemory _buffer;

#if NETSTANDARD2_0
        private readonly CancellationTokenSource _writerCompleted = new CancellationTokenSource();
        private Exception _writerCompletedException;
#endif

        private bool _bufferHasUnexaminedData;
        private bool _isCanceled;
        private bool _isCompleted;

        public SocketPipeReader(
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

        public void Dispose()
        {
            _buffer.Dispose();
#if NETSTANDARD2_0
            _writerCompleted.Dispose();
#endif
        }

        public override bool TryRead(out ReadResult result)
        {
            throw new NotImplementedException();
        }

        public override async ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken)
        {
            if (_bufferHasUnexaminedData == false &&
                IsCompleted == false)
            {
                try
                {
                    // memory based on default page size for MemoryPool being used
                    var memory = _buffer.GetTrailingMemory();

                    if (_options.HighVolumeLogging)
                    {
                        _logger.ReadStarting(_connection.ConnectionId, memory.Length);
                    }

                    var bytes = await _socket.ReceiveAsync(memory, cancellationToken);

                    if (_options.HighVolumeLogging)
                    {
                        _logger.ReadSucceeded(_connection.ConnectionId, bytes);
                    }

                    if (bytes != 0)
                    {
                        // advance rolling memory based on number of bytes received
                        _buffer.TrailingMemoryFilled(bytes);

                        // the new bytes have not been examined yet. this flag
                        // is true until the parser calls AdvanceTo with
                        // an examined SequencePosition corresponding to the tail
                        _bufferHasUnexaminedData = true;
                    }
                    else
                    {
                        // reading 0 bytes means the remote client has
                        // sent FIN and no more bytes will be received
                        _isCompleted = true;
                    }
                }
                catch (TaskCanceledException)
                {
                    _logger.ReadCanceled(_connection.ConnectionId);
                    _isCanceled = true;
                }
                catch (Exception error)
                {
                    _logger.ReadFailed(_connection.ConnectionId, error);

                    // Return ReadResult.IsCompleted == true from now on
                    // because we assume any read exceptions are not temporary
                    _isCompleted = true;

                    // inform the protocol layer the remote client is abnormally unreadable
                    _connection.FireConnectionClosed();
#if NETSTANDARD2_0
                    FireWriterCompleted(error);
#endif
                }
            }

            return new ReadResult(
                _buffer.GetOccupiedMemory(),
                isCanceled: IsCanceled,
                isCompleted: IsCompleted);
        }

        public override void AdvanceTo(SequencePosition consumed)
        {
            AdvanceTo(consumed, consumed);
        }

        public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
        {
            _buffer.ConsumeOccupiedMemory(consumed);

            _bufferHasUnexaminedData = _buffer.HasUnexaminedData(examined);
        }

        public override void CancelPendingRead()
        {
            _logger.PendingReadCanceling(_connection.ConnectionId);

            _socket.CancelPendingRead();
        }

        public override void Complete(Exception exception)
        {
            _logger.PipeReaderComplete(_connection.ConnectionId, exception);

            _isCompleted = true;
#if NETSTANDARD2_0
            _connection.OnPipeReaderComplete(exception);
#endif
        }

#if NETSTANDARD2_0
        public override void OnWriterCompleted(Action<Exception, object> callback, object state)
        {
            _writerCompleted.Token.Register(() => callback(_writerCompletedException, state));
        }

        public void FireWriterCompleted(Exception exception)
        {
            _writerCompletedException = exception;
            _writerCompleted.Cancel();
        }
#endif
    }
}
