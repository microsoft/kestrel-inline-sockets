// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Tests.Fixtures
{
    public class ServicesFixture : IServiceProvider, IDisposable
    {
        private readonly ServiceProvider _serviceProvider;

        public ServicesFixture(LoggingFixture loggingFixture)
        {
            _serviceProvider = new ServiceCollection()
                .AddInlineSocketsTransport()
                .AddLogging(builder => loggingFixture?.ConfigureLogging(builder))
                .AddTransient<IConfigureOptions<KestrelServerOptions>, SetKestrelServerOptions>()
                .AddSingleton<IServer, KestrelServer>()
                .BuildServiceProvider();
        }

        public void Dispose()
        {
            _serviceProvider.Dispose();
        }

        public object GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }

        private class SetKestrelServerOptions : IConfigureOptions<KestrelServerOptions>
        {
            private readonly IServiceProvider _services;

            public SetKestrelServerOptions(IServiceProvider services)
            {
                _services = services;
            }

            public void Configure(KestrelServerOptions options)
            {
                options.ApplicationServices = _services;
            }
        }
    }
}
