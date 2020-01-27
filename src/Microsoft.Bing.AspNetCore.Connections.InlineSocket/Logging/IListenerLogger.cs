// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Logging
{
    public interface IListenerLogger : ILogger
    {
        void BindListenSocket(EndPoint endPoint);

        void UnbindListenSocket(EndPoint endPoint);

        void StopListener();

        void SocketAccepted(EndPoint remoteEndPoint, EndPoint localEndPoint);

        void ConnectionDispatchFailed(string connectionId, Exception error);

        void ConnectionReset(string connectionId);
    }
}
