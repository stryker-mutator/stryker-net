namespace Stryker.Core.Options.Options
{
    class DiffEnabledOption : BaseStrykerOption<bool>
    {
        public DiffEnabledOption(bool diffEabled)
        {
            Value = diffEabled;
        }

        public override StrykerOption Type => StrykerOption.DiffEnabled;
        public override string HelpText => "";
        public override bool DefaultValue => false;
    }
}
