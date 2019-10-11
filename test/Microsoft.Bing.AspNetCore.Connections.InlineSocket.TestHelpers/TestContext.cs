using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.TestHelpers.Fixtures;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Tests.Fixtures;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.TestHelpers
{
    public class TestContext : IDisposable
    {
        private readonly ServiceProvider _fixtures;

        public TestContext()
        {
            _fixtures = new ServiceCollection()
                .AddSingleton<AppFixture>()
                .AddSingleton<EndPointFixture>()
                .AddSingleton<LoggingFixture>()
                .AddSingleton<ServerFixture>()
                .AddSingleton<ServicesFixture>()
                .AddSingleton<TimeoutFixture>()
                .AddSingleton<OptionsFixture>()
                .AddSingleton<ClientFixture>()
                .BuildServiceProvider();
        }

        public AppFixture App => _fixtures.GetService<AppFixture>();
        public EndPointFixture EndPoint => _fixtures.GetService<EndPointFixture>();
        public LoggingFixture Logging => _fixtures.GetService<LoggingFixture>();
        public ServerFixture Server => _fixtures.GetService<ServerFixture>();
        public ServicesFixture Services => _fixtures.GetService<ServicesFixture>();
        public TimeoutFixture Timeout => _fixtures.GetService<TimeoutFixture>();
        public OptionsFixture Options => _fixtures.GetService<OptionsFixture>();
        public ClientFixture Client => _fixtures.GetService<ClientFixture>();

        public void Dispose()
        {
            _fixtures.Dispose();
        }
    }
}
