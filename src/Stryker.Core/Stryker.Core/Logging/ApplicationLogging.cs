using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System.Collections.Generic;
using System.IO;
using LibGitLogLevel = LibGit2Sharp.LogLevel;
using MSLogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Stryker.Core.Logging
{
    public static class ApplicationLogging
    {
        private static readonly Dictionary<LibGitLogLevel, MSLogLevel> LogLevelMap = new Dictionary<LibGitLogLevel, MSLogLevel>
                                                                                   {
                                                                                       { LibGitLogLevel.None, MSLogLevel.None },
                                                                                       { LibGitLogLevel.Debug, MSLogLevel.Debug },
                                                                                       { LibGitLogLevel.Trace, MSLogLevel.Trace },
                                                                                       { LibGitLogLevel.Info, MSLogLevel.Information },
                                                                                       { LibGitLogLevel.Warning, MSLogLevel.Warning },
                                                                                       { LibGitLogLevel.Fatal, MSLogLevel.Critical },
                                                                                       { LibGitLogLevel.Error, MSLogLevel.Error }
                                                                                   };

        private static ILoggerFactory _factory = null;

        public static void ConfigureLogger(LogOptions options, string outputPath, IEnumerable<LogMessage> initialLogMessages = null)
        {
            LoggerFactory.AddSerilog(new LoggerConfiguration().MinimumLevel.Is(options.LogLevel).Enrich.FromLogContext().WriteTo.Console().CreateLogger());

            if (options.LogToFile)
            {
                // log on the lowest level to the log file
                var logFilesPath = Path.Combine(outputPath, "logs");
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
                GlobalSettings.LogConfiguration = new LogConfiguration(LibGitLogLevel.Info, (level, message) => libGit2SharpLogger.Log(LogLevelMap[level], message));
            }
        }

        public static ILoggerFactory LoggerFactory
        {
            get => _factory ??= new LoggerFactory();
            set => _factory = value;
        }
    }
}
