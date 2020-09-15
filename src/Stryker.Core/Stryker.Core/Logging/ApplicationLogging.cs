using Microsoft.Extensions.Logging;
using Serilog;
using System.Collections.Generic;
using System.IO;

namespace Stryker.Core.Logging
{
    using LibGit2Sharp;

    using Serilog.Events;

    using MSLogLevel = Microsoft.Extensions.Logging.LogLevel;

    public static class ApplicationLogging
    {
        private static readonly Dictionary<LogLevel, MSLogLevel> LogLevelMap = new Dictionary<LogLevel, MSLogLevel>
                                                                                   {
                                                                                       { LogLevel.None, MSLogLevel.None },
                                                                                       { LogLevel.Debug, MSLogLevel.Debug },
                                                                                       { LogLevel.Trace, MSLogLevel.Trace },
                                                                                       { LogLevel.Info, MSLogLevel.Information },
                                                                                       { LogLevel.Warning, MSLogLevel.Warning },
                                                                                       { LogLevel.Fatal, MSLogLevel.Critical },
                                                                                       { LogLevel.Error, MSLogLevel.Error }
                                                                                   };

        private static ILoggerFactory _factory = null;

        public static void ConfigureLogger(LogOptions options, IEnumerable<LogMessage> initialLogMessages = null)
        {
            LoggerFactory.AddSerilog(new LoggerConfiguration().MinimumLevel.Is(options.LogLevel).Enrich.FromLogContext().WriteTo.Console().CreateLogger());

            if (options.LogToFile)
            {
                // log on the lowest level to the log file
                var logFilesPath = Path.Combine(options.OutputPath, "logs");
                LoggerFactory.AddFile(logFilesPath + "/log-{Date}.txt", MSLogLevel.Trace);
            }

            if (initialLogMessages != null)
            {
                var logger = LoggerFactory.CreateLogger("InitialLogging");

                foreach (var logMessage in initialLogMessages)
                {
                    logger.Log(logMessage.LogLevel, logMessage.EventId, logMessage.State, logMessage.Exception, logMessage.Formatter);
                }
            }

            // When stryker log level is debug or trace, set LibGit2Sharp loglevel to info
            if (options.LogLevel < LogEventLevel.Information)
            {
                var libGit2SharpLogger = LoggerFactory.CreateLogger(nameof(LibGit2Sharp));
                GlobalSettings.LogConfiguration = new LogConfiguration(LogLevel.Info, (level, message) => libGit2SharpLogger.Log(LogLevelMap[level], message));
            }
        }

        public static ILoggerFactory LoggerFactory
        {
            get => _factory ??= new LoggerFactory();
            set => _factory = value;
        }
    }
}
