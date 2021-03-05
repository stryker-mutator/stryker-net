using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class OptimizationModeInput : InputDefinition<string, OptimizationModes>
    {
        public override string DefaultInput => "perTest";
        public override OptimizationModes Default => new OptimizationModeInput(DefaultInput).Value;

        protected override string Description => @"Use coverage info to speed up execution. Possible values are: off, all, perTest, perIsolatedTest.
    - off: coverage data is not captured.
    - perTest (Default): capture the list of mutations covered by each test. For every mutation that has tests, only the tests that cover this mutation are tested. Fastest option.
    - all: capture the list of mutations covered by each test. Test only these mutations. Fast option.
    - perTestInIsolation: like 'perTest', but running each test in an isolated run. Slowest fast option.";

        public OptimizationModeInput() { }
        public OptimizationModeInput(string coverageAnalysis)
        {
            if (coverageAnalysis is { })
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
}
