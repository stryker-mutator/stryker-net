using Serilog.Events;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class LogLevelInput : ComplexStrykerInput<string, LogEventLevel>
    {
        static LogLevelInput()
        {
            HelpText = "Sets the console output logging level | Options [error, warning, info (default), debug, trace]";
            DefaultInput = "info";
            DefaultValue = new LogLevelInput(DefaultInput).Value;
        }

        public override StrykerInput Type => StrykerInput.LogLevel;

        public LogLevelInput(string logLevel)
        {
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

                Value = logEventLevel;
            }
        }
    }
}
