using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    [TestClass]
    public class OptimizationModeInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new CoverageAnalysisInput();
            target.HelpText.ShouldBe(@"Use coverage info to speed up execution. Possible values are: off, perTest, all, perTestInIsolation.
	- off: Coverage data is not captured. Every mutant is tested against all test. Slowest, use in case of doubt.
	- perTest: Capture mutations covered by each test. Mutations are tested against covering tests (or flagged NoCoverage if no test cover them). Fastest option.
	- all: Capture the list of mutations covered by some test. Test only these mutations, other are flagged as NoCoverage. Fast option.
	- perTestInIsolation: 'perTest' but coverage of each test is captured in isolation. Increase coverage accuracy at the expense of a slow init phase.
 | default: 'perTest'");
        }

        [TestMethod]
        [DataRow(null, OptimizationModes.CoverageBasedTest)]
        [DataRow("perTestinisolation", OptimizationModes.CoverageBasedTest, OptimizationModes.CaptureCoveragePerTest)]
        [DataRow("perTest", OptimizationModes.CoverageBasedTest)]
        [DataRow("all", OptimizationModes.SkipUncoveredMutants)]
        [DataRow("off", OptimizationModes.None)]
        public void ShouldSetFlags(string value, params OptimizationModes[] expectedFlags)
        {
            var target = new CoverageAnalysisInput { SuppliedInput = value };

            var result = target.Validate();

            foreach (var flag in expectedFlags)
            {
                result.HasFlag(flag).ShouldBeTrue();
            }
        }

        [TestMethod]
        public void ShouldThrowOnInvalidOptimizationMode()
        {
            var target = new CoverageAnalysisInput { SuppliedInput = "gibberish" };

            var exception = Should.Throw<InputException>(() => target.Validate());

            exception.Message.ShouldMatch("Incorrect coverageAnalysis option \\(gibberish\\)\\. The options are \\[.+\\]\\.");

            exception.ToString().ShouldContain("[off, perTest, all, perTestInIsolation].");
        }
    }
}
