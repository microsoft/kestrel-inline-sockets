// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network
{
    public class NetworkSocket : INetworkSocket
    {
        private readonly Socket _socket;

        private readonly object _receiveSync = new object();
        private readonly SocketAsyncEventArgs _receiveEventArgs;
        private TaskCompletionSource<int> _receiveAsyncTaskSource;
        private TaskCompletionSource<int> _receiveAsyncTaskSourceCache;
        private int _disposed;
        private bool _pendingReadCanceled;

        public NetworkSocket(Socket socket)
        {
            _socket = socket;
            _receiveEventArgs = new SocketAsyncEventArgs();
            _receiveEventArgs.Completed += ReceiveAsyncCompleted;
        }

        public virtual EndPoint LocalEndPoint => _socket.LocalEndPoint;

        public virtual EndPoint RemoteEndPoint => _socket.RemoteEndPoint;

        public virtual void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                if (_socket.Connected)
                {
                    _socket.Disconnect(reuseSocket: false);
                }

                _socket.Dispose();
                _receiveEventArgs.Dispose();
            }
        }

        public virtual async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                if (_socket.Connected)
                {
                    using var disconnectEventArgs = new SocketAsyncEventArgs();
                    var disconnectCompletionSource = new TaskCompletionSource<int>();
                    disconnectEventArgs.Completed += (sender, e) => disconnectCompletionSource.SetResult(0);
                    if (_socket.DisconnectAsync(disconnectEventArgs))
                    {
                        await disconnectCompletionSource.Task;
                    }
                }

                _socket.Dispose();
                _receiveEventArgs.Dispose();
            }
        }

        public virtual Task<int> ReceiveAsync(Memory<byte> memory, CancellationToken cancellationToken)
        {
            if (!MemoryMarshal.TryGetArray((ReadOnlyMemory<byte>)memory, out var segment))
            {
                throw new ArgumentException("Memory<byte> must be backed by an array", nameof(memory));
            }

            lock (_receiveSync)
            {
                if (_pendingReadCanceled)
                {
                    ThrowTaskCanceledException();

                    static void ThrowTaskCanceledException()
                    {
                        throw new TaskCanceledException();
                    }
                }

                _receiveEventArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

                if (_receiveAsyncTaskSource != null)
                {
                    throw new InvalidOperationException("Concurrent calls to ReceiveAsync are not allowed");
                }

                _receiveAsyncTaskSource = _receiveAsyncTaskSourceCache ?? new TaskCompletionSource<int>();
                _receiveAsyncTaskSourceCache = null;

                try
                {
                    var receiveAsyncIsPending = _socket.ReceiveAsync(_receiveEventArgs);
                    if (receiveAsyncIsPending)
                    {
                        return _receiveAsyncTaskSource.Task;
                    }
                }
                catch
                {
                    _receiveAsyncTaskSourceCache = _receiveAsyncTaskSource;
                    _receiveAsyncTaskSource = null;
                    throw;
                }

                _receiveAsyncTaskSourceCache = _receiveAsyncTaskSource;
                _receiveAsyncTaskSource = null;

                if (_receiveEventArgs.SocketError != SocketError.Success)
                {
                    ThrowSocketException(_receiveEventArgs.SocketError);

                    static void ThrowSocketException(SocketError e)
                    {
                        throw new SocketException((int)e);
                    }
                }

                return Task.FromResult(_receiveEventArgs.BytesTransferred);
            }
        }

        public void ReceiveAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            TaskCompletionSource<int> receiveAsyncTaskSource;
            SocketError socketError;
            int bytesTransferred;
            lock (_receiveSync)
            {
                receiveAsyncTaskSource = _receiveAsyncTaskSource;
                socketError = _receiveEventArgs.SocketError;
                bytesTransferred = _receiveEventArgs.BytesTransferred;

                _receiveAsyncTaskSource = null;
            }

            // TODO: more robust guard against reading from cancelled socket?
            if (socketError != SocketError.Success)
            {
                receiveAsyncTaskSource?.TrySetException(new SocketException((int)socketError));
            }
            else
            {
                receiveAsyncTaskSource?.TrySetResult(bytesTransferred);
            }
        }

        public virtual void CancelPendingRead()
        {
            TaskCompletionSource<int> receiveAsyncTaskSource;

            lock (_receiveSync)
            {
                _pendingReadCanceled = true;
                receiveAsyncTaskSource = _receiveAsyncTaskSource;
                _receiveAsyncTaskSource = null;
            }

            // TODO: more robust guard against reading from cancelled socket?
            receiveAsyncTaskSource?.TrySetCanceled();
        }

        public virtual int Send(ReadOnlySequence<byte> data)
        {
            // TODO: avoid allocating this List<T>
            var segments = new List<ArraySegment<byte>>();
            foreach (var buffer in data)
            {
                MemoryMarshal.TryGetArray(buffer, out var segment);
                segments.Add(segment);
            }

            return _socket.Send(segments, SocketFlags.None);
        }

        public virtual void ShutdownSend()
        {
            _socket.Shutdown(SocketShutdown.Send);
        }
    }
}
