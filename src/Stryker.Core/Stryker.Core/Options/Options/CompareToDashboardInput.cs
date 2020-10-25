namespace Stryker.Core.Options.Options
{
    public class CompareToDashboardInput : SimpleStrykerInput<bool>
    {
        static CompareToDashboardInput()
        {
            HelpText = $@"EXPERIMENTAL: Enables comparing to results stored in Stryker Dashboard. This feature is only available in combination with --diff";
        }

        public override StrykerInput Type => StrykerInput.CompareToDashboard;

        public CompareToDashboardInput(bool? compareToDashboard)
        {
            Value = compareToDashboard ?? DefaultValue;
        }
    }
}
