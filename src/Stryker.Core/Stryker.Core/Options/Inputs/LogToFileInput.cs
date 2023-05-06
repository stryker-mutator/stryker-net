using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs;

public class LogToFileInput : Input<bool?>
{
    public override bool? Default => false;

    protected override string Description => "Makes the logger write to a file. Logging to file always uses loglevel trace.";

    public LogToFileInput() { }

    public bool Validate(string outputPath)
    {
        if (SuppliedInput.HasValue)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                throw new InputException("Output path must be set if log to file is enabled");
            }

            return SuppliedInput.Value;
        }
        else
        {
            return false;
        }
    }
}
