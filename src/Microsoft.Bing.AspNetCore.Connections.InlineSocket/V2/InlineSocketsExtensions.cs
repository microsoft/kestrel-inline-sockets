// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if NETSTANDARD2_0
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
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
                .AddTransient<ITransportFactory, TransportFactory>()
                .AddTransient<INetworkProvider, NetworkProvider>()
                .AddTransient<IListenerLogger, ListenerLogger>()
                .AddTransient<IConnectionLogger, ConnectionLogger>();
        }
    }
}
#endif

