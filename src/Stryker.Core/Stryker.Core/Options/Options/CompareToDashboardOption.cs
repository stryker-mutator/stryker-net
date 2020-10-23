namespace Stryker.Core.Options.Options
{
    public class CompareToDashboardOption : BaseStrykerOption<bool>
    {
        public CompareToDashboardOption(bool? compareToDashboard)
        {
            Value = compareToDashboard ?? DefaultValue;
        }

        public override StrykerOption Type => StrykerOption.CompareToDashboard;
        public override string HelpText => "EXPERIMENTAL: Enables comparing to results stored in Stryker Dashboard. This feature is only available in combination with Diff Enabled";
    }
}
