// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network
{
    public class NetworkProvider : INetworkProvider
    {
        private readonly ILogger<NetworkProvider> _logger;

        public NetworkProvider(ILogger<NetworkProvider> logger)
        {
            _logger = logger;
        }

        public virtual INetworkListener CreateListener(NetworkListenerSettings settings)
        {
            return new NetworkListener(_logger, settings);
        }
    }
}
