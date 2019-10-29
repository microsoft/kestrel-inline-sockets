// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using Microsoft.Extensions.Logging;

namespace Microsoft.Bing.AspNetCore.Connections.InlineSocket.Logging
{
    public abstract class ForwardingLogger : ILogger
    {
        private readonly ILogger _logger;

        public ForwardingLogger(ILoggerFactory loggerFactory, string categoryName)
        {
            _logger = loggerFactory.CreateLogger(categoryName);
        }

        public virtual IDisposable BeginScope<TState>(TState state)
        {
            return _logger.BeginScope(state);
        }

        public virtual bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}
