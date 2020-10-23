using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
    class OptimizationsOption : BaseStrykerOption<OptimizationFlags>
    {
        public OptimizationsOption(string coverageAnalysis, bool abortTestOnFail, bool disableSimultaneousTesting)
        {
            OptimizationFlags value;
            switch (coverageAnalysis.ToLower())
            {
                case "pertestinisolation":
                    value = OptimizationFlags.CoverageBasedTest | OptimizationFlags.CaptureCoveragePerTest;
                    break;
                case "pertest":
                    value = OptimizationFlags.CoverageBasedTest;
                    break;
                case "all":
                    value = OptimizationFlags.SkipUncoveredMutants;
                    break;
                case "off":
                case "":
                    value = OptimizationFlags.NoOptimization;
                    break;
                default:
                    throw new StrykerInputException(
                        ErrorMessage,
                        $"Incorrect coverageAnalysis option ({coverageAnalysis}). The options are [Off, All, PerTest or PerTestInIsolation].");
            }
            Value = value | (abortTestOnFail ? OptimizationFlags.AbortTestOnKill : 0) | (disableSimultaneousTesting ? OptimizationFlags.DisableTestMix : 0);
        }

        public override StrykerOption Type => StrykerOption.Optimizations;
        public override string HelpText => @"Sets the OptimizationFlags depending on the 'Optimalization Mode', 'Abort Test On Fail' and 'Disable Testing Mix'";
        public override OptimizationFlags DefaultValue => OptimizationFlags.CoverageBasedTest | (true ? OptimizationFlags.AbortTestOnKill : 0) | (false ? OptimizationFlags.DisableTestMix : 0);
    }
}
