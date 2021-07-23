using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class ThresholdHighInputTests
    {
        [Fact]
        public void ShouldHaveHelptext()
        {
            var target = new ThresholdHighInput();
            target.HelpText.ShouldBe(@"Minimum good mutation score. Must be higher than or equal to threshold low. | default: '80' | allowed: 1 - 100");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        public void MustBeBetween1and100(int thresholdHigh)
        {
            var ex = Assert.Throws<InputException>(() =>
            {
                var options = new ThresholdHighInput { SuppliedInput = thresholdHigh }.Validate(low: 60);
            });
            ex.Message.ShouldBe("Threshold high must be in range 2 to 100.");
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
        public void ShouldAllow2()
        {
            var input = 2;
            var options = new ThresholdHighInput { SuppliedInput = input }.Validate(low: 1);
            options.ShouldBe(input);
        }

        [Fact]
        public void ShouldAllow100()
        {
            var input = 100;
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
