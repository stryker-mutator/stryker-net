using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class OptimizationModeInputTests
    {
        [Fact]
        public void ShouldValidateOptimisationMode()
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new CoverageAnalysisInput { SuppliedInput = "gibberish" }.Validate();
            });
            ex.Details.ShouldBe($"Incorrect coverageAnalysis option (gibberish). The options are [Off, All, PerTest or PerTestInIsolation].");
        }

        [Fact]
        public void ShouldSetOptimisationMode()
        {
            var flags = new CoverageAnalysisInput { SuppliedInput = "perTestInIsolation" }.Validate();
            flags.HasFlag(OptimizationModes.CoverageBasedTest).ShouldBeTrue();
            flags.HasFlag(OptimizationModes.CaptureCoveragePerTest).ShouldBeTrue();

            flags = new CoverageAnalysisInput { SuppliedInput = null }.Validate();
            flags.HasFlag(OptimizationModes.CoverageBasedTest).ShouldBeTrue();

            flags = new CoverageAnalysisInput { SuppliedInput = "all" }.Validate();
            flags.HasFlag(OptimizationModes.SkipUncoveredMutants).ShouldBeTrue();

            flags = new CoverageAnalysisInput { SuppliedInput = "off" }.Validate();
            flags.HasFlag(OptimizationModes.NoOptimization).ShouldBeTrue();
        }
    }
}
