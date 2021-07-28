using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class ProjectUnderTestNameInputTests
    {
        [Fact]
        public void ShouldHaveHelpText()
        {
            var target = new ProjectUnderTestNameInput();
            target.HelpText.ShouldBe(@"Used to find the project to test in the project references of the test project. Example: ""ExampleProject.csproj"" | default: ''");
        }

        [Fact]
        public void ShouldReturnName()
        {
            var target = new ProjectUnderTestNameInput { SuppliedInput = "name" };

            var result = target.Validate();

            result.ShouldBe("name");
        }

        [Fact]
        public void ShouldHaveDefault()
        {
            var target = new ProjectUnderTestNameInput { SuppliedInput = null };

            var result = target.Validate();

            result.ShouldBe("");
        }

        [Theory]
        [InlineData("")]
        public void ShouldThrowOnEmpty(string value)
        {
            var target = new ProjectUnderTestNameInput { SuppliedInput = value };

            var exception = Should.Throw<InputException>(() => target.Validate());

            exception.Message.ShouldBe("Project file cannot be empty.");
        }
    }
}
