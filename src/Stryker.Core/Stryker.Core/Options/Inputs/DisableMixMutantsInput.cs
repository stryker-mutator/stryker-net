namespace Stryker.Core.Options.Inputs
{
    public class DisableMixMutantsInput : InputDefinition<bool?, OptimizationModes>
    {
        public override bool? Default => false;

        protected override string Description => "Test each mutation in an isolated test run.";

        public OptimizationModes Validate()
        {
            if (SuppliedInput is { })
            {
                return SuppliedInput.Value ? OptimizationModes.DisableMixMutants : OptimizationModes.NoOptimization;
            }
            return OptimizationModes.NoOptimization;
        }
    }
}
