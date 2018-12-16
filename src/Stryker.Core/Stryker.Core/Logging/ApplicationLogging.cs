using Microsoft.Extensions.Logging;
using Serilog;

namespace Stryker.Core.Logging
{
    public static class ApplicationLogging
    {
        private static ILoggerFactory _factory = null;

        public static void ConfigureLogger(LogOptions options)
        {
            LoggerFactory.AddSerilog(new LoggerConfiguration()
                .MinimumLevel.Is(options.LogLevel)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger());

            if(options.LogToFile ?? false)
            {
                // log on the lowest level to the log file
                LoggerFactory.AddFile("StrykerLogs/log-{Date}.txt", LogLevel.Trace);
            }
        }

        public static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_factory == null)
                {
                    _factory = new LoggerFactory();
                }
                return _factory;
            }
            set { _factory = value; }
        }
    }
}
