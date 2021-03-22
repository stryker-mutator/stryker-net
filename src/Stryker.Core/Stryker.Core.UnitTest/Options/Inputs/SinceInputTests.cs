using Shouldly;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class SinceInputTests
    {
        [Fact]
        public void Should_Enable_Diff_When_CompareToDashboard_Is_Enabled()
        {
            var sinceEnabled = new SinceInput().Validate(true);

            sinceEnabled.ShouldBeTrue();
        }
    }
}
