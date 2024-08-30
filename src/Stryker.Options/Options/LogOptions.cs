using Serilog.Events;

namespace Stryker.Abstractions.Options
{
    public class LogOptions : ILogOptions
    {
        public bool LogToFile { get; init; }
        public LogEventLevel LogLevel { get; init; }
    }
}
