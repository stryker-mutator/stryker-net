using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
    public class OptimizationsOption : BaseStrykerOption<OptimizationFlags>
    {
        public OptimizationsOption(string coverageAnalysis, bool abortTestOnFail, bool disableSimultaneousTesting)
        {
            var optimization = coverageAnalysis.ToLower() switch
            {
                "pertestinisolation" => OptimizationFlags.CoverageBasedTest | OptimizationFlags.CaptureCoveragePerTest,
                "pertest" => OptimizationFlags.CoverageBasedTest,
                "all" => OptimizationFlags.SkipUncoveredMutants,
                "off" => OptimizationFlags.NoOptimization,
                _ => throw new StrykerInputException($"Incorrect coverageAnalysis option ({coverageAnalysis}). The options are [Off, All, PerTest or PerTestInIsolation].")
            };

            optimization |= abortTestOnFail ? OptimizationFlags.AbortTestOnKill : OptimizationFlags.NoOptimization;
            optimization |= disableSimultaneousTesting ? OptimizationFlags.DisableTestMix : OptimizationFlags.NoOptimization;

            Value = optimization;
        }

        public override StrykerOption Type => StrykerOption.Optimizations;
        public override string HelpText => @"Sets the OptimizationFlags depending on the 'Optimization Mode', 'Abort Test On Fail' and 'Disable Testing Mix'";
        public override OptimizationFlags DefaultValue => OptimizationFlags.CoverageBasedTest | OptimizationFlags.AbortTestOnKill;
    }
}
