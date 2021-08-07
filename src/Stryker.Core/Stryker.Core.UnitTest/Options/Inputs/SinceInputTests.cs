using Shouldly;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class SinceInputTests : TestBase
    {
        [Fact]
        public void ShouldHaveHelpText()
        {
            var target = new SinceInput();
            target.HelpText.ShouldBe(@"Enables diff compare. Only test changed files. | default: 'False'");
        }

        [Fact]
        public void ShouldBeEnabledWhenTrue()
        {
            var target = new SinceInput { SuppliedInput = true };

            var result = target.Validate(withBaseline: null);

            result.ShouldBeTrue();
        }

        [Fact]
        public void ShouldBeEnabledWhenTrueEvenIfWithBaselineFalse()
        {
            var target = new SinceInput { SuppliedInput = true };

            var result = target.Validate(withBaseline: false);

            result.ShouldBeTrue();
        }

        [Fact]
        public void ShouldProvideDefaultWhenNull()
        {
            var target = new SinceInput();

            var result = target.Validate(withBaseline: null);

            result.ShouldBe(target.Default.Value);
        }

        [Fact]
        public void ShouldNotBeEnabledWhenFalse()
        {
            var target = new SinceInput { SuppliedInput = false };

            var result = target.Validate(withBaseline: null);

            result.ShouldBeFalse();
        }

        [Fact]
        public void ShouldBeEnabledWithBaseline()
        {
            var sinceEnabled = new SinceInput().Validate(withBaseline: true);

            sinceEnabled.ShouldBeTrue();
        }
    }
}
