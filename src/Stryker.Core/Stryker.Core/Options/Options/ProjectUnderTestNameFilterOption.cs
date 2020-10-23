namespace Stryker.Core.Options.Options
{
    class ProjectUnderTestNameFilterOption : BaseStrykerOption<string>
    {
        public ProjectUnderTestNameFilterOption(string projectUnderTestNameFilter)
        {
            Value = projectUnderTestNameFilter;
        }

        public override StrykerOption Type => StrykerOption.ProjectUnderTestNameFilter;
        public override string HelpText => "";
        public override string DefaultValue => "";
    }
}
