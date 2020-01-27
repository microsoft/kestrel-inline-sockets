// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public class KestrelServerOptionsDefaults : IConfigureOptions<KestrelServerOptions>
    {
        public void Configure(KestrelServerOptions options)
        {
            options.AllowSynchronousIO = true;
        }
    }
}
