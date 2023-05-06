using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class AdditionalTimeoutMsInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new AdditionalTimeoutInput();
        target.HelpText.ShouldBe(@"A timeout is calculated based on the initial unit test run before mutating.
To prevent infinite loops Stryker cancels a testrun if it runs longer than the timeout value.
If you experience a lot of timeouts you might need to increase the timeout value. | default: '5000'");
    }

    [Fact]
    public void ShouldAllowZero()
    {
        var target = new AdditionalTimeoutInput { SuppliedInput = 0 };

        var result = target.Validate();

        result.ShouldBe(0);
    }
    
    [Fact]
    public void ShouldHaveDefault()
    {
        var target = new AdditionalTimeoutInput { SuppliedInput = null };

        var result = target.Validate();

        result.ShouldBe(5000);
    }

    [Fact]
    public void ShouldAllowMillion()
    {
        var target = new AdditionalTimeoutInput { SuppliedInput = 1000000 };

        var result = target.Validate();

        result.ShouldBe(1000000);
    }

    [Fact]
    public void ShouldThrowAtNegative()
    {
        var target = new AdditionalTimeoutInput { SuppliedInput = -1 };

        var exception = Should.Throw<InputException>(() => target.Validate());

        exception.Message.ShouldBe("Timeout cannot be negative.");
    }
}
