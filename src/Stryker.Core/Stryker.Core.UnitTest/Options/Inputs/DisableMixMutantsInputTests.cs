using Shouldly;
using Stryker.Configuration;
using Stryker.Configuration.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Configuration.UnitTest.Options.Inputs
{
    [TestClass]
    public class DisableMixMutantsInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new DisableMixMutantsInput();
            target.HelpText.ShouldBe(@"Test each mutation in an isolated test run. | default: 'False'");
        }

        [TestMethod]
        [DataRow(false, OptimizationModes.None)]
        [DataRow(true, OptimizationModes.DisableMixMutants)]
        [DataRow(null, OptimizationModes.None)]
        public void ShouldValidate(bool? input, OptimizationModes expected)
        {
            var target = new DisableMixMutantsInput { SuppliedInput = input };

            var result = target.Validate();

            result.ShouldBe(expected);
        }
    }
}
