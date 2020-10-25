namespace Stryker.Core.Options.Inputs
{
    public class CompareToDashboardInput : SimpleStrykerInput<bool>
    {
        static CompareToDashboardInput()
        {
            Description = $@"EXPERIMENTAL: Enables comparing to results stored in Stryker Dashboard. This feature is only available in combination with --diff";
        }

        public override StrykerInput Type => StrykerInput.DashboardCompareEnabled;

        public CompareToDashboardInput(bool? compareToDashboard)
        {
            Value = compareToDashboard ?? DefaultValue;
        }
    }
}
