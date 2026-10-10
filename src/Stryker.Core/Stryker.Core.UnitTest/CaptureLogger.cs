using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Stryker.Core.UnitTest;

internal sealed class CaptureLogger<T> : ILogger<T>
{
    public List<(LogLevel Level, string Message)> Entries { get; } = new();

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) =>
        Entries.Add((logLevel, formatter(state, exception)));
}
