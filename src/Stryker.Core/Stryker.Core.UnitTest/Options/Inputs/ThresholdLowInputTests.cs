using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class ThresholdLowInputTests
    {
        [Theory]
        [InlineData(-1, "Threshold low must be between 0 and 100.")]
        [InlineData(101, "Threshold low must be between 0 and 100.")]
        [InlineData(61, "Threshold low must be less than or equal to threshold high and more than or equal to threshold break. Current high: 60, low: 61, break: 60.")]
        public void ShouldValidateThresholdLow(int thresholdLow, string message)
        {
            var ex = Assert.Throws<InputException>(() =>
            {
                var options = new ThresholdLowInput { SuppliedInput = thresholdLow }.Validate(60, 60);
            });
            ex.Message.ShouldBe(message);
        }
    }
}
