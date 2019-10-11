// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Logging;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network;
using Microsoft.Extensions.Options;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public class Transport : ITransport
    {
        private readonly Listener _listener;
        private readonly IListenerLogger _logger;
        private readonly IEndPointInformation _endPointInformation;
        private readonly IConnectionDispatcher _dispatcher;
        private readonly CancellationTokenSource _acceptLoopCancellation = new CancellationTokenSource();
        private Task _acceptLoopTask;

        public Transport(
            IListenerLogger logger,
            InlineSocketsOptions options,
            INetworkProvider networkProvider,
            IEndPointInformation endPointInformation,
            IConnectionDispatcher dispatcher)
        {
            _listener = new Listener(logger, options, networkProvider);
            _logger = logger;
            _endPointInformation = endPointInformation;
            _dispatcher = dispatcher;
        }

        public virtual async Task BindAsync()
        {
            await _listener.BindAsync(
                _endPointInformation.IPEndPoint);

            _acceptLoopTask = Task.Run(() => AcceptLoopAsync(_acceptLoopCancellation.Token));
        }

        public virtual async Task UnbindAsync()
        {
            _acceptLoopCancellation.Cancel();

            try
            {
                await _acceptLoopTask;
            }
            catch (TaskCanceledException)
            {
                // normal exit via cancellation
            }
            catch (OperationCanceledException)
            {
                // normal exit via cancellation
            }

            await _listener.UnbindAsync();
        }

        public virtual async Task StopAsync()
        {
            await _listener.DisposeAsync();
        }

        public virtual async Task AcceptLoopAsync(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var connection = await _listener.AcceptAsync(cancellationToken);
                if (connection != null)
                {
                    var task = DispatchConnectionAsync(connection);
                    HandleExceptions(connection.ConnectionId, task);
                }
            }
        }

        private async Task DispatchConnectionAsync(IConnection connection)
        {
            try
            {
                var transportConnection = new TransportConnection(connection);
                await _dispatcher.OnConnection(transportConnection);
            }
            finally
            {
                await connection.DisposeAsync();
            }
        }

        private async void HandleExceptions(string connectionId, Task task)
        {
            try
            {
                await task;
            }
            catch (Exception error)
            {
                _logger.ConnectionDispatchFailed(connectionId, error);
            }
        }
    }
}
#endif
