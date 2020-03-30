// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.AspNetCore.Connections.Features;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public partial class Connection : IConnectionOutputControlFeature
    {
        bool IConnectionOutputControlFeature.IsSuspended => (_socketOutput as IConnectionOutputControlFeature)?.IsSuspended ?? false;

        void IConnectionOutputControlFeature.Suspend()
        {
            (_socketOutput as IConnectionOutputControlFeature)?.Suspend();
        }

        void IConnectionOutputControlFeature.Resume()
        {
            (_socketOutput as IConnectionOutputControlFeature)?.Resume();
        }
    }
}
