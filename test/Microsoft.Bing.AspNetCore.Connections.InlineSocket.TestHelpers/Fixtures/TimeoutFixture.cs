using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.TestHelpers.Fixtures
{
    public class TimeoutFixture : IDisposable
    {
        private readonly Lazy<CancellationTokenSource> _cts;

        public TimeoutFixture()
        {
            _cts = new Lazy<CancellationTokenSource>(() => new CancellationTokenSource(Debugger.IsAttached ? TimeSpan.FromMinutes(5) : TimeSpan.FromSeconds(25)));
        }

        public CancellationToken Token => _cts.Value.Token;

        public void Dispose()
        {
        }
    }
}
