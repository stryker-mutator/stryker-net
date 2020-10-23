using Serilog.Events;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;

namespace Stryker.Core.Options.Options
{
    public class LogOptionsOption : BaseStrykerOption<LogOptions>
    {
        public LogOptionsOption(string logLevel, bool? logToFile, string outputPath)
        {
            if (logToFile is { })
            {
                if (outputPath.IsNullOrEmptyInput())
                {
                    throw new StrykerInputException("Output path must be set if log to file is enabled");
                }

                Value = new LogOptions(Value.LogLevel, logToFile.Value, outputPath);
            }

            if (logLevel is { })
            {
                var logEventLevel = logLevel.ToLower() switch
                {
                    "error" => LogEventLevel.Error,
                    "warning" => LogEventLevel.Warning,
                    "info" => LogEventLevel.Information,
                    "debug" => LogEventLevel.Debug,
                    "trace" => LogEventLevel.Verbose,
                    _ => throw new StrykerInputException($"Incorrect log level ({logLevel}).")
                };

                Value = new LogOptions(logEventLevel, Value.LogToFile, Value.OutputPath);
            }
        }

        public override StrykerOption Type => StrykerOption.LogOptions;
        public override string HelpText => "Sets the console output logging level";
        public override LogOptions DefaultValue => new LogOptions(LogEventLevel.Information, logToFile: false, null);
    }
}
