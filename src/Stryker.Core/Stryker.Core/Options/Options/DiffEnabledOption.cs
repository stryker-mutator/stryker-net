namespace Stryker.Core.Options.Options
{
    class DiffEnabledOption : BaseStrykerOption<bool>
    {
        public DiffEnabledOption(bool diffEabled)
        {
            Value = diffEabled;
        }

        public override StrykerOption Type => StrykerOption.DiffEnabled;
        public override string HelpText => "Enables the diff feature. It makes sure to only mutate changed files. Gets the diff from git by default";
        public override bool DefaultValue => false;
    }
}
