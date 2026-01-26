using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Exceptions;
using Stryker.Configuration.Options.Inputs;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class ConfigurationInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new ConfigurationInput();
        target.HelpText.ShouldBe("Configuration to use when building the project(s) (e.g., 'Debug' or 'Release'). If not specified, the default configuration of the project(s) will be used.");
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
}
