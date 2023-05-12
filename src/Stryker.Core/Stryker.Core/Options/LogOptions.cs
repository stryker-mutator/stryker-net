namespace Stryker.Core.Options;
using Serilog.Events;

public class LogOptions
{
    public bool LogToFile { get; init; }
    public LogEventLevel LogLevel { get; init; }
}
