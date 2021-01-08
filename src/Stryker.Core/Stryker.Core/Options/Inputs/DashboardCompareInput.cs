namespace Stryker.Core.Options.Inputs
{
    public class DashboardCompareInput : OptionDefinition<bool>
    {
        public override StrykerOption Type => StrykerOption.DashboardCompare;
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
