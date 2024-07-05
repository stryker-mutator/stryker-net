using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    [TestClass]
    public class DisableBailInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new DisableBailInput();
            target.HelpText.ShouldBe(@"Disable abort unit testrun as soon as the first unit test fails. | default: 'False'");
        }

        [TestMethod]
        [DataRow(false, OptimizationModes.None)]
        [DataRow(true, OptimizationModes.DisableBail)]
        [DataRow(null, OptimizationModes.None)]
        public void ShouldValidate(bool? input, OptimizationModes expected)
        {
            var target = new DisableBailInput { SuppliedInput = input };

            var result = target.Validate();

            result.ShouldBe(expected);
        }
    }
}
