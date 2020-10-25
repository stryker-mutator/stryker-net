namespace Stryker.Core.Options.Inputs
{
    class DisableSimultaneousTestingInput : ComplexStrykerInput<bool, OptimizationModes>
    {
        static DisableSimultaneousTestingInput()
        {
            HelpText = @"Test each mutation in an isolated test run.";
            DefaultInput = false;
            DefaultValue = new DisableSimultaneousTestingInput(DefaultInput, OptimizationsInput.DefaultValue).Value;
        }

        public override StrykerInput Type => StrykerInput.DisableSimultaneousTesting;

        public DisableSimultaneousTestingInput(bool disableSimultaneousTesting, OptimizationModes optimizationModes)
        {
            optimizationModes |= disableSimultaneousTesting ? OptimizationModes.DisableTestMix : OptimizationModes.NoOptimization;
            Value = optimizationModes;
        }
    }
}
