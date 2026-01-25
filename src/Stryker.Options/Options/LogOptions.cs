using Serilog.Events;
using Stryker.Abstractions.Options;

namespace Stryker.Configuration.Options;

public class LogOptions : ILogOptions
{
    public bool LogToFile { get; init; }
    public LogEventLevel LogLevel { get; init; }
}
