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
    public class SocketPipeReader : PipeReader, IDisposable
    {
        private readonly IConnectionLogger _logger;
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

                    _logger.LogTrace("TODO: ReadStarting");
                    var bytes = await _socket.ReceiveAsync(memory, cancellationToken);
                    _logger.LogTrace("TODO: ReadComplete {bytes}", bytes);

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
                        _connection.FireConnectionClosed();
                    }
                }
                catch (TaskCanceledException)
                {
                    _logger.LogTrace("TODO: ReadCanceled");
                    _isCanceled = true;
                    _connection.FireConnectionClosed();
                }
                catch (Exception ex)
                {
                    _logger.LogTrace(ex, "TODO: ReadFailed");

                    // Return ReadResult.IsCompleted == true from now on
                    // because we assume any read exceptions are not temporary
                    _isCompleted = true;
                    _connection.FireConnectionClosed();
#if NETSTANDARD2_0
                    FireWriterCompleted(ex);
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
            _logger.LogTrace("TODO: CancelPendingRead");

            _socket.CancelPendingRead();
        }

        public override void Complete(Exception exception)
        {
            _logger.LogTrace(exception, "TODO: PipeReaderComplete");

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
