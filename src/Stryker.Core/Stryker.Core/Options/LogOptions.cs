using Serilog.Events;
using Stryker.Shared.Options;

namespace Stryker.Core.Options;

public class LogOptions : ILogOptions
{
    public bool LogToFile { get; init; }
    public LogEventLevel LogLevel { get; init; }
}
