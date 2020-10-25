using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class OptimizationsInput : SimpleStrykerInput<OptimizationModes>
    {
        static OptimizationsInput()
        {
            Description = @"Sets the OptimizationFlags depending on the 'Optimization Mode', 'Abort Test On Fail' and 'Disable Testing Mix'";
            DefaultValue = OptimizationModes.CoverageBasedTest | OptimizationModes.AbortTestOnKill | 0;
        }

        public override StrykerInput Type => StrykerInput.Optimizations;

        public OptimizationsInput(string coverageAnalysis)
        {
            var optimization = coverageAnalysis.ToLower() switch
            {
                "pertestinisolation" => OptimizationModes.CoverageBasedTest | OptimizationModes.CaptureCoveragePerTest,
                "pertest" => OptimizationModes.CoverageBasedTest,
                "all" => OptimizationModes.SkipUncoveredMutants,
                "off" => OptimizationModes.NoOptimization,
                _ => throw new StrykerInputException($"Incorrect coverageAnalysis option ({coverageAnalysis}). The options are [Off, All, PerTest or PerTestInIsolation].")
            };
            Value = optimization;
        }
    }
}
