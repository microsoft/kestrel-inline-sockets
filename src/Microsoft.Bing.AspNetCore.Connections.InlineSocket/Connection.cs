// Copyright (c) Microsoft Corporation. All rights reserved.
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
using Microsoft.Extensions.Logging;

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
            _connectionClosedTokenSource.Token.Register(() => _logger.LogTrace("TODO: ConnectionClosed"));
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
            _logger.LogDebug("TODO: DisposeAsync {ConnectionId}", ConnectionId);

            await _socket.DisposeAsync();

            ((IDisposable)this).Dispose();
        }

        public virtual void Dispose()
        {
            _logger.LogDebug("TODO: Dispose {ConnectionId}", ConnectionId);

            (_socketInput as IDisposable)?.Dispose();
            (_socketOutput as IDisposable)?.Dispose();
            _socket.Dispose();

            _connectionClosedTokenSource.Dispose();
#if NETSTANDARD2_0
            _connectionCloseRequestedSource.Dispose();
#endif
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
            _logger.LogDebug(abortReason, "TODO: Abort {ConnectionId}", ConnectionId);

            // immediate FIN so client understands server will not complete current response or accept subsequent requests
            _socket.ShutdownSend();

            // stop any additional data from arriving
            _socketInput.CancelPendingRead();
        }
    }
}
