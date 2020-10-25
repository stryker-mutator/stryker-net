using Stryker.Core.Exceptions;
using System.Linq;

namespace Stryker.Core.Options.Inputs
{
    public class OptimizationModeInput : SimpleStrykerInput<string>
    {
        static OptimizationModeInput()
        {
            HelpText = @"Use coverage info to speed up execution. Possible values are: off, all, perTest, perIsolatedTest.
    - off: coverage data is not captured.
    - perTest (Default): capture the list of mutations covered by each test. For every mutation that has tests, only the tests that cover this mutation are tested. Fastest option.
    - all: capture the list of mutations covered by each test. Test only these mutations. Fast option.
    - perTestInIsolation: like 'perTest', but running each test in an isolated run. Slowest fast option.";
            DefaultValue = "perTest";
        }

        public override StrykerInput Type => StrykerInput.OptimizationMode;

        public OptimizationModeInput(string coverageAnalysis)
        {
            if (!new[] { "off", "all", "pertest", "pertestinisolation" }.Contains(coverageAnalysis))
            {
                throw new StrykerInputException($"Incorrect coverageAnalysis option ({coverageAnalysis}).");
            }

            Value = coverageAnalysis is { } ? coverageAnalysis : DefaultValue;
        }
    }
}
