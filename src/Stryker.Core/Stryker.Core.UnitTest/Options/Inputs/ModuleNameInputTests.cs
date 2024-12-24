using Shouldly;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class ModuleNameInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new ModuleNameInput();
        target.HelpText.ShouldBe(@"Module name used by reporters. Usually a project in your solution would be a module. | default: ''");
    }

    [TestMethod]
    public void ShouldHaveDefault()
    {
        var target = new ModuleNameInput { SuppliedInput = null };

        var result = target.Validate();

        result.ShouldBe(string.Empty);
    }

    [TestMethod]
    public void ShouldReturnName()
    {
        var target = new ModuleNameInput { SuppliedInput = "TestName" };

        var result = target.Validate();

        result.ShouldBe("TestName");
    }

    [TestMethod]
    public void ShouldThrowOnNull()
    {
        var target = new ModuleNameInput { SuppliedInput = string.Empty };

        var exception = Should.Throw<InputException>(() => target.Validate());

        exception.Message.ShouldBe("Module name cannot be empty. Either fill the option or leave it out.");
    }
}
