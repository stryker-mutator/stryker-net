
using Stryker.Abstractions.Exceptions;

namespace Stryker.Abstractions.Options.Inputs;
public class ConfigurationInput : Input<string>
{
    public override string Default => null;
    protected override string Description => "Configuration to use when building the project(s) (e.g., 'Debug' or 'Release'). If not specified, the default configuration of the project(s) will be used.";

    public string Validate()
    {
        if (SuppliedInput is null)
        {
            return Default;
        }

        return SuppliedInput;
    }
}
