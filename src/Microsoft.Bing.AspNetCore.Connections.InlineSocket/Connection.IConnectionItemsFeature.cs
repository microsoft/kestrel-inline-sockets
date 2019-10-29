// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Connections.Features;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public partial class Connection : IConnectionItemsFeature
    {
        private IDictionary<object, object> _items;

        IDictionary<object, object> IConnectionItemsFeature.Items
        {
            get => _items ?? Interlocked.CompareExchange(ref _items, new Dictionary<object, object>(), null) ?? _items;
            set => _items = value;
        }
    }
}
