// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Network
{
    public class NetworkListener : INetworkListener
    {
        private readonly EndPoint _serverSocketEP;
        private readonly int _listenerBacklog;
        private readonly bool _isIPEndPoint;
        private readonly bool? _socketNoDelay;
        private Socket _serverSocket;

        public NetworkListener(NetworkListenerSettings settings)
        {
            _serverSocketEP = settings.EndPoint;

            var protocolType = ProtocolType.Unspecified;

            var ip = _serverSocketEP as IPEndPoint;
            if (ip != null)
            {
                protocolType = ProtocolType.Tcp;
                _isIPEndPoint = true;
            }

            _serverSocket = new Socket(_serverSocketEP.AddressFamily, SocketType.Stream, protocolType);

            if (Equals(ip?.Address, IPAddress.IPv6Any))
            {
                _serverSocket.DualMode = true;
            }

            if (settings.ExclusiveAddressUse.HasValue)
            {
                _serverSocket.ExclusiveAddressUse = settings.ExclusiveAddressUse.Value;
            }

            if (settings.AllowNatTraversal.HasValue)
            {
                _serverSocket.SetIPProtectionLevel(settings.AllowNatTraversal.Value ? IPProtectionLevel.Unrestricted : IPProtectionLevel.EdgeRestricted);
            }

            _listenerBacklog = settings.ListenerBacklog ?? (int)SocketOptionName.MaxConnections;
            _socketNoDelay = settings.NoDelay;
        }

        public virtual void Dispose()
        {
            StopInternal();
        }

        public virtual void Start()
        {
            _serverSocket.Bind(_serverSocketEP);
            try
            {
                _serverSocket.Listen(_listenerBacklog);
            }
            catch (SocketException)
            {
                // When there is an exception, unwind previous actions (bind, etc).
                StopInternal();
                throw;
            }
        }

        public virtual void Stop()
        {
            StopInternal();
        }

        public virtual async Task<INetworkSocket> AcceptSocketAsync()
        {
            var socket = await _serverSocket.AcceptAsync();

            if (_isIPEndPoint && _socketNoDelay.HasValue)
            {
                socket.NoDelay = _socketNoDelay.Value;
            }

            return new NetworkSocket(socket);
        }

        private void StopInternal()
        {
            _serverSocket?.Dispose();
            _serverSocket = null;
        }
    }
}
