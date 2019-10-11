// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading.Tasks;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network
{
    public interface INetworkListener : IDisposable
    {
        void Start();

        void Stop();

        Task<INetworkSocket> AcceptSocketAsync();
    }
}
