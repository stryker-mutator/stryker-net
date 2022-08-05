using Shouldly;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class BaselineCreateEnabledInputTests : TestBase
    {
        [Fact]
        public void ShouldHaveHelpText()
        {
            var target = new BaselineCreateEnabledInput();
            target.HelpText.ShouldBe("When enabled a new baseline will be created by doing a full run and storing the mutation results. | default: 'False'");
        }

        [Fact]
        public void ShouldHaveDefault()
        {
            var target = new BaselineCreateEnabledInput { SuppliedInput = true };

            var result = target.Validate();

            target.Default.ShouldBeFalse();
            result.ShouldBeTrue();
        }

        [Fact]
        public void ShouldHaveDefaultForDashboard()
        {
            var target = new BaselineCreateEnabledInput { SuppliedInput = false };

            var result = target.Validate();

            target.Default.ShouldBeFalse();
            result.ShouldBeFalse();
        }
    }
}
