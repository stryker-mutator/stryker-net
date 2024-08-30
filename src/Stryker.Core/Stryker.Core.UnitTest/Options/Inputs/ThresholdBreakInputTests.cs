using Shouldly;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Abstractions.UnitTest.Options.Inputs
{
    [TestClass]
    public class ThresholdBreakInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new ThresholdBreakInput();
            target.HelpText.ShouldBe(@"Anything below this mutation score will return a non-zero exit code. Must be less than or equal to threshold low. | default: '0' | allowed: 0 - 100");
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(101)]
        public void ShouldValidateThresholdBreak(int thresholdBreak)
        {
            var ex = Should.Throw<InputException>(() =>
            {
                new ThresholdBreakInput { SuppliedInput = thresholdBreak }.Validate(low: 50);
            });
            ex.Message.ShouldBe("Threshold break must be between 0 and 100.");
        }

        [TestMethod]
        public void ThresholdBreakShouldBeLowerThanOrEqualToThresholdLow()
        {
            var ex = Should.Throw<InputException>(() =>
            {
                new ThresholdBreakInput { SuppliedInput = 51 }.Validate(low: 50);
            });
            ex.Message.ShouldBe("Threshold break must be less than or equal to threshold low. Current break: 51, low: 50.");
        }

        [TestMethod]
        public void CanBeEqualToThresholdLow()
        {
            var input = 60;
            var options = new ThresholdBreakInput { SuppliedInput = input }.Validate(low: 60);
            options.ShouldBe(input);
        }

        [TestMethod]
        public void ShouldAllow100PercentBreak()
        {
            var result = new ThresholdBreakInput { SuppliedInput = 100 }.Validate(low: 100);
            result.ShouldBe(100, "because some people will not allow any mutations in their projects.");
        }

        [TestMethod]
        public void ShouldAllow0PercentBreak()
        {
            var result = new ThresholdBreakInput { SuppliedInput = 0 }.Validate(low: 100);
            result.ShouldBe(0, "because some users will want to break only on literally 0.00 percent score.");
        }

        [TestMethod]
        public void ShouldBeDefaultValueWhenNull()
        {
            var input = new ThresholdBreakInput { SuppliedInput = null };
            var options = input.Validate(low: 80);
            options.ShouldBe(input.Default.Value);
        }
    }
}
