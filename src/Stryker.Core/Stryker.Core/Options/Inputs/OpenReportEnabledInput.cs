namespace Stryker.Core.Options.Inputs
{
    public class OpenReportEnabledInput : Input<bool>
    {
        public override bool Default => false;

        protected override string Description => "When enabled the report will open automatically after stryker has generated the report.";

        public bool Validate() => SuppliedInput;
    }
}
