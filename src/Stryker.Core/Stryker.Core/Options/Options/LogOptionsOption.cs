using Serilog.Events;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using System;
using System.Collections.Generic;

namespace Stryker.Core.Options.Options
{
    public class LogOptionsOption : BaseStrykerOption<LogOptions>
    {
        readonly LogEventLevel _logEventLevel;

        public LogOptionsOption(string logLevel, bool logToFile, string outputPath)
        {
            switch (logLevel?.ToLower() ?? "")
            {
                case "error":
                    _logEventLevel = LogEventLevel.Error;
                    break;
                case "warning":
                    _logEventLevel = LogEventLevel.Warning;
                    break;
                case "info":
                case "":
                    _logEventLevel = LogEventLevel.Information;
                    break;
                case "debug":
                    _logEventLevel = LogEventLevel.Debug;
                    break;
                case "trace":
                    _logEventLevel = LogEventLevel.Verbose;
                    break;
                default:
                    throw new StrykerInputException(
                        ErrorMessage,
                        $"Incorrect log level ({logLevel}). The log level options are [{string.Join(", ", (IEnumerable<LogEventLevel>)Enum.GetValues(typeof(LogEventLevel)))}]");
            }
            Value = new LogOptions(_logEventLevel, logToFile, outputPath);
        }

        public override StrykerOption Type => StrykerOption.LogOptions;
        public override string HelpText => "Sets the console output logging level";
        public override LogOptions DefaultValue => new LogOptions(LogEventLevel.Information, false, "");
    }
}
