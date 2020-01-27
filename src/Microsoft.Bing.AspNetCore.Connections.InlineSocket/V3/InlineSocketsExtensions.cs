// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Logging;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class InlineSocketsExtensions
    {
        public static IServiceCollection AddInlineSocketsTransport(this IServiceCollection services)
        {
            return services
                .AddTransient<IConfigureOptions<InlineSocketsOptions>, InlineSocketsOptionsDefaults>()
                .AddTransient<IConfigureOptions<KestrelServerOptions>, KestrelServerOptionsDefaults>()
                .AddTransient<IConnectionListenerFactory, ConnectionListenerFactory>()
                .AddTransient<INetworkProvider, NetworkProvider>()
                .AddTransient<IListenerLogger, ListenerLogger>()
                .AddTransient<IConnectionLogger, ConnectionLogger>();
        }
    }
}
