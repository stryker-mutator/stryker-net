namespace Stryker.Abstractions.Options.Inputs
{
    public class OpenReportEnabledInput : Input<bool>
    {
        public override bool Default => false;

        protected override string Description => "";

        public bool Validate() => SuppliedInput;
    }
}
