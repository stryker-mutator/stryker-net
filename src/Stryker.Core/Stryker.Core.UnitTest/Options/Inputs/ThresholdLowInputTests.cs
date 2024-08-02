using Shouldly;
using Stryker.Configuration.Exceptions;
using Stryker.Configuration.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Configuration.UnitTest.Options.Inputs
{
    [TestClass]
    public class ThresholdLowInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new ThresholdLowInput();
            target.HelpText.ShouldBe(@"Minimum acceptable mutation score. Must be less than or equal to threshold high and more than or equal to threshold break. | default: '60' | allowed: 0 - 100");
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(101)]
        public void MustBeBetween0and100(int thresholdLow)
        {
            var ex = Should.Throw<InputException>(() =>
            {
                var options = new ThresholdLowInput { SuppliedInput = thresholdLow }.Validate(@break: 0, high: 100);
            });
            ex.Message.ShouldBe("Threshold low must be between 0 and 100.");
        }

        [TestMethod]
        public void MustBeLessthanOrEqualToThresholdHigh()
        {
            var ex = Should.Throw<InputException>(() =>
            {
                var options = new ThresholdLowInput { SuppliedInput = 61 }.Validate(@break: 60, high: 60);
            });
            ex.Message.ShouldBe("Threshold low must be less than or equal to threshold high. Current low: 61, high: 60.");
        }

        [TestMethod]
        public void MustBeMoreThanThresholdBreak()
        {
            var ex = Should.Throw<InputException>(() =>
            {
                var options = new ThresholdLowInput { SuppliedInput = 59 }.Validate(@break: 60, high: 60);
            });
            ex.Message.ShouldBe("Threshold low must be more than or equal to threshold break. Current low: 59, break: 60.");
        }

        [TestMethod]
        public void CanBeEqualToThresholdBreak()
        {
            var input = 60;
            var options = new ThresholdLowInput { SuppliedInput = input }.Validate(@break: 60, high: 100);
            options.ShouldBe(input);
        }

        [TestMethod]
        public void CanBeEqualToThresholdHigh()
        {
            var input = 60;
            var options = new ThresholdLowInput { SuppliedInput = input }.Validate(@break: 0, high: 60);
            options.ShouldBe(input);
        }

        [TestMethod]
        public void ShouldAllow0()
        {
            var input = 0;
            var options = new ThresholdLowInput { SuppliedInput = input }.Validate(@break: 0, high: 100);
            options.ShouldBe(input);
        }

        [TestMethod]
        public void ShouldAllow100()
        {
            var input = 100;
            var options = new ThresholdLowInput { SuppliedInput = input }.Validate(@break: 0, high: 100);
            options.ShouldBe(input);
        }

        [TestMethod]
        public void ShouldBeDefaultValueWhenNull()
        {
            var input = new ThresholdLowInput { SuppliedInput = null };
            var options = input.Validate(@break: 0, high: 80);
            options.ShouldBe(input.Default.Value);
        }
    }
}
