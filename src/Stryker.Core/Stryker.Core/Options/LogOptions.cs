using Serilog.Events;

namespace Stryker.Core.Options;

public class LogOptions
{
    public bool LogToFile { get; init; }
    public LogEventLevel LogLevel { get; init; }
}
