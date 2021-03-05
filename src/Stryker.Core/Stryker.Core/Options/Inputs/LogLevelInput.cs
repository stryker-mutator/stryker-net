using Serilog.Events;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class LogLevelInput : OptionDefinition<string, LogEventLevel>
    {
        public override string DefaultInput => "info";
        public override LogEventLevel DefaultValue => new LogLevelInput(DefaultInput).Value;

        protected override string Description => "The loglevel for output to the console.";
        protected override string HelpOptions => FormatEnumHelpOptions();

        public LogLevelInput() { }
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
