using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Options.Inputs;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    [TestClass]
    public class ProjectNameInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new ProjectNameInput();
            target.HelpText.ShouldBe(@"The organizational name for your project. Required when dashboard reporter is turned on.
For example: Your project might be called 'consumer-loans' and it might contains sub-modules 'consumer-loans-frontend' and 'consumer-loans-backend'. | default: ''");
        }

        [TestMethod]
        public void ShouldReturnName()
        {
            var input = new ProjectNameInput { SuppliedInput = "name" };

            var result = input.Validate();

            result.ShouldBe("name");
        }

        [TestMethod]
        public void ShouldHaveDefault()
        {
            var input = new ProjectNameInput { SuppliedInput = null };

            var result = input.Validate();

            result.ShouldBe(string.Empty);
        }
    }
}
