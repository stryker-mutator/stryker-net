namespace Stryker.Core.Options.Options
{
    public class ProjectNameOption : BaseStrykerOption<string>
    {
        public override StrykerOption Type => StrykerOption.ProjectName;

        public override string HelpText => "The organizational name for your project. Required when dashboard reporter is turned on. Often the name of your solution.";

        public override string DefaultValue => null;
    }
}
