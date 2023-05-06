using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs;

public class SinceTargetInput : Input<string>
{
    public override string Default => "master";
    protected override string Description => "The target branch/commit to compare with the current codebase when the since feature is enabled.";

    public string Validate(bool sinceEnabled)
    {
        if (sinceEnabled && SuppliedInput is not null)
        {
            if (string.IsNullOrWhiteSpace(SuppliedInput))
            {
                throw new InputException("The since target cannot be empty when the since feature is enabled");
            }

            return SuppliedInput;
        }
        return Default;
    }
}
