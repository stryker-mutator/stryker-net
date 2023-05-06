using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class ModuleNameInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new ModuleNameInput();
        target.HelpText.ShouldBe(@"Module name used by reporters. Usually a project in your solution would be a module. | default: ''");
    }

    [Fact]
    public void ShouldHaveDefault()
    {
        var target = new ModuleNameInput { SuppliedInput = null };

        var result = target.Validate();

        result.ShouldBe(string.Empty);
    }

    [Fact]
    public void ShouldReturnName()
    {
        var target = new ModuleNameInput { SuppliedInput = "TestName" };

        var result = target.Validate();

        result.ShouldBe("TestName");
    }

    [Fact]
    public void ShouldThrowOnNull()
    {
        var target = new ModuleNameInput { SuppliedInput = string.Empty };

        var exception = Should.Throw<InputException>(() => target.Validate());

        exception.Message.ShouldBe("Module name cannot be empty. Either fill the option or leave it out.");
    }
}
