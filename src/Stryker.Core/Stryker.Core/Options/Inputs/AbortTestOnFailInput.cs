namespace Stryker.Core.Options.Inputs
{
    public class AbortTestOnFailInput : ComplexStrykerInput<bool, OptimizationModes>
    {
        static AbortTestOnFailInput()
        {
            Description = @"Abort unit testrun as soon as any one unit test fails. This can reduce the overall running time.";
            DefaultInput = true;
            DefaultValue = new AbortTestOnFailInput(DefaultInput, OptimizationsInput.DefaultValue).Value;
        }

        public override StrykerInput Type => StrykerInput.AbortTestOnFail;

        public AbortTestOnFailInput(bool abortTestOnFail, OptimizationModes optimizationModes)
        {
            optimizationModes |= abortTestOnFail ? OptimizationModes.DisableTestMix : OptimizationModes.NoOptimization;
            Value = optimizationModes;
        }
    }
}
