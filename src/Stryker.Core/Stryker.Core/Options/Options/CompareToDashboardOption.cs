namespace Stryker.Core.Options.Options
{
    class CompareToDashboardOption : BaseStrykerOption<bool>
    {
        public CompareToDashboardOption(bool compareToDashboard)
        {
            Value = compareToDashboard;
        }

        public override StrykerOption Type => StrykerOption.CompareToDashboard;
        public override string HelpText => "";
        public override bool DefaultValue => false;
    }
}
