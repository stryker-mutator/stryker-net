namespace Stryker.Core.Options.Options
{
    public class AbortTestOnFailInput : ComplexStrykerInput<bool, OptimizationFlags>
    {
        static AbortTestOnFailInput()
        {
            HelpText = @"Abort unit testrun as soon as any one unit test fails. This can reduce the overall running time.";
            DefaultInput = true;
            DefaultValue = new AbortTestOnFailInput(OptimizationsInput.DefaultValue, DefaultInput).Value;
        }

        public override StrykerInput Type => StrykerInput.AbortTestOnFail;

        public AbortTestOnFailInput(OptimizationFlags optimizationFlag, bool abortTestOnFail)
        {
            optimizationFlag |= abortTestOnFail ? OptimizationFlags.AbortTestOnKill : OptimizationFlags.NoOptimization;
            Value = optimizationFlag;
        }
    }
}
