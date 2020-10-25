namespace Stryker.Core.Options.Inputs
{
    class DisableSimultaneousTestingInput : ComplexStrykerInput<bool, OptimizationModes>
    {
        static DisableSimultaneousTestingInput()
        {
            HelpText = @"Test each mutation in an isolated test run.";
            DefaultInput = false;
            DefaultValue = new DisableSimultaneousTestingInput(AbortTestOnFailInput.DefaultValue, DefaultInput).Value;
        }

        public override StrykerInput Type => StrykerInput.DisableSimultaneousTesting;

        public DisableSimultaneousTestingInput(OptimizationModes optimizationFlag, bool disableSimultaneousTesting)
        {
            optimizationFlag |= disableSimultaneousTesting ? OptimizationModes.DisableTestMix : OptimizationModes.NoOptimization;
            Value = optimizationFlag;
        }
    }
}
