namespace Stryker.Core.Options.Inputs
{
    public class DisableAbortTestOnFailInput : InputDefinition<bool?, OptimizationModes>
    {
        public override bool? Default => false;

        protected override string Description => "Disable abort unit testrun as soon as any one unit test fails.";

        public OptimizationModes Validate()
        {
            if (SuppliedInput is { })
            {
                return SuppliedInput.Value ? OptimizationModes.DisableAbortTestOnKill : OptimizationModes.NoOptimization;
            }
            return OptimizationModes.NoOptimization;
        }
    }
}
