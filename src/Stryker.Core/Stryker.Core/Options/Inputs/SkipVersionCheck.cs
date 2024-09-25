namespace Stryker.Core.Options.Inputs
{
    public class SkipVersionCheck : Input<bool?>
    {
        public override bool? Default => false;

        protected override string Description => @"Skips check for newer version.";

        public bool Validate()
        {
            return SuppliedInput ?? false;
        }
    }
}
