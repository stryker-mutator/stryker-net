using Serilog.Events;

namespace Stryker.Abstractions.Options;

public interface ILogOptions
{
    LogEventLevel LogLevel { get; init; }
    bool LogToFile { get; init; }
}
