using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    [TestClass]
    public class ThresholdHighInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new ThresholdHighInput();
            target.HelpText.ShouldBe(@"Minimum good mutation score. Must be higher than or equal to threshold low. | default: '80' | allowed: 0 - 100");
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(101)]
        public void MustBeBetween0and100(int thresholdHigh)
        {
            var ex = Should.Throw<InputException>(() =>
            {
                var options = new ThresholdHighInput { SuppliedInput = thresholdHigh }.Validate(low: 0);
            });
            ex.Message.ShouldBe("Threshold high must be between 0 and 100.");
        }

        [TestMethod]
        public void MustBeMoreThanOrEqualToThresholdLow()
        {
            var ex = Should.Throw<InputException>(() =>
            {
                var options = new ThresholdHighInput { SuppliedInput = 59 }.Validate(low: 60);
            });
            ex.Message.ShouldBe("Threshold high must be higher than or equal to threshold low. Current high: 59, low: 60.");
        }

        [TestMethod]
        public void CanBeEqualToThresholdLow()
        {
            var input = 60;
            var options = new ThresholdHighInput { SuppliedInput = input }.Validate(low: 60);
            options.ShouldBe(input);
        }

        [TestMethod]
        public void ShouldAllow0()
        {
            var input = 0;
            var options = new ThresholdHighInput { SuppliedInput = input }.Validate(low: 0);
            options.ShouldBe(input);
        }

        [TestMethod]
        public void ShouldAllow100()
        {
            var input = 100;
            var options = new ThresholdHighInput { SuppliedInput = input }.Validate(low: 60);
            options.ShouldBe(input);
        }

        [TestMethod]
        public void ShouldBeDefaultValueWhenNull()
        {
            var input = new ThresholdHighInput { SuppliedInput = null };
            var options = input.Validate(low: 60);
            options.ShouldBe(input.Default.Value);
        }
    }
}
