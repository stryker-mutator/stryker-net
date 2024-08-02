using Serilog.Events;

namespace Stryker.Configuration.Options;

public interface ILogOptions
{
    LogEventLevel LogLevel { get; init; }
    bool LogToFile { get; init; }
}
