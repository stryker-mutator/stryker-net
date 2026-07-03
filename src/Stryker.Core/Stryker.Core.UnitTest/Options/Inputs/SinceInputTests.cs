using Shouldly;
using Stryker.Abstractions.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Configuration.Options.Inputs;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class SinceInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new SinceInput();
        target.HelpText.ShouldBe(@"Enables diff compare. Only test changed files. | default: 'False'");
    }

    [TestMethod]
    public void ShouldBeEnabledWhenTrue()
    {
        var target = new SinceInput { SuppliedInput = true };

        var result = target.Validate(baselineEnabled: null);

        result.ShouldBeTrue();
    }

    [TestMethod]
    public void ShouldBeEnabledWhenTrueEvenIfBaselineEnabledFalse()
    {
        var target = new SinceInput { SuppliedInput = true };

        var result = target.Validate(baselineEnabled: false);

        result.ShouldBeTrue();
    }

    [TestMethod]
    public void ShouldProvideDefaultWhenNull()
    {
        var target = new SinceInput();

        var result = target.Validate(baselineEnabled: null);

        result.ShouldBe(target.Default.Value);
    }

    [TestMethod]
    public void ShouldNotBeEnabledWhenFalse()
    {
        var target = new SinceInput { SuppliedInput = false };

        var result = target.Validate(baselineEnabled: null);

        result.ShouldBeFalse();
    }

    [TestMethod]
    public void ShouldBeImplicitlyEnabledWithBaseline()
    {
        var sinceEnabled = new SinceInput().Validate(baselineEnabled: true);

        sinceEnabled.ShouldBeTrue();
    }

    [TestMethod]
    public void ShouldNotBeAllowedToExplicitlyEnableWithBaseline()
    {
        var sinceEnabled = new SinceInput { SuppliedInput = true };

        var exception = Should.Throw<InputException>(() => sinceEnabled.Validate(baselineEnabled: true));
        exception.Message.ShouldBe("The since and baseline features are mutually exclusive.");
    }
}
