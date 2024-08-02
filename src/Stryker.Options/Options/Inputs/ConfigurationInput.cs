
using Stryker.Configuration.Exceptions;

namespace Stryker.Configuration.Options.Inputs;
public class ConfigurationInput : Input<string>
{
    public override string Default => null;
    protected override string Description => "Configuration to use when building the project(s).";

    public string Validate()
    {
        if (SuppliedInput is null)
        {
            return Default;
        }
        if (string.IsNullOrWhiteSpace(SuppliedInput))
        {
            throw new InputException("Please provide a non whitespace only configuration.");
        }
        return SuppliedInput; 
    }
}
