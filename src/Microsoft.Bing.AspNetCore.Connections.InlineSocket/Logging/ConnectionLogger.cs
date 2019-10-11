// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Logging;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Logging
{
    public class ConnectionLogger : ForwardingLogger, IConnectionLogger
    {
        public ConnectionLogger(ILoggerFactory loggerFactory)
            : base(loggerFactory, typeof(Connection).FullName)
        {
        }
    }
}
