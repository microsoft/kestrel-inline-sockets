// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.IO.Pipelines;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Logging;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public partial class Connection : IConnection
    {
        private readonly IConnectionLogger _logger;
        private readonly InlineSocketsOptions _options;
        private readonly INetworkSocket _socket;
        private readonly PipeReader _socketInput;
        private readonly PipeWriter _socketOutput;
        private readonly CancellationTokenSource _connectionClosedTokenSource;
        private readonly CancellationTokenRegistration _connectionClosedTokenRegistration;

        private int _disposed;
        private string _connectionId;

        public Connection(
            IConnectionLogger logger,
            InlineSocketsOptions options,
            INetworkSocket socket)
        {
            Features = new FeatureCollection(this);
            _logger = logger;
            _options = options;
            _socket = socket;
            RemoteEndPoint = _socket.RemoteEndPoint;
            LocalEndPoint = _socket.LocalEndPoint;

            (_socketInput, _socketOutput) = options.CreateSocketPipelines(this, socket);

            _connectionClosedTokenSource = new CancellationTokenSource();
            _connectionClosedTokenRegistration = _connectionClosedTokenSource.Token.Register(LogConnectionClosed);

            void LogConnectionClosed()
            {
                _logger.ConnectionClosed(ConnectionId);
            }
        }

        public virtual IFeatureCollection Features { get; private set; }

        public virtual string ConnectionId
        {
            get => _connectionId ?? Interlocked.CompareExchange(ref _connectionId, CorrelationIdGenerator.GetNextId(), null) ?? _connectionId;
            set => _connectionId = value;
        }

        public virtual EndPoint LocalEndPoint { get; set; }

        public virtual EndPoint RemoteEndPoint { get; set; }

        public virtual async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                _logger.ConnectionDisposed(ConnectionId, isAsync: true);

                (_socketInput as IDisposable)?.Dispose();
                (_socketOutput as IDisposable)?.Dispose();
                await _socket.DisposeAsync();

                _connectionClosedTokenRegistration.Dispose();
                _connectionClosedTokenSource.Dispose();
            }
        }

        public virtual void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                _logger.ConnectionDisposed(ConnectionId, isAsync: false);

                (_socketInput as IDisposable)?.Dispose();
                (_socketOutput as IDisposable)?.Dispose();
                _socket.Dispose();

                _connectionClosedTokenRegistration.Dispose();
                _connectionClosedTokenSource.Dispose();
            }
        }

        public virtual void CancelPendingRead()
        {
            _socketInput.CancelPendingRead();
        }

        public virtual void FireConnectionClosed()
        {
            _connectionClosedTokenSource.Cancel();
        }

        public virtual void Abort(ConnectionAbortedException abortReason)
        {
            _logger.ConnectionAborting(ConnectionId, abortReason);

            // immediate FIN so client understands server will not complete current response or accept subsequent requests
            _socket.ShutdownSend();

            // stop any additional data from arriving
            _socketInput.CancelPendingRead();
        }
    }
}
