using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class ThresholdBreakInputTests
    {
        [Fact]
        public void ShouldHaveHelptext()
        {
            var target = new ThresholdBreakInput();
            target.HelpText.ShouldBe(@"Anything below this mutation score will return a non-zero exit code. Must be less than or equal to threshold low. | default: '0' | allowed: 0 - 100");
        }

        [Theory]
        [InlineData(-1, "Threshold break must be between 0 and 100.")]
        [InlineData(101, "Threshold break must be between 0 and 100.")]
        public void ShouldValidateThresholdBreak(int thresholdBreak, string message)
        {
            var ex = Assert.Throws<InputException>(() =>
            {
                new ThresholdBreakInput { SuppliedInput = thresholdBreak }.Validate(low: 50);
            });
            ex.Message.ShouldBe(message);
        }

        [Fact]
        public void ThresholdBreakShouldBeLowerThanOrEqualToThresholdLow()
        {
            var ex = Assert.Throws<InputException>(() =>
            {
                new ThresholdBreakInput { SuppliedInput = 51 }.Validate(low: 50);
            });
            ex.Message.ShouldBe("Threshold break must be less than or equal to threshold low. Current break: 51, low: 50.");
        }

        [Fact]
        public void CanBeEqualToThresholdLow()
        {
            var input = 60;
            var options = new ThresholdBreakInput { SuppliedInput = input }.Validate(low: 60);
            options.ShouldBe(input);
        }

        [Fact]
        public void ShouldAllow100PercentBreak()
        {
            var result = new ThresholdBreakInput { SuppliedInput = 100 }.Validate(low: 100);
            result.ShouldBe(100, "because some people will not allow any mutations in their projects.");
        }

        [Fact]
        public void ShouldAllow0PercentBreak()
        {
            var result = new ThresholdBreakInput { SuppliedInput = 0 }.Validate(low: 100);
            result.ShouldBe(0, "because should be able to be turned off.");
        }

        [Fact]
        public void ShouldBeDefaultValueWhenNull()
        {
            var input = new ThresholdBreakInput { SuppliedInput = null };
            var options = input.Validate(low: 80);
            options.ShouldBe(input.Default.Value);
        }
    }
}
