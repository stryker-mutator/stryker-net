using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class OptimizationsInput : SimpleStrykerInput<OptimizationFlags>
    {
        static OptimizationsInput()
        {
            HelpText = @"Sets the OptimizationFlags depending on the 'Optimization Mode', 'Abort Test On Fail' and 'Disable Testing Mix'";
            DefaultValue = OptimizationFlags.CoverageBasedTest | OptimizationFlags.AbortTestOnKill | 0;
        }

        public override StrykerInput Type => StrykerInput.Optimizations;

        public OptimizationsInput(string coverageAnalysis)
        {
            var optimization = coverageAnalysis.ToLower() switch
            {
                "pertestinisolation" => OptimizationFlags.CoverageBasedTest | OptimizationFlags.CaptureCoveragePerTest,
                "pertest" => OptimizationFlags.CoverageBasedTest,
                "all" => OptimizationFlags.SkipUncoveredMutants,
                "off" => OptimizationFlags.NoOptimization,
                _ => throw new StrykerInputException($"Incorrect coverageAnalysis option ({coverageAnalysis}). The options are [Off, All, PerTest or PerTestInIsolation].")
            };
            Value = optimization;
        }
    }
}
