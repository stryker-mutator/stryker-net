namespace Stryker.Core.Options.Inputs
{
    public class DisableAbortTestOnFailInput : InputDefinition<bool, OptimizationModes>
    {
        public override bool DefaultInput => false;
        public override OptimizationModes Default => new DisableAbortTestOnFailInput(DefaultInput).Value;

        protected override string Description => "Disable abort unit testrun as soon as any one unit test fails.";

        public DisableAbortTestOnFailInput() { }
        public DisableAbortTestOnFailInput(bool? disableAbortTestOnFail)
        {
            if (disableAbortTestOnFail is { })
            {
                Value = disableAbortTestOnFail.Value ? OptimizationModes.DisableAbortTestOnKill : OptimizationModes.NoOptimization;
            }
        }
    }
}
