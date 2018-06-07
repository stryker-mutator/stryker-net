
using Serilog.Events;

namespace Stryker.Core.Logging
{
    public class LogOptions
    {
        public bool LogToFile { get; }
        public LogEventLevel LogLevel { get; }

        public LogOptions(string logLevel, bool logToFile)
        {
            LogLevel = GetLogLevel(logLevel);
            LogToFile = logToFile;
        }

        private LogEventLevel GetLogLevel(string levelText)
        {
            switch (levelText?.ToLower() ?? "")
            {
                case "info":
                    return LogEventLevel.Information;
                case "debug":
                    return LogEventLevel.Debug;
                case "trace":
                    return LogEventLevel.Verbose;
                default:
                    return LogEventLevel.Warning;
            }
        }
    }
}
