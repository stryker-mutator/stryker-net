using Stryker.Abstractions.Exceptions;

namespace Stryker.Configuration.Options.Inputs;

public class BaselineTargetInput : Input<string>
{
    public override string Default => "master";

    protected override string Description => "The target branch/commit to compare with the current codebase when the baseline feature is enabled.";

    public BaselineTargetInput() { }

    public string Validate(bool sinceEnabled)
    {
        if (sinceEnabled)
        {
            if (SuppliedInput is null)
            {
                return Default;
            }

            if (string.IsNullOrEmpty(SuppliedInput))
            {
                throw new InputException("The baseline target cannot be empty when the baseline feature is enabled");
            }

            return SuppliedInput;
        }

        return Default;
    }
}