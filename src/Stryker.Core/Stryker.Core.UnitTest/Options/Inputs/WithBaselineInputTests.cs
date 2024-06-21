using Shouldly;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class BaselineEnabledInputTests : TestBase
    {
        [Fact]
        public void ShouldHaveHelpText()
        {
            var target = new BaselineEnabledInput();
            target.HelpText.ShouldBe(@"EXPERIMENTAL: Use results stored in stryker dashboard to only test new mutants. | default: 'False'");
        }

        [Fact]
        public void ShouldBeEnabledWhenTrue()
        {
            var target = new BaselineEnabledInput { SuppliedInput = true };

            var result = target.Validate();

            result.ShouldBeTrue();
        }

        [Fact]
        public void ShouldProvideDefaultFalseWhenNull()
        {
            var target = new BaselineEnabledInput { SuppliedInput = null };

            var result = target.Validate();

            result.ShouldBeFalse();
        }

        [Fact]
        public void ShouldNotBeEnabledWhenFalse()
        {
            var target = new BaselineEnabledInput { SuppliedInput = false };

            var result = target.Validate();

            result.ShouldBeFalse();
        }
    }
}
