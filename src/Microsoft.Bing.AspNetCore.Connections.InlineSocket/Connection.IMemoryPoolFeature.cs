// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Buffers;
using Microsoft.AspNetCore.Connections.Features;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public partial class Connection : IMemoryPoolFeature
    {
        MemoryPool<byte> IMemoryPoolFeature.MemoryPool => _options.MemoryPool;
    }
}
