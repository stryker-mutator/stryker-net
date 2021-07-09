using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class ProjectNameInputTests
    {
        [Fact]
        public void ShouldHaveHelptext()
        {
            var target = new ProjectNameInput();
            target.HelpText.ShouldBe(@"The organizational name for your project. Required when dashboard reporter is turned on.
For example: Your project might be called 'consumer-loans' and it might contains sub-modules 'consumer-loans-frontend' and 'consumer-loans-backend'. | default: ''");
        }

        [Fact]
        public void ShouldReturnName()
        {
            var input = new ProjectNameInput { SuppliedInput = "name" };

            var result = input.Validate(new[] { Reporter.Dashboard });

            result.ShouldBe("name");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ShouldThrowOnEmptyValue(string value)
        {
            var input = new ProjectNameInput { SuppliedInput = value };

            var exception = Should.Throw<InputException>(() =>
            {
                input.Validate(new[] { Reporter.Dashboard });
            });

            exception.Message.ShouldBe("When the stryker dashboard is enabled the project name is required.");
        }

        [Fact]
        public void ShouldHaveDefault()
        {
            var input = new ProjectNameInput { SuppliedInput = null };
            
            var result = input.Validate(new Reporter[] { });

            result.ShouldBe(string.Empty);
        }
    }
}
