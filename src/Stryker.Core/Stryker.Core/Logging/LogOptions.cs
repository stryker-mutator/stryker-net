using Serilog.Events;

namespace Stryker.Core.Logging
{
    public class LogOptions
    {
        public bool? LogToFile { get; }
        public LogEventLevel LogLevel { get; }

        public LogOptions(LogEventLevel logLevel, bool? logToFile)
        {
            LogLevel = logLevel;
            LogToFile = logToFile;
        }
    }
}
