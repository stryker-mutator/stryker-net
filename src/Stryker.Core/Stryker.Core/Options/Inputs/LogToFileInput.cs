using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class LogToFileInput : SimpleStrykerInput<bool>
    {
        static LogToFileInput()
        {
            Description = "Makes the logger write to a file (Logging to file always uses loglevel trace)";
            DefaultValue = false;
        }

        public override StrykerInput Type => StrykerInput.LogToFile;

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
