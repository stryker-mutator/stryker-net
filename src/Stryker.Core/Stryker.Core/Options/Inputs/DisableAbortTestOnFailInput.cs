namespace Stryker.Core.Options.Inputs
{
    public class DisableAbortTestOnFailInput : ComplexStrykerInput<bool, OptimizationModes>
    {
        public override StrykerInput Type => StrykerInput.DisableAbortTestOnFail;
        public override bool DefaultInput => false;
        public override OptimizationModes DefaultValue => new DisableAbortTestOnFailInput(DefaultInput).Value;

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
