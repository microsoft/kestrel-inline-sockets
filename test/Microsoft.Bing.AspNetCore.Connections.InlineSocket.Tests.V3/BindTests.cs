// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.TestHelpers;
using Xunit;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Tests
{
    public class BindTests
    {
        [Fact]
        public async Task BindCreatesSocketWhichAcceptsConnection()
        {
            using var test = new TestContext();

            // create listener and bind to unused port
            using var listener = test.Options.InlineSocketsOptions.CreateListener();
            test.EndPoint.FindUnusedPort();
            await listener.BindAsync(test.EndPoint.IPEndPoint, test.Timeout.Token);

            // accept connection which will not be available yet
            var acceptTask = listener.AcceptAsync(test.Timeout.Token);
            Assert.False(acceptTask.IsCompleted);

            // create client socket and connect
            var client = new Socket(SocketType.Stream, ProtocolType.IP);
            client.Connect(test.EndPoint.IPEndPoint);

            // finish accepting connection from listener
            using var connection = await acceptTask;

            // verify connection's remote port is same as client's local port
            var localIPEndPoint = (IPEndPoint)client.LocalEndPoint;
            var remoteIPEndPoint = (IPEndPoint)connection.RemoteEndPoint;
            Assert.Equal(localIPEndPoint.Port, remoteIPEndPoint.Port);

            client.Dispose();
            await connection.DisposeAsync();
            await listener.DisposeAsync();
        }
    }
}

