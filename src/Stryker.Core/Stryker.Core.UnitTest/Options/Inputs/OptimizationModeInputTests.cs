using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class OptimizationModeInputTests
    {
        [Theory]
        [InlineData(null, OptimizationModes.CaptureCoveragePerTest)]
        [InlineData("pertestinisolation", OptimizationModes.CoverageBasedTest, OptimizationModes.CaptureCoveragePerTest)]
        [InlineData("pertest", OptimizationModes.CoverageBasedTest)]
        [InlineData("all", OptimizationModes.SkipUncoveredMutants)]
        [InlineData("off", OptimizationModes.NoOptimization)]
        public void ShouldSetFlags(string value, params OptimizationModes[] expectedFlags)
        {
            var target = new CoverageAnalysisInput { SuppliedInput = value };

            var result = target.Validate();

            foreach (var flag in expectedFlags)
            {
                result.HasFlag(flag).ShouldBeTrue();
            }
        }

        [Fact]
        public void ShouldThrowOnInvalidOptimizationMode()
        {
            var target = new CoverageAnalysisInput { SuppliedInput = "gibberish" };

            var ex = Assert.Throws<InputException>(() => target.Validate());

            ex.Message.ShouldBe($"Incorrect coverageAnalysis option (gibberish). The options are [Off, All, PerTest or PerTestInIsolation].");
        }
    }
}
