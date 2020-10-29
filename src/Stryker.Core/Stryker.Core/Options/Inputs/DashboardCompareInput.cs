namespace Stryker.Core.Options.Inputs
{
    public class DashboardCompareInput : SimpleStrykerInput<bool>
    {
        public override StrykerInput Type => StrykerInput.DashboardCompare;
        public override bool DefaultValue => false;

        protected override string Description => "EXPERIMENTAL: Enables comparing to results stored in Stryker Dashboard. This feature is only available in combination with --diff";

        public DashboardCompareInput() { }
        public DashboardCompareInput(bool? compareToDashboard)
        {
            if (compareToDashboard is { })
            {
                Value = compareToDashboard.Value;
            }
        }
    }
}
