// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Logging
{
    internal struct LoggerMessage
    {
        public Action<ILogger, Exception> Log { get; private set; }

        public static implicit operator LoggerMessage((LogLevel logLevel, EventId eventId, string message) details) => new LoggerMessage { Log = Microsoft.Extensions.Logging.LoggerMessage.Define(details.logLevel, details.eventId, details.message) };

        public static implicit operator LoggerMessage((LogLevel logLevel, int eventId, string eventName, string message) details) => new LoggerMessage { Log = Microsoft.Extensions.Logging.LoggerMessage.Define(details.logLevel, new EventId(details.eventId, details.eventName), details.message) };
    }

    internal struct LoggerMessage<T1>
    {
        public Action<ILogger, T1, Exception> Log { get; private set; }

        public static implicit operator LoggerMessage<T1>((LogLevel logLevel, EventId eventId, string message) details) => new LoggerMessage<T1> { Log = Microsoft.Extensions.Logging.LoggerMessage.Define<T1>(details.logLevel, details.eventId, details.message) };

        public static implicit operator LoggerMessage<T1>((LogLevel logLevel, int eventId, string eventName, string message) details) => new LoggerMessage<T1> { Log = Microsoft.Extensions.Logging.LoggerMessage.Define<T1>(details.logLevel, new EventId(details.eventId, details.eventName), details.message) };
    }

    internal struct LoggerMessage<T1, T2>
    {
        public Action<ILogger, T1, T2, Exception> Log { get; private set; }

        public static implicit operator LoggerMessage<T1, T2>((LogLevel logLevel, EventId eventId, string message) details) => new LoggerMessage<T1, T2> { Log = Microsoft.Extensions.Logging.LoggerMessage.Define<T1, T2>(details.logLevel, details.eventId, details.message) };

        public static implicit operator LoggerMessage<T1, T2>((LogLevel logLevel, int eventId, string eventName, string message) details) => new LoggerMessage<T1, T2> { Log = Microsoft.Extensions.Logging.LoggerMessage.Define<T1, T2>(details.logLevel, new EventId(details.eventId, details.eventName), details.message) };
    }
}
