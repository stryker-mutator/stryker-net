using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class ModuleNameInput : SimpleStrykerInput<string>
    {
        static ModuleNameInput()
        {
            Description = $"Module name used in reporters when project consists of multiple modules. See project-name for examples.";
        }

        public override StrykerInput Type => StrykerInput.ModuleName;

        public ModuleNameInput(string moduleName)
        {
            if (moduleName is { })
            {
                if (moduleName.IsNullOrEmptyInput())
                {
                    throw new StrykerInputException("Module name cannot be empty. Either fill the option or leave it out.");
                }
                Value = moduleName;
            }
        }
    }
}
