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

            exception.Message.ShouldBe("When the compare to dashboard feature is enabled, dashboard-version cannot be empty, please provide a dashboard-version");
        }
    }
}
