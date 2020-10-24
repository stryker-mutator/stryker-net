namespace Stryker.Core.Options.Options
{
    public class CompareToDashboardOption : BaseStrykerOption<bool>
    {
        static CompareToDashboardOption()
        {
        HelpText = "EXPERIMENTAL: Enables comparing to results stored in Stryker Dashboard. This feature is only available in combination with Diff Enabled";
        }

        public override StrykerOption Type => StrykerOption.CompareToDashboard;

        public CompareToDashboardOption(bool? compareToDashboard)
        {
            Value = compareToDashboard ?? DefaultValue;
        }
    }
}
