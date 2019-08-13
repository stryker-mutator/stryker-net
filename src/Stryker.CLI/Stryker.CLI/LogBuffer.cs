using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Stryker.CLI
{
    /// <summary>
    /// Allows to buffer log message and write them to an <see cref="ILogger"/>.
    /// </summary>
    public class LogBuffer : ILogger
    {
        private readonly List<LogMessage> _logMessages = new List<LogMessage>();
        private readonly MethodInfo _logMethodInfo = typeof(ILogger).GetMethod("Log");

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

        public void WriteToLog(ILogger logger)
        {
            lock (_logMessages)
            {
                foreach (var logMessage in _logMessages)
                {
                    // Create the generic variant of the method to make sure any consumer can use typeof(TState)
                    _logMethodInfo.MakeGenericMethod(logMessage.StateType)
                        .Invoke(
                            logger,
                            new [] { logMessage.LogLevel, logMessage.EventId, logMessage.State, logMessage.Exception, logMessage.Formatter });
                }

                _logMessages.Clear();
            }
        }

        private class LogMessage
        {
            public LogMessage(
                LogLevel logLevel,
                EventId eventId,
                object state,
                Type stateType,
                Exception exception,
                Func<object, Exception, string> formatter)
            {
                LogLevel = logLevel;
                EventId = eventId;
                State = state;
                StateType = stateType;
                Exception = exception;
                Formatter = formatter;
            }

            public LogLevel LogLevel { get; }

            public EventId EventId { get; }

            public object State { get; }

            public Type StateType { get; }

            public Exception Exception { get; }

            public Func<object, Exception, string> Formatter { get; }
        }
    }
}