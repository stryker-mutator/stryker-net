using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Stryker.Core.UnitTest
{
    public class MockLogger : ILogger
    {
        public int LogHitCount => LogMessages.Count;
        public List<string> LogMessages { get; private set; } = new List<string>();

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            LogMessages.Add(formatter.Invoke(state, exception));
        }
    }
}
