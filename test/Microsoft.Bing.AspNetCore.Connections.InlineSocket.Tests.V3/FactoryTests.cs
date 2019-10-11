// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.TestHelpers;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Tests.Stubs;
using Xunit;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Tests
{
    public class FactoryTests
    {
        [Fact]
        public async Task TransportFactoryCreate()
        {
            using var test = new TestContext();

            using var listener = test.Options.InlineSocketsOptions.CreateListener();

            Assert.NotNull(listener);

            await listener.DisposeAsync();
        }

        [Fact]
        public async Task ConnectionFactoryCreate()
        {
            using var test = new TestContext();

            var socket = new TestNetworkSocket();

            var connection = test.Options.InlineSocketsOptions.CreateConnection(socket);

            Assert.NotNull(connection);

            connection.Dispose();

            Assert.True(socket.IsDisposed);
        }
    }
}
