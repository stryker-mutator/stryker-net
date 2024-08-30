using Shouldly;
using Stryker.Abstractions.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Abstractions.UnitTest.Options.Inputs
{
    [TestClass]
    public class WithBaselineInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new WithBaselineInput();
            target.HelpText.ShouldBe(@"EXPERIMENTAL: Use results stored in stryker dashboard to only test new mutants. | default: 'False'");
        }

        [TestMethod]
        public void ShouldBeEnabledWhenTrue()
        {
            var target = new WithBaselineInput { SuppliedInput = true };

            var result = target.Validate();

            result.ShouldBeTrue();
        }

        [TestMethod]
        public void ShouldProvideDefaultFalseWhenNull()
        {
            var target = new WithBaselineInput { SuppliedInput = null };

            var result = target.Validate();

            result.ShouldBeFalse();
        }

        [TestMethod]
        public void ShouldNotBeEnabledWhenFalse()
        {
            var target = new WithBaselineInput { SuppliedInput = false };

            var result = target.Validate();

            result.ShouldBeFalse();
        }
    }
}
