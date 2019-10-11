// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if NETSTANDARD2_0
using System.Collections.Generic;
using System.Threading.Tasks;
using kestrel = Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Tests.Stubs
{
    public class TestConnectionDispatcher : kestrel.IConnectionDispatcher
    {
        public List<TransportConnection> Connections { get; } = new List<TransportConnection>();

        public Task OnConnection(TransportConnection connection)
        {
            Connections.Add(connection);
            return Task.CompletedTask;
        }

        public Task OnConnection(kestrel.TransportConnection connection)
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif
