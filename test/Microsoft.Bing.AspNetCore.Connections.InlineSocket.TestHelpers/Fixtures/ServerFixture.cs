using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.TestHelpers.Fixtures
{
    public class ServerFixture : IDisposable
    {
        private readonly TimeoutFixture _timeout;
        private readonly ServicesFixture _services;
        private readonly AppFixture _app;
        private readonly EndPointFixture _endPoint;
        private IServer _server;

        public ServerFixture(
            TimeoutFixture timeout,
            ServicesFixture services,
            AppFixture app,
            EndPointFixture endPoint)
        {
            _timeout = timeout;
            _services = services;
            _app = app;
            _endPoint = endPoint;
        }

        public IServer Server => _server ?? Interlocked.CompareExchange(ref _server, _services.GetService<IServer>(), null) ?? _server;

        public async Task StartAsync()
        {
            _endPoint.FindUnusedPort();

            var serverAddresses = Server.Features.Get<IServerAddressesFeature>();
            serverAddresses.Addresses.Clear();
            serverAddresses.Addresses.Add(_endPoint.Address);

            await Server.StartAsync(_app, _timeout.Token);
        }

        public async Task StopAsync()
        {
            await Server.StopAsync(_timeout.Token);
        }

        public void Dispose()
        {
            _server?.Dispose();
            _server = null;
        }
    }
}
