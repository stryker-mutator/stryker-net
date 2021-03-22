using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class ThresholdLowInputTests
    {
        [Theory]
        [InlineData(101, "The thresholds must be between 0 and 100")]
        [InlineData(1000, "The thresholds must be between 0 and 100")]
        [InlineData(-1, "The thresholds must be between 0 and 100")]
        [InlineData(61, "The values of your thresholds are incorrect. Change `--threshold-break` to the lowest value and `--threshold-high` to the highest.")]
        public void ShouldValidateThresholdsIncorrect(int thresholdBreak, string message)
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                var options = new ThresholdLowInput { SuppliedInput = thresholdBreak }.Validate(60, 60);
            });
            ex.Details.ShouldBe(message);
        }
    }
}
