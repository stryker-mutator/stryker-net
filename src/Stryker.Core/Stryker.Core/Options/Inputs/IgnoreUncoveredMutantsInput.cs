namespace Stryker.Core.Options.Inputs
{
    public class IgnoreUncoveredMutantsInput : Input<bool?>
    {
        public override bool? Default => false;

        protected override string Description => "Instruct Stryker to ignore all No Coverage mutants, so that the mutation score reflects the quality of existing tests.";

        public bool Validate()
        {
            return SuppliedInput == true;
        }
    }
}
