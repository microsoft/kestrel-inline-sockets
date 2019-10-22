// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Logging
{
    public class ListenerLogger : ForwardingLogger, IListenerLogger
    {
        private static readonly LoggerMessage<IPEndPoint> _logBindListenSocket = (LogLevel.Debug, nameof(BindListenSocket), "Binding listen socket to {IPEndPoint}");
        private static readonly LoggerMessage<IPEndPoint> _logUnbindListenSocket = (LogLevel.Debug, nameof(UnbindListenSocket), "Unbinding listen socket from {IPEndPoint}");
        private static readonly LoggerMessage _logStopListener = (LogLevel.Debug, nameof(StopListener), "Inline sockets transport is stopped");
        private static readonly LoggerMessage<EndPoint, EndPoint> _logSocketAccepted = (LogLevel.Information, nameof(SocketAccepted), "Socket accepted from {RemoteEndPoint} to {LocalEndPoint}");
        private static readonly LoggerMessage<string> _logConnectionDispatchFailed = (LogLevel.Debug, nameof(ConnectionDispatchFailed), "Unexpected failure thrown by IConnectionDispatcher.OnConnection of connection '{ConnectionId}'");
        private static readonly LoggerMessage<string> _logConnectionReset = (LogLevel.Debug, nameof(ConnectionReset), "Connection '{ConnectionId}' reset");

        public ListenerLogger(ILoggerFactory loggerFactory)
            : base(loggerFactory, typeof(Listener).FullName)
        {
        }

        public virtual void BindListenSocket(IPEndPoint ipEndPoint) => _logBindListenSocket.Log(this, ipEndPoint, null);

        public virtual void UnbindListenSocket(IPEndPoint ipEndPoint) => _logUnbindListenSocket.Log(this, ipEndPoint, null);

        public virtual void StopListener() => _logStopListener.Log(this, null);

        public virtual void SocketAccepted(EndPoint remoteEndPoint, EndPoint localEndPoint) => _logSocketAccepted.Log(this, remoteEndPoint, localEndPoint, null);

        public virtual void ConnectionDispatchFailed(string connectionId, Exception error) => _logConnectionDispatchFailed.Log(this, connectionId, error);

        public virtual void ConnectionReset(string connectionId) => _logConnectionReset.Log(this, connectionId, null);
    }
}
