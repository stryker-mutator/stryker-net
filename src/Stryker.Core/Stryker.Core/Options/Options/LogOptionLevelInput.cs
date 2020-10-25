using Serilog.Events;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
    public class LogOptionLevelInput : ComplexStrykerInput<string, LogEventLevel>
    {
        static LogOptionLevelInput()
        {
            HelpText = "Sets the console output logging level | Options [error, warning, info (default), debug, trace]";
            DefaultInput = "info";
            DefaultValue = new LogOptionLevelInput(DefaultInput).Value;
        }

        public override StrykerInput Type => StrykerInput.LogOptionLevelInput;

        public LogOptionLevelInput(string logLevel)
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
