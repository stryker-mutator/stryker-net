using Serilog.Events;

namespace Stryker.Core.Logging
{
    public class LogOptions
    {
        public bool LogToFile { get; }
        public string OutputPath { get; }
        public LogEventLevel LogLevel { get; }

        public LogOptions(LogEventLevel logLevel, bool logToFile, string outputPath)
        {
            LogLevel = logLevel;
            LogToFile = logToFile;
            OutputPath = outputPath;
        }
    }
}
