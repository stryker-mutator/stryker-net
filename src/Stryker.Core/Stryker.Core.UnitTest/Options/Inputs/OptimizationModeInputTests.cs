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
        public void ShouldValidateOptimizationMode()
        {
            var ex = Assert.Throws<InputException>(() =>
            {
                new CoverageAnalysisInput { SuppliedInput = "gibberish" }.Validate();
            });
            ex.Message.ShouldBe($"Incorrect coverageAnalysis option (gibberish). The options are [Off, All, PerTest or PerTestInIsolation].");
        }
    }
}
