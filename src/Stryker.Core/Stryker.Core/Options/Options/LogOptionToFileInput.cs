using Serilog.Events;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;

namespace Stryker.Core.Options.Options
{
    public class LogOptionToFileInput : SimpleStrykerInput<bool>
    {
        static LogOptionToFileInput()
        {
            HelpText = "Sets the console output logging level";
            DefaultValue = false;
        }

        public override StrykerInput Type => StrykerInput.LogOptionToFileInput;

        public LogOptionToFileInput(bool? logToFile, string outputPath)
        {
            if (logToFile is { })
            {
                if (outputPath.IsNullOrEmptyInput())
                {
                    throw new StrykerInputException("Output path must be set if log to file is enabled");
                }

                Value = (bool)logToFile;
            }
        }
    }
}
