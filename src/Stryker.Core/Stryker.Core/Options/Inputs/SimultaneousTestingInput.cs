namespace Stryker.Core.Options.Inputs
{
    class SimultaneousTestingInput : ComplexStrykerInput<bool, OptimizationFlags>
    {
        static SimultaneousTestingInput()
        {
            HelpText = @"Test each mutation in an isolated test run.";
            DefaultInput = false;
            DefaultValue = new SimultaneousTestingInput(AbortTestOnFailInput.DefaultValue, DefaultInput).Value;
        }

        public override StrykerInput Type => StrykerInput.DisableSimultaneousTesting;

        public SimultaneousTestingInput(OptimizationFlags optimizationFlag, bool disableSimultaneousTesting)
        {
            optimizationFlag |= disableSimultaneousTesting ? OptimizationFlags.DisableTestMix : OptimizationFlags.NoOptimization;
            Value = optimizationFlag;
        }
    }
}
