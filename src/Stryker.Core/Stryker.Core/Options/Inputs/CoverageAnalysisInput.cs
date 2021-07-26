using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class CoverageAnalysisInput : InputDefinition<string>
    {
        public override string Default => "perTest";

        protected override string Description => @"Use coverage info to speed up execution. Possible values are: off, all, perTest, perIsolatedTest.
    - off: coverage data is not captured.
    - perTest (Default): capture the list of mutations covered by each test. For every mutation that has tests, only the tests that cover this mutation are tested. Fastest option.
    - all: capture the list of mutations covered by each test. Test only these mutations. Fast option.
    - perTestInIsolation: like 'perTest', but running each test in an isolated run. Slowest fast option.";

        public OptimizationModes Validate()
        {
            if (SuppliedInput is { })
            {
                var optimization = SuppliedInput.ToLower() switch
                {
                    "pertestinisolation" => OptimizationModes.CoverageBasedTest | OptimizationModes.CaptureCoveragePerTest,
                    "pertest" => OptimizationModes.CoverageBasedTest,
                    "all" => OptimizationModes.SkipUncoveredMutants,
                    "off" => OptimizationModes.None,
                    _ => throw new InputException($"Incorrect coverageAnalysis option ({SuppliedInput}). The options are [Off, All, PerTest or PerTestInIsolation].")
                };

                return optimization;
            }
            return OptimizationModes.CaptureCoveragePerTest;
        }
    }
}
