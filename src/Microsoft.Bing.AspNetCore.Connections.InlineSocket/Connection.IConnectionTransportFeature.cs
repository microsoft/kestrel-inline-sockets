// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.IO.Pipelines;
using Microsoft.AspNetCore.Connections.Features;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket
{
    public partial class Connection : IConnectionTransportFeature, IDuplexPipe
    {
        private IDuplexPipe _transport;

        public IDuplexPipe Transport
        {
            get => _transport ?? this;
            set => _transport = _options.WrapTransportPipelines(this, value);
        }

        PipeReader IDuplexPipe.Input => _socketInput;

        PipeWriter IDuplexPipe.Output => _socketOutput;
    }
}
