// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Net;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Features;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public interface IConnection : IDisposable, IAsyncDisposable
    {
        IFeatureCollection Features { get; }

        string ConnectionId { get; set; }

        EndPoint LocalEndPoint { get; set; }

        EndPoint RemoteEndPoint { get; set; }

        void Abort(ConnectionAbortedException abortReason);

        void FireConnectionClosed();

        void CancelPendingRead();
    }
}
