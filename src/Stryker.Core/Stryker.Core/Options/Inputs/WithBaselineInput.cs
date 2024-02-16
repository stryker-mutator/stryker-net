namespace Stryker.Core.Options.Inputs
{
    public class BaselineEnabledInput : Input<bool?>
    {
        public override bool? Default => false;

        protected override string Description => "EXPERIMENTAL: Use results stored in stryker dashboard to only test new mutants.";

        public BaselineEnabledInput() { }

        public bool Validate()
        {
            return SuppliedInput ?? Default.Value;
        }
    }
}
