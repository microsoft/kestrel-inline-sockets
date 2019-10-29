// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.IO.Pipelines;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Pipelines
{
    internal class DuplexPipe : IDuplexPipe
    {
        public DuplexPipe(PipeReader input, PipeWriter output)
        {
            Input = input;
            Output = output;
        }

        public PipeReader Input { get; }

        public PipeWriter Output { get; }
    }
}
