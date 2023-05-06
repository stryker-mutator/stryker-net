using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class ProjectNameInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new ProjectNameInput();
        target.HelpText.ShouldBe(@"The organizational name for your project. Required when dashboard reporter is turned on.
For example: Your project might be called 'consumer-loans' and it might contains sub-modules 'consumer-loans-frontend' and 'consumer-loans-backend'. | default: ''");
    }

    [Fact]
    public void ShouldReturnName()
    {
        var input = new ProjectNameInput { SuppliedInput = "name" };

        var result = input.Validate();

        result.ShouldBe("name");
    }

    [Fact]
    public void ShouldHaveDefault()
    {
        var input = new ProjectNameInput { SuppliedInput = null };

        var result = input.Validate();

        result.ShouldBe(string.Empty);
    }
}
