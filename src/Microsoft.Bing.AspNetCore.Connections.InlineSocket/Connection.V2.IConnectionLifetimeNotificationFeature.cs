// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

#if NETSTANDARD2_0
using System;
using System.Threading;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.Extensions.Logging;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public partial class Connection : IConnectionLifetimeNotificationFeature
    {
        private readonly CancellationTokenSource _connectionCloseRequestedSource = new CancellationTokenSource();

        CancellationToken IConnectionLifetimeNotificationFeature.ConnectionClosedRequested
        {
            get => _connectionCloseRequestedSource.Token;
            set => throw new NotImplementedException();
        }

        void IConnectionLifetimeNotificationFeature.RequestClose()
        {
            _logger.ConnectionClosing(ConnectionId);

            _connectionCloseRequestedSource.Cancel(throwOnFirstException: false);
        }
    }
}
#endif
