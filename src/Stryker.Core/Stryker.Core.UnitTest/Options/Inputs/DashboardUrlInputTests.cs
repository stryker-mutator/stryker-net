using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class DashboardUrlInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new DashboardUrlInput();
        target.HelpText.ShouldBe(@"Alternative url for Stryker Dashboard. | default: 'https://dashboard.stryker-mutator.io'");
    }

    [Fact]
    public void ShouldHaveDefault()
    {
        var target = new DashboardUrlInput { SuppliedInput = null };

        var defaultValue = target.Validate();

        defaultValue.ShouldBe("https://dashboard.stryker-mutator.io");
    }
    
    [Fact]
    public void ShouldAllowValidUri()
    {
        var target = new DashboardUrlInput { SuppliedInput = "http://example.com:8042" };

        var defaultValue = target.Validate();

        defaultValue.ShouldBe("http://example.com:8042");
    }

    [Fact]
    public void ShouldThrowOnInvalidUri()
    {
        var target = new DashboardUrlInput { SuppliedInput = "test" };

        var exception = Should.Throw<InputException>(() => target.Validate());

        exception.Message.ShouldBe("Stryker dashboard url 'test' is invalid.");
    }
}
