using Serilog.Events;

namespace Stryker.Shared.Options;
public interface ILogOptions
{
    bool LogToFile { get; init; }
    LogEventLevel LogLevel { get; init; }
}
