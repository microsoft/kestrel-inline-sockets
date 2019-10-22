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

        public static implicit operator LoggerMessage((LogLevel logLevel, string eventName, string message) details) => new LoggerMessage { Log = Microsoft.Extensions.Logging.LoggerMessage.Define(details.logLevel, LoggerMessage.EventId(details.eventName), details.message) };

        internal static EventId EventId(string eventName)
        {
            unchecked
            {
                var hash = 3074457345618258791ul;
                for (var index = 0; index < eventName.Length; ++index)
                {
                    hash += eventName[index];
                    hash *= 3074457345618258799ul;
                }

                return new EventId(100000 + (int)(hash % 900000), eventName);
            }
        }
    }

    internal struct LoggerMessage<T1>
    {
        public Action<ILogger, T1, Exception> Log { get; private set; }

        public static implicit operator LoggerMessage<T1>((LogLevel logLevel, EventId eventId, string message) details) => new LoggerMessage<T1> { Log = Microsoft.Extensions.Logging.LoggerMessage.Define<T1>(details.logLevel, details.eventId, details.message) };

        public static implicit operator LoggerMessage<T1>((LogLevel logLevel, int eventId, string eventName, string message) details) => new LoggerMessage<T1> { Log = Microsoft.Extensions.Logging.LoggerMessage.Define<T1>(details.logLevel, new EventId(details.eventId, details.eventName), details.message) };

        public static implicit operator LoggerMessage<T1>((LogLevel logLevel, string eventName, string message) details) => new LoggerMessage<T1> { Log = Microsoft.Extensions.Logging.LoggerMessage.Define<T1>(details.logLevel, LoggerMessage.EventId(details.eventName), details.message) };
    }

    internal struct LoggerMessage<T1, T2>
    {
        public Action<ILogger, T1, T2, Exception> Log { get; private set; }

        public static implicit operator LoggerMessage<T1, T2>((LogLevel logLevel, EventId eventId, string message) details) => new LoggerMessage<T1, T2> { Log = Microsoft.Extensions.Logging.LoggerMessage.Define<T1, T2>(details.logLevel, details.eventId, details.message) };

        public static implicit operator LoggerMessage<T1, T2>((LogLevel logLevel, int eventId, string eventName, string message) details) => new LoggerMessage<T1, T2> { Log = Microsoft.Extensions.Logging.LoggerMessage.Define<T1, T2>(details.logLevel, new EventId(details.eventId, details.eventName), details.message) };

        public static implicit operator LoggerMessage<T1, T2>((LogLevel logLevel, string eventName, string message) details) => new LoggerMessage<T1, T2> { Log = Microsoft.Extensions.Logging.LoggerMessage.Define<T1, T2>(details.logLevel, LoggerMessage.EventId(details.eventName), details.message) };
    }
}
