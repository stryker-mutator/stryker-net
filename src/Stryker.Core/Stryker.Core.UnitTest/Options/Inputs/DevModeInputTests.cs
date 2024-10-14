using Shouldly;
using Stryker.Abstractions.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    [TestClass]
    public class DevModeInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new DevModeInput();
            target.HelpText.ShouldBe(@"Stryker automatically removes all mutations from a method if a failed mutation could not be rolled back
    Setting this flag makes stryker not remove the mutations but rather crash on failed rollbacks | default: 'False'");
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, true)]
        [DataRow(null, false)]
        public void ShouldValidate(bool? input, bool expected)
        {
            var target = new DevModeInput { SuppliedInput = input };

            var result = target.Validate();

            result.ShouldBe(expected);
        }
    }
}
