using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class ConfigurationInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new ConfigurationInput();
        target.HelpText.ShouldBe("Configuration to use when building the project(s).");
    }

    [TestMethod]
    public void ShouldReturnSuppliedInput()
    {
        var target = new ConfigurationInput { SuppliedInput = "Debug" };

        var result = target.Validate();

        result.ShouldBe("Debug");
    }

    [TestMethod]
    public void ShouldReturnDefault()
    {
        var target = new ConfigurationInput { SuppliedInput = null };

        var result = target.Validate();

        result.ShouldBeNull();
    }

    [TestMethod]
    public void ShouldThrowOnEmptyInput()
    {
        var target = new ConfigurationInput { SuppliedInput = "   " };

        var ex = Should.Throw<InputException>(() => target.Validate());

        ex.Message.ShouldBe("Please provide a non whitespace only configuration.");
    }
}
