// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if NETSTANDARD2_0
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Tests.Stubs
{
    public class TestEndPointInformation : IEndPointInformation
    {
        public ListenType Type { get; set; }

        public IPEndPoint IPEndPoint { get; set; }

        public string SocketPath { get; set; }

        public ulong FileHandle { get; set; }

        public FileHandleType HandleType { get; set; }

        public bool NoDelay { get; set; }
    }
}
#endif
