using Microsoft.Extensions.Logging;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

/// <summary>
/// Records the formatted messages of every enabled level, so a test can assert on what a component logged.
/// </summary>
internal sealed class CapturingLogger(bool debugEnabled = true) : ILogger
{
    public List<(LogLevel Level, string Message)> Entries { get; } = [];

    public IEnumerable<string> Messages => Entries.Select(entry => entry.Message);

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => debugEnabled || logLevel > LogLevel.Debug;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (IsEnabled(logLevel))
        {
            Entries.Add((logLevel, formatter(state, exception)));
        }
    }
}