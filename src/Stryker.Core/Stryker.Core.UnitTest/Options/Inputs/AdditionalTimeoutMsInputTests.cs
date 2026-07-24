using Shouldly;
using Stryker.Abstractions.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Configuration.Options.Inputs;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class AdditionalTimeoutMsInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new AdditionalTimeoutInput();
        target.HelpText.ShouldBe(@"A number of milliseconds that is added to the calculated timeout value for each mutant.
A timeout is calculated per mutant based on the initial unit test run before mutating.
To prevent infinite loops Stryker cancels a testrun if it runs longer than the timeout value.
If you experience a lot of timeouts you might need to increase the timeout value. | default: '1000'");
    }

    [TestMethod]
    public void ShouldAllowZero()
    {
        var target = new AdditionalTimeoutInput { SuppliedInput = 0 };

        var result = target.Validate();

        result.ShouldBe(0);
    }

    [TestMethod]
    public void ShouldHaveDefault()
    {
        var target = new AdditionalTimeoutInput { SuppliedInput = null };

        var result = target.Validate();

        result.ShouldBe(1000);
    }

    [TestMethod]
    public void ShouldAllowMillion()
    {
        var target = new AdditionalTimeoutInput { SuppliedInput = 1000000 };

        var result = target.Validate();

        result.ShouldBe(1000000);
    }

    [TestMethod]
    public void ShouldThrowAtNegative()
    {
        var target = new AdditionalTimeoutInput { SuppliedInput = -1 };

        var exception = Should.Throw<InputException>(() => target.Validate());

        exception.Message.ShouldBe("Timeout cannot be negative.");
    }
}
