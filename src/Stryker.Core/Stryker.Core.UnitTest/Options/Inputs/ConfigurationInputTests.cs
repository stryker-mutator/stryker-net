using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;
public class ConfigurationInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new ConfigurationInput();
        target.HelpText.ShouldBe("Configuration to use when building the project(s).");
    }

    [Fact]
    public void ShouldReturnSuppliedInput()
    {
        var target = new ConfigurationInput { SuppliedInput = "Debug" };

        var result = target.Validate();

        result.ShouldBe("Debug");
    }

    [Fact]
    public void ShouldReturnDefault()
    {
        var target = new ConfigurationInput { SuppliedInput = null };

        var result = target.Validate();

        result.ShouldBeNull();
    }

    [Fact]
    public void ShouldThrowOnEmptyInput()
    {
        var target = new ConfigurationInput { SuppliedInput = "   " };

        var ex = Assert.Throws<InputException>(() => target.Validate());

        ex.Message.ShouldBe("Please provide a non whitespace only configuration.");
    }
}
