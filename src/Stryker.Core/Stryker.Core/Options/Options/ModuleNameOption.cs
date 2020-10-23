namespace Stryker.Core.Options.Options
{
    public class ModuleNameOption : BaseStrykerOption<string>
    {
        public override StrykerOption Type => StrykerOption.ModuleName;

        public override string HelpText => "Module name used by reporters. Usually a project in your solution would be a module.";

        public override string DefaultValue => null;
    }
}
