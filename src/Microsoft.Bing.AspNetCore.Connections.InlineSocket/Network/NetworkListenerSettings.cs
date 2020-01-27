// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Net;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network
{
    public class NetworkListenerSettings
    {
        public EndPoint EndPoint { get; set; }

        public bool? AllowNatTraversal { get; set; }

        public bool? ExclusiveAddressUse { get; set; }

        public int? ListenerBacklog { get; set; }

        public bool? NoDelay { get; set; }
    }
}
