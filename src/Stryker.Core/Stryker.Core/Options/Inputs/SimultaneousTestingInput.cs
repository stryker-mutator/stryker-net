namespace Stryker.Core.Options.Inputs
{
    class SimultaneousTestingInput : ComplexStrykerInput<bool, OptimizationModes>
    {
        static SimultaneousTestingInput()
        {
            HelpText = @"Test each mutation in an isolated test run.";
            DefaultInput = false;
            DefaultValue = new SimultaneousTestingInput(AbortTestOnFailInput.DefaultValue, DefaultInput).Value;
        }

        public override StrykerInput Type => StrykerInput.DisableSimultaneousTesting;

        public SimultaneousTestingInput(OptimizationModes optimizationFlag, bool disableSimultaneousTesting)
        {
            optimizationFlag |= disableSimultaneousTesting ? OptimizationModes.DisableTestMix : OptimizationModes.NoOptimization;
            Value = optimizationFlag;
        }
    }
}
