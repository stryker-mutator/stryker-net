using Microsoft.Extensions.Logging;
using System;

namespace Stryker.Core.Logging
{
    public class LogMessage
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