// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Buffers;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network
{
    public interface INetworkSocket : IDisposable, IAsyncDisposable
    {
        IPEndPoint LocalEndPoint { get; }

        IPEndPoint RemoteEndPoint { get; }

        Task<int> ReceiveAsync(Memory<byte> buffers, CancellationToken cancellationToken);

        void CancelPendingRead();

        int Send(ReadOnlySequence<byte> buffers);

        void ShutdownSend();
    }
}
