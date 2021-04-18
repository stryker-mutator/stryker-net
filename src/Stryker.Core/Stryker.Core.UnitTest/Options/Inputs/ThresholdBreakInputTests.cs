using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class ThresholdBreakInputTests
    {
        [Theory]
        [InlineData(101, "Threshold break must be between 0 and 100.")]
        [InlineData(1000, "Threshold break must be between 0 and 100.")]
        [InlineData(-1, "Threshold break must be between 0 and 100.")]
        [InlineData(51, "Threshold break must be less than or equal to threshold low. Current low: 50, break: 51")]
        public void ShouldValidateThresholdsIncorrect(int thresholdBreak, string message)
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new ThresholdBreakInput { SuppliedInput = thresholdBreak }.Validate(50);
            });
            ex.Message.ShouldBe(message);
        }

        [Fact]
        public void ShouldAllow100PercentBreak()
        {
            var result = new ThresholdBreakInput { SuppliedInput = 100 }.Validate(100);
            result.ShouldBe(100);
        }
    }
}
