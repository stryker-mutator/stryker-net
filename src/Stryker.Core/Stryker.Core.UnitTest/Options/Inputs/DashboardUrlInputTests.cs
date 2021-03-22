using Shouldly;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class DashboardUrlInputTests
    {
        [Fact]
        public void ShouldContainCorrectDefaults()
        {
            var defaultValue = new DashboardUrlInput().Validate();

            defaultValue.ShouldBe("https://dashboard.stryker-mutator.io");
        }
    }
}
