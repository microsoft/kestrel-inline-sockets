// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Logging;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Memory;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network;
using Microsoft.Extensions.Logging;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Pipelines
{
    public class SocketPipeWriter : PipeWriter, IDisposable
    {
        private readonly IConnectionLogger _logger;
        private readonly INetworkSocket _socket;
        private readonly RollingMemory _buffer;

#if NETSTANDARD2_0
        private readonly IConnection _connection;
        private readonly CancellationTokenSource _readerCompleted = new CancellationTokenSource();
        private Exception _readerCompletedException;
#endif

        private bool _isCanceled;
        private bool _isCompleted;

        public SocketPipeWriter(
            IConnectionLogger logger,
            InlineSocketsOptions options,
            IConnection connection,
            INetworkSocket socket)
        {
            _logger = logger;
#if NETSTANDARD2_0
            _connection = connection;
#endif
            _socket = socket;
            _buffer = new RollingMemory(options.MemoryPool);
        }

        public bool IsCanceled => _isCanceled;

        public bool IsCompleted => _isCanceled || _isCompleted;

        public void Dispose()
        {
            _buffer.Dispose();
#if NETSTANDARD2_0
            _readerCompleted.Dispose();
#endif
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
            try
            {
                while (!_buffer.IsEmpty)
                {
                    var memory = _buffer.GetOccupiedMemory();

                    _logger.LogTrace("TODO: SendStarting {bytes}", memory.Length);
                    var bytes = _socket.Send(memory);
                    _logger.LogTrace("TODO: SendComplete {bytes}", bytes);

                    _buffer.ConsumeOccupiedMemory(bytes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex, "TODO: SendError");

                // Return FlushResult.IsCompleted true from now on
                // because we assume any write exceptions are not temporary
                _isCompleted = true;
#if NETSTANDARD2_0
                FireReaderCompleted(ex);
#endif
            }

            return new ValueTask<FlushResult>(new FlushResult(
                isCanceled: IsCanceled,
                isCompleted: IsCompleted));
        }

        public override void CancelPendingFlush()
        {
            _isCanceled = true;
        }

        public override void Complete(Exception exception = null)
        {
            _logger.LogTrace(exception, "TODO: PipeWriterComplete");

            _isCompleted = true;
#if NETSTANDARD2_0
            _connection.OnPipeWriterComplete(exception);
#endif
        }

#if NETSTANDARD2_0
        public override void OnReaderCompleted(Action<Exception, object> callback, object state)
        {
            _readerCompleted.Token.Register(() => callback(_readerCompletedException, state));
        }

        private void FireReaderCompleted(Exception exception)
        {
            _readerCompletedException = exception;
            _readerCompleted.Cancel();
        }
#endif
    }
}
