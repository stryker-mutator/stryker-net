using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class LogToFileInput : OptionDefinition<bool>
    {
        public override bool DefaultValue => false;

        protected override string Description => "Makes the logger write to a file. Logging to file always uses loglevel trace.";

        public LogToFileInput() { }
        public LogToFileInput(bool? logToFile, string outputPath)
        {
            if (logToFile is { })
            {
                if (outputPath.IsNullOrEmptyInput())
                {
                    throw new StrykerInputException("Output path must be set if log to file is enabled");
                }

                Value = logToFile.Value;
            }
        }
    }
}
