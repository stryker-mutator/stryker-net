using Shouldly;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class WithBaselineInputTests
    {
        [Fact]
        public void ShouldBeEnabledWhenTrue()
        {
            var target = new WithBaselineInput { SuppliedInput = null };

            var result = target.Validate();

            result.ShouldBeFalse();
        }

        [Fact]
        public void ShouldProvideDefaultWhenNull()
        {
            var target = new WithBaselineInput { SuppliedInput = null };

            var result = target.Validate();

            result.ShouldBe(target.Default.Value);
        }

        [Fact]
        public void ShouldNotBeEnabledWhenFalse()
        {
            var target = new WithBaselineInput { SuppliedInput = false };

            var result = target.Validate();

            result.ShouldBeFalse();
        }
    }
}
