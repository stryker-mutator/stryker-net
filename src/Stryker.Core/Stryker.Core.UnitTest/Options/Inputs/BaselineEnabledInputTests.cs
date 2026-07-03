using Shouldly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Configuration.Options.Inputs;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class BaselineEnabledInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new BaselineEnabledInput();
        target.HelpText.ShouldBe(@"EXPERIMENTAL: Use results stored in stryker dashboard to only test new mutants. | default: 'False'");
    }

    [TestMethod]
    public void ShouldBeEnabledWhenTrue()
    {
        var target = new BaselineEnabledInput { SuppliedInput = true };

        var result = target.Validate();

        result.ShouldBeTrue();
    }

    [TestMethod]
    public void ShouldProvideDefaultFalseWhenNull()
    {
        var target = new BaselineEnabledInput { SuppliedInput = null };

        var result = target.Validate();

        result.ShouldBeFalse();
    }

    [TestMethod]
    public void ShouldNotBeEnabledWhenFalse()
    {
        var target = new BaselineEnabledInput { SuppliedInput = false };

        var result = target.Validate();

        result.ShouldBeFalse();
    }
}
