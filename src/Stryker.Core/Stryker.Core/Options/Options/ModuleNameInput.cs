using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
    public class ModuleNameInput : SimpleStrykerInput<string>
    {
        static ModuleNameInput()
        {
            HelpText = "Module name used by reporters. Usually a project in your solution would be a module.";
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
