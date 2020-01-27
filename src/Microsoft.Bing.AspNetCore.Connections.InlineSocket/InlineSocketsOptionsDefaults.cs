// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Reflection;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Logging;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Pipelines;
using Microsoft.Extensions.Options;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public class InlineSocketsOptionsDefaults : IConfigureOptions<InlineSocketsOptions>
    {
        private readonly Action<InlineSocketsOptions> _configure;

        public InlineSocketsOptionsDefaults(
            IListenerLogger listenerLogger,
            IConnectionLogger connectionLogger,
            INetworkProvider networkProvider)
        {
            _configure = options =>
            {
                var socketTransportOptions = new Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets.SocketTransportOptions();
                var socketTransportOptionsTypeInfo = socketTransportOptions.GetType().GetTypeInfo();
                var memoryPoolFactoryProperty = socketTransportOptionsTypeInfo.GetDeclaredProperty("MemoryPoolFactory");
                var memoryPoolFactory = memoryPoolFactoryProperty.GetValue(socketTransportOptions) as Func<MemoryPool<byte>>;
                var memoryPool = memoryPoolFactory.Invoke();

                options.MemoryPool = memoryPool;
                options.CreateListener = CreateListener;
                options.CreateConnection = CreateConnection;
                options.CreateSocketPipelines = CreateSocketPipelines;
                options.WrapTransportPipelines = WrapTransportPipelines;

                Listener CreateListener()
                {
                    return new Listener(listenerLogger, options, networkProvider);
                }

                IConnection CreateConnection(INetworkSocket socket)
                {
                    return new Connection(connectionLogger, options, socket);
                }

                (PipeReader input, PipeWriter output) CreateSocketPipelines(IConnection connection, INetworkSocket socket)
                {
                    var input = new SocketPipeReader(connectionLogger, options, connection, socket);
                    var output = new SocketPipeWriter(connectionLogger, options, connection, socket);
                    return (input, output);
                }

                static IDuplexPipe WrapTransportPipelines(IConnection connection, IDuplexPipe transport)
                {
                    var input = new CancelNotificationPipeReader(transport.Input, connection.CancelPendingRead);
                    return new DuplexPipe(input, transport.Output);
                }
            };
        }

        public void Configure(InlineSocketsOptions options) => _configure(options);
    }
}
