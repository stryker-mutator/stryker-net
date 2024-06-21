using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    [TestClass]
    public class SourceProjectNameInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new SourceProjectNameInput();
            target.HelpText.ShouldBe(@"Used to find the project to test in the project references of the test project. Example: ""ExampleProject.csproj"" | default: ''");
        }

        [TestMethod]
        public void ShouldReturnName()
        {
            var target = new SourceProjectNameInput { SuppliedInput = "name" };

            var result = target.Validate();

            result.ShouldBe("name");
        }

        [TestMethod]
        public void ShouldHaveDefault()
        {
            var target = new SourceProjectNameInput { SuppliedInput = null };

            var result = target.Validate();

            result.ShouldBe("");
        }

        [TestMethod]
        [DataRow("")]
        public void ShouldThrowOnEmpty(string value)
        {
            var target = new SourceProjectNameInput { SuppliedInput = value };

            var exception = Should.Throw<InputException>(() => target.Validate());

            exception.Message.ShouldBe("Project file cannot be empty.");
        }
    }
}
