// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Net.Sockets;
using System.Threading.Tasks;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network
{
    public class NetworkListener : INetworkListener
    {
        private readonly TcpListener _listener;
        private readonly int? _listenerBacklog;
        private readonly bool? _socketNoDelay;

        public NetworkListener(NetworkListenerSettings settings)
        {
            // TODO: logic to bind ipv4 and/or ipv6 ?
            _listener = new TcpListener(settings.IPEndPoint);

            if (settings.ExclusiveAddressUse.HasValue)
            {
                _listener.ExclusiveAddressUse = settings.ExclusiveAddressUse.Value;
            }

            if (settings.AllowNatTraversal.HasValue)
            {
                _listener.AllowNatTraversal(settings.AllowNatTraversal.Value);
            }

            _listenerBacklog = settings.ListenerBacklog;
            _socketNoDelay = settings.NoDelay;
        }

        public virtual void Dispose()
        {
            _listener.Stop();
        }

        public virtual void Start()
        {
            if (_listenerBacklog.HasValue)
            {
                _listener.Start(_listenerBacklog.Value);
            }
            else
            {
                _listener.Start();
            }
        }

        public virtual void Stop()
        {
            _listener.Stop();
        }

        public virtual async Task<INetworkSocket> AcceptSocketAsync()
        {
            var socket = await _listener.AcceptSocketAsync();
            if (_socketNoDelay.HasValue)
            {
                socket.NoDelay = _socketNoDelay.Value;
            }

            return new NetworkSocket(socket);
        }
    }
}
