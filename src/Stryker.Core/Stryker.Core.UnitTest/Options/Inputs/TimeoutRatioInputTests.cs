using Shouldly;
using Stryker.Abstractions.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Configuration.Options.Inputs;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class TimeoutRatioInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new TimeoutRatioInput();
        target.HelpText.ShouldBe(@"The ratio the estimated test time is multiplied by when calculating the timeout for a mutant.
A timeout is calculated per mutant based on the initial unit test run before mutating.
Increase this value if you experience a lot of timeouts, decrease it to catch endless loops faster. | default: '1.5'");
    }

    [TestMethod]
    public void ShouldHaveDefault()
    {
        var target = new TimeoutRatioInput { SuppliedInput = null };

        var result = target.Validate();

        result.ShouldBe(1.5);
    }

    [TestMethod]
    public void ShouldAllowCustomRatio()
    {
        var target = new TimeoutRatioInput { SuppliedInput = 2.5 };

        var result = target.Validate();

        result.ShouldBe(2.5);
    }

    [TestMethod]
    public void ShouldThrowAtZero()
    {
        var target = new TimeoutRatioInput { SuppliedInput = 0 };

        var exception = Should.Throw<InputException>(() => target.Validate());

        exception.Message.ShouldBe("Timeout ratio must be higher than 0.");
    }

    [TestMethod]
    public void ShouldThrowAtNegative()
    {
        var target = new TimeoutRatioInput { SuppliedInput = -1 };

        var exception = Should.Throw<InputException>(() => target.Validate());

        exception.Message.ShouldBe("Timeout ratio must be higher than 0.");
    }
}
