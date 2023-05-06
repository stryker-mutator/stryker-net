using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs;

public class ModuleNameInput : Input<string>
{
    protected override string Description => "Module name used by reporters. Usually a project in your solution would be a module.";

    public override string Default => string.Empty;

    public string Validate()
    {
        if (SuppliedInput is not null)
        {
            if (string.IsNullOrWhiteSpace(SuppliedInput))
            {
                throw new InputException("Module name cannot be empty. Either fill the option or leave it out.");
            }
            return SuppliedInput;
        }
        return Default;
    }
}
