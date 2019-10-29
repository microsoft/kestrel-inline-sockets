// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

#if NETCOREAPP3_0
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Options;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public class ConnectionListenerFactory : IConnectionListenerFactory
    {
        private readonly InlineSocketsOptions _options;

        public ConnectionListenerFactory(IOptions<InlineSocketsOptions> options)
        {
            _options = options.Value;
        }

        public virtual async ValueTask<IConnectionListener> BindAsync(EndPoint endpoint, CancellationToken cancellationToken = default)
        {
            var listener = _options.CreateListener();
            await listener.BindAsync(endpoint, cancellationToken);
            return new ConnectionListener(listener);
        }
    }
}
#endif
