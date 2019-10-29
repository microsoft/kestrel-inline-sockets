// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Threading;
using Microsoft.AspNetCore.Connections.Features;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public partial class Connection : IConnectionLifetimeFeature
    {
        CancellationToken IConnectionLifetimeFeature.ConnectionClosed
        {
            get => _connectionClosedTokenSource.Token;
            set => throw new NotImplementedException();
        }

        void IConnectionLifetimeFeature.Abort()
        {
            Abort(null);
        }
    }
}
