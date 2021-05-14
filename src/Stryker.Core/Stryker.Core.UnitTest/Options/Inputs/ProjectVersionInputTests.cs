using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class ProjectVersionInputTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ProjectVersionCannotBeEmpty(string value)
        {
            var input = new ProjectVersionInput { };
            input.SuppliedInput = value;

            var exception = Should.Throw<StrykerInputException>(() => {
                input.Validate(null, reporters: new[] { Reporter.Dashboard }, true);
            });

            exception.Message.ShouldBe("When the stryker dashboard is enabled the project version is required. Please provide a project version.");
        }
    }
}
