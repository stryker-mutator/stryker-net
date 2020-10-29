namespace Stryker.Core.Options.Inputs
{
    public class DashboardCompareInput : SimpleStrykerInput<bool>
    {
        public override StrykerInput Type => StrykerInput.DashboardCompare;
        public override bool DefaultValue => false;

        protected override string Description => "EXPERIMENTAL: Use results stored in stryker dashboard to only test new mutants.";

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
