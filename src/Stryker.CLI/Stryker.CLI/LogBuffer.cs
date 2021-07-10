using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using System;
using System.Collections.Generic;

namespace Stryker.CLI
{
    /// <summary>
    /// Allows to buffer log message and write them to an <see cref="ILogger" />.
    /// </summary>
    public class LogBuffer : ILogger
    {
        private List<LogMessage> _logMessages = new List<LogMessage>();

        public IReadOnlyCollection<LogMessage> GetMessages()
        {
            lock (_logMessages)
            {
                var messages = _logMessages;
                _logMessages = new List<LogMessage>();
                return messages;
            }
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            lock (_logMessages)
            {
                _logMessages.Add(new LogMessage(logLevel, eventId, state, typeof(TState), exception, (o, e) => formatter((TState)o, e)));
            }
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel) => true;

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state) => null;
    }
}
