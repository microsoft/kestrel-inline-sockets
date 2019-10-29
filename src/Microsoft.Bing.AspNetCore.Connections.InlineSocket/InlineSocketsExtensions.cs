// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class InlineSocketsExtensions
    {
        public static IServiceCollection AddInlineSocketsTransport(this IServiceCollection services, Action<InlineSocketsOptions> configuration)
        {
            return AddInlineSocketsTransport(services).Configure(configuration);
        }
    }
}
