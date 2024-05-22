using System.IO;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using LibGitLogLevel = LibGit2Sharp.LogLevel;
using MSLogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Stryker.CLI.Logging
{
    public static class ApplicationLogging
    {
        private static ILoggerFactory factory;

        public static void ConfigureLogger(LogEventLevel logLevel, bool logToFile, bool traceToFile, string outputPath)
        {
            LoggerFactory.AddSerilog(new LoggerConfiguration().MinimumLevel.Is(logLevel).Enrich.FromLogContext().WriteTo.Console().CreateLogger());

            if (logToFile)
            {
                // log on the lowest level to the log file
                var logFilesPath = Path.Combine(outputPath, "logs");
                LoggerFactory.AddFile(logFilesPath + "/log-{Date}.txt", traceToFile ? MSLogLevel.Trace : MSLogLevel.Debug);
            }

            // When stryker log level is debug or trace, set LibGit2Sharp loglevel
            if (logLevel < LogEventLevel.Information) // LibGit2Sharp does not handle LogEventLevel.None properly.
            {
                var libGit2SharpLogger = LoggerFactory.CreateLogger(nameof(LibGit2Sharp));
                GlobalSettings.LogConfiguration = new LogConfiguration(LogLevelConverter.Convert(logLevel), (level, message) => libGit2SharpLogger.Log(LogLevelConverter.Convert(level), message));
            }
        }

        public static ILoggerFactory LoggerFactory
        {
            get => factory ??= new LoggerFactory();
            set => factory = value;
        }

        private static class LogLevelConverter
        {
            public static LibGitLogLevel Convert(LogEventLevel level) => level switch
            {
                LogEventLevel.Information => LibGitLogLevel.None,
                LogEventLevel.Debug => LibGitLogLevel.Debug,
                LogEventLevel.Verbose => LibGitLogLevel.Trace,
                LogEventLevel.Warning => LibGitLogLevel.Warning,
                LogEventLevel.Error => LibGitLogLevel.Error,
                _ => LibGitLogLevel.None
            };

            public static MSLogLevel Convert(LibGitLogLevel level) => level switch
            {
                LibGitLogLevel.None => MSLogLevel.None,
                LibGitLogLevel.Trace => MSLogLevel.Trace,
                LibGitLogLevel.Debug => MSLogLevel.Debug,
                LibGitLogLevel.Info => MSLogLevel.Information,
                LibGitLogLevel.Warning => MSLogLevel.Warning,
                LibGitLogLevel.Error => MSLogLevel.Error,
                LibGitLogLevel.Fatal => MSLogLevel.Critical,
                _ => MSLogLevel.None
            };
        }
    }
}
