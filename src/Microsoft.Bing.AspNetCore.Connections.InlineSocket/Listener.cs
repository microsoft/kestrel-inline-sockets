// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Logging;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public class Listener : IListener
    {
        private readonly IListenerLogger _logger;
        private readonly InlineSocketsOptions _options;
        private readonly INetworkProvider _networkProvider;
        private EndPoint _endpoint;
        private INetworkListener _listener;
        private Action _listenerCancellationCallback;

        public Listener(
            IListenerLogger logger,
            InlineSocketsOptions options,
            INetworkProvider networkProvider)
        {
            _logger = logger;
            _options = options;
            _networkProvider = networkProvider;
        }

        public virtual EndPoint EndPoint => _endpoint;

        public virtual async ValueTask BindAsync(EndPoint endpoint, CancellationToken cancellationToken = default)
        {
            _endpoint = endpoint;
            _logger.BindListenSocket(_endpoint);

            _listener = _networkProvider.CreateListener(new NetworkListenerSettings
            {
                EndPoint = _endpoint,
                AllowNatTraversal = _options.AllowNatTraversal,
                ExclusiveAddressUse = _options.ExclusiveAddressUse,
                ListenerBacklog = _options.ListenBacklog,
                NoDelay = _options.NoDelay,
            });

            // the only way to cancel a call to accept a socket is to stop the listener.
            // this is okay, because this call is cancelled only when the listener is about to be
            // unbound and disposed anyway.
            _listenerCancellationCallback = _listener.Stop;

            _listener.Start();
        }

        public virtual async ValueTask UnbindAsync(CancellationToken cancellationToken = default)
        {
            _logger.UnbindListenSocket(_endpoint);
            _listener.Stop();
        }

        public virtual void Dispose()
        {
            _listener?.Dispose();
            _listener = null;
            _listenerCancellationCallback = null;
        }

        public virtual async ValueTask DisposeAsync()
        {
            Dispose();
        }

        public virtual async ValueTask<IConnection> AcceptAsync(CancellationToken cancellationToken = default)
        {
            using (cancellationToken.Register(_listenerCancellationCallback))
            {
                while (true)
                {
                    try
                    {
                        var socket = await _listener.AcceptSocketAsync();
                        _logger.SocketAccepted(socket.RemoteEndPoint, socket.LocalEndPoint);
                        return _options.CreateConnection(socket);
                    }
                    catch (ObjectDisposedException)
                    {
                        // A call was made to UnbindAsync/DisposeAsync just return null which signals we're done
                        return null;
                    }
                    catch (SocketException e) when (e.SocketErrorCode == SocketError.OperationAborted)
                    {
                        // A call was made to UnbindAsync/DisposeAsync just return null which signals we're done
                        return null;
                    }
                    catch (InvalidOperationException)
                    {
                        // Stopping the server immediately can cause this exception
                        // "Not listening. You must call the Start() method before calling this method."
                        return null;
                    }
                    catch (SocketException)
                    {
                        // The connection got reset while it was in the backlog, so we try again.
                        _logger.ConnectionReset(connectionId: "(null)");
                    }
                }
            }
        }
    }
}
