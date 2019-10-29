// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public interface IListener : IDisposable, IAsyncDisposable
    {
        EndPoint EndPoint { get; }

        ValueTask BindAsync(EndPoint endpoint, CancellationToken cancellationToken = default);

        ValueTask UnbindAsync(CancellationToken cancellationToken = default);

        ValueTask<IConnection> AcceptAsync(CancellationToken cancellationToken = default);
    }
}
