using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class TargetFrameworkInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new TargetFrameworkInput();
        target.HelpText.ShouldBe("The framework to build the project against.");
    }

    [Fact]
    public void ShouldHaveDefaultNull()
    {
        var target = new TargetFrameworkInput { SuppliedInput = null };

        var result = target.Validate();

        result.ShouldBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void ShouldThrowOnEmptyInput(string input)
    {
        var target = new TargetFrameworkInput { SuppliedInput = input };

        var exception = Should.Throw<InputException>(() => target.Validate());

        exception.Message.ShouldBe(
            "Target framework cannot be empty. " +
            "Please provide a valid value from this list: " +
            "https://docs.microsoft.com/en-us/dotnet/standard/frameworks");
    }

    [Fact]
    public void ShouldReturnFramework()
    {
        var target = new TargetFrameworkInput { SuppliedInput = "netcoreapp3.1" };

        var result = target.Validate();

        result.ShouldBe("netcoreapp3.1");
    }
}
