using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs;

public class TargetFrameworkInput : Input<string>
{
    public override string Default => null;

    protected override string Description => "The framework to build the project against.";

    public string Validate()
    {
        if (SuppliedInput is not null)
        {
            if (string.IsNullOrWhiteSpace(SuppliedInput))
            {
                throw new InputException("Target framework cannot be empty. Please provide a valid value from this list: https://docs.microsoft.com/en-us/dotnet/standard/frameworks");
            }

            return SuppliedInput;
        }

        return Default;
    }
}
