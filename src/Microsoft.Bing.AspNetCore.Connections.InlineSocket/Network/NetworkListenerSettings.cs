// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Net;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network
{
    public class NetworkListenerSettings
    {
        public IPEndPoint IPEndPoint { get; set; }

        public bool? AllowNatTraversal { get; set; }

        public bool? ExclusiveAddressUse { get; set; }

        public int? ListenerBacklog { get; set; }

        public bool? NoDelay { get; set; }
    }
}
