// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network
{
    public interface INetworkProvider
    {
        INetworkListener CreateListener(NetworkListenerSettings settings);
    }
}
