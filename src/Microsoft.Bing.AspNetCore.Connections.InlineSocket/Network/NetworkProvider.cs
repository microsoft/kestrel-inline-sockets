// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network
{
    public class NetworkProvider : INetworkProvider
    {
        public virtual INetworkListener CreateListener(NetworkListenerSettings settings)
        {
            return new NetworkListener(settings);
        }
    }
}
