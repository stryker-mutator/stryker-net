namespace Stryker.Core.Options.Inputs
{
    public class DisableSimultaneousTestingInput : OptionDefinition<bool, OptimizationModes>
    {
        public override bool DefaultInput => false;
        public override OptimizationModes DefaultValue => new DisableSimultaneousTestingInput(DefaultInput).Value;

        protected override string Description => "Test each mutation in an isolated test run.";

        public DisableSimultaneousTestingInput() { }
        public DisableSimultaneousTestingInput(bool? disableSimultaneousTesting)
        {
            if (disableSimultaneousTesting is { })
            {
                Value = disableSimultaneousTesting.Value ? OptimizationModes.DisableTestMix : OptimizationModes.NoOptimization;
            }
        }
    }
}
