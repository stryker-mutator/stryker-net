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
        public void MustBeBetween1and100(int thresholdHigh, string message)
        {
            var ex = Assert.Throws<InputException>(() =>
            {
                var options = new ThresholdHighInput { SuppliedInput = thresholdHigh }.Validate(low: 60);
            });
            ex.Message.ShouldBe(message);
        }

        [Fact]
        public void MustBeMoreThanOrEqualToThresholdLow()
        {
            var ex = Assert.Throws<InputException>(() =>
            {
                var options = new ThresholdHighInput { SuppliedInput = 59 }.Validate(low: 60);
            });
            ex.Message.ShouldBe("Threshold high must be higher than or equal to threshold low. Current high: 59, low: 60.");
        }

        [Fact]
        public void CanBeEqualToThresholdLow()
        {
            var input = 60;
            var options = new ThresholdHighInput { SuppliedInput = input }.Validate(low: 60);
            options.ShouldBe(input);
        }

        [Fact]
        public void ShouldBeDefaultValueWhenNull()
        {
            var input = new ThresholdHighInput { SuppliedInput = null };
            var options = input.Validate(low: 60);
            options.ShouldBe(input.Default.Value);
        }
    }
}
