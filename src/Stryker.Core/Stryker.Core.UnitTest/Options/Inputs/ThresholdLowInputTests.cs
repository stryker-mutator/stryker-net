using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class ThresholdLowInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new ThresholdLowInput();
        target.HelpText.ShouldBe(@"Minimum acceptable mutation score. Must be less than or equal to threshold high and more than or equal to threshold break. | default: '60' | allowed: 0 - 100");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void MustBeBetween0and100(int thresholdLow)
    {
        var ex = Assert.Throws<InputException>(() =>
        {
            var options = new ThresholdLowInput { SuppliedInput = thresholdLow }.Validate(@break: 0, high: 100);
        });
        ex.Message.ShouldBe("Threshold low must be between 0 and 100.");
    }

    [Fact]
    public void MustBeLessthanOrEqualToThresholdHigh()
    {
        var ex = Assert.Throws<InputException>(() =>
        {
            var options = new ThresholdLowInput { SuppliedInput = 61 }.Validate(@break: 60, high: 60);
        });
        ex.Message.ShouldBe("Threshold low must be less than or equal to threshold high. Current low: 61, high: 60.");
    }

    [Fact]
    public void MustBeMoreThanThresholdBreak()
    {
        var ex = Assert.Throws<InputException>(() =>
        {
            var options = new ThresholdLowInput { SuppliedInput = 59 }.Validate(@break: 60, high: 60);
        });
        ex.Message.ShouldBe("Threshold low must be more than or equal to threshold break. Current low: 59, break: 60.");
    }

    [Fact]
    public void CanBeEqualToThresholdBreak()
    {
        var input = 60;
        var options = new ThresholdLowInput { SuppliedInput = input }.Validate(@break: 60, high: 100);
        options.ShouldBe(input);
    }

    [Fact]
    public void CanBeEqualToThresholdHigh()
    {
        var input = 60;
        var options = new ThresholdLowInput { SuppliedInput = input }.Validate(@break: 0, high: 60);
        options.ShouldBe(input);
    }

    [Fact]
    public void ShouldAllow0()
    {
        var input = 0;
        var options = new ThresholdLowInput { SuppliedInput = input }.Validate(@break: 0, high: 100);
        options.ShouldBe(input);
    }

    [Fact]
    public void ShouldAllow100()
    {
        var input = 100;
        var options = new ThresholdLowInput { SuppliedInput = input }.Validate(@break: 0, high: 100);
        options.ShouldBe(input);
    }

    [Fact]
    public void ShouldBeDefaultValueWhenNull()
    {
        var input = new ThresholdLowInput { SuppliedInput = null };
        var options = input.Validate(@break: 0, high: 80);
        options.ShouldBe(input.Default.Value);
    }
}
