using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
    public class ModuleNameOption : BaseStrykerOption<string>
    {
        public ModuleNameOption(string moduleName)
        {
            if (moduleName is { })
            {
                if (moduleName.IsEmptyInput())
                {
                    throw new StrykerInputException("Module name cannot be empty. Either fill the option or leave it out.");
                }
                Value = moduleName;
            }
        }

        public override StrykerOption Type => StrykerOption.ModuleName;

        public override string HelpText => "Module name used by reporters. Usually a project in your solution would be a module.";
    }
}
