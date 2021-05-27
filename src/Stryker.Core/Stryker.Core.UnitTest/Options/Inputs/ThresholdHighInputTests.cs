using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class ThresholdHighInputTests
    {
        [Theory]
        [InlineData(0, "Threshold high must be between 1 and 100.")]
        [InlineData(101, "Threshold high must be between 1 and 100.")]
        [InlineData(59, "Threshold high must be higher than or equal to threshold low. Current low: 60, high: 59.")]
        public void ShouldValidateThresholdHigh(int thresholdHigh, string message)
        {
            var ex = Assert.Throws<InputException>(() =>
            {
                var options = new ThresholdHighInput { SuppliedInput = thresholdHigh }.Validate(60);
            });
            ex.Message.ShouldBe(message);
        }
    }
}
