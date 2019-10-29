// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.AspNetCore.Connections.Features;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public partial class Connection : IConnectionIdFeature
    {
        string IConnectionIdFeature.ConnectionId
        {
            get => ConnectionId;
            set => ConnectionId = value;
        }
    }
}
