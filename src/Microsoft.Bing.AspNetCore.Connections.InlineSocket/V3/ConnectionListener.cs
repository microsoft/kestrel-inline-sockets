// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public class ConnectionListener : IConnectionListener
    {
        private readonly IListener _listener;

        public ConnectionListener(IListener listener)
        {
            _listener = listener;
        }

        public EndPoint EndPoint => _listener.EndPoint;

        public virtual async ValueTask<Microsoft.AspNetCore.Connections.ConnectionContext> AcceptAsync(CancellationToken cancellationToken = default)
        {
            var connection = await _listener.AcceptAsync(cancellationToken);
            if (connection == null)
            {
                return null;
            }

            return new ConnectionContext(connection);
        }

        public virtual async ValueTask UnbindAsync(CancellationToken cancellationToken = default)
        {
            await _listener.UnbindAsync(cancellationToken);
        }

        public virtual async ValueTask DisposeAsync()
        {
            await _listener.DisposeAsync();
        }
    }
}
