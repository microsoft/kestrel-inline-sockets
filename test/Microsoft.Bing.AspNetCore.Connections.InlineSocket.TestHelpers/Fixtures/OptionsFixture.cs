using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Bing.AspNetCore.Connections.InlineSocket.Tests.Fixtures;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.TestHelpers.Fixtures
{
    public class OptionsFixture
    {
        private readonly ServicesFixture _services;

        public OptionsFixture(ServicesFixture services)
        {
            _services = services;
        }

        public InlineSocketsOptions InlineSocketsOptions => _services.GetService<IOptions<InlineSocketsOptions>>().Value;
    }
}
