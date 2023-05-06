using Shouldly;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class TestCaseFilterInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var input = new TestCaseFilterInput();
        input.HelpText.ShouldBe(@"Filters out tests in the project using the given expression.
Uses the syntax for dotnet test --filter option and vstest.console.exe --testcasefilter option.
For more information on running selective tests, see https://docs.microsoft.com/en-us/dotnet/core/testing/selective-unit-tests. | default: ''");
    }

    [Fact]
    public void DefaultShouldBeEmpty()
    {
        var input = new TestCaseFilterInput();
        input.Default.ShouldBeEmpty();
    }

    [Fact]
    public void ShouldReturnSuppliedInputWhenNotNullOrWhiteSpace()
    {
        var input = new TestCaseFilterInput { SuppliedInput = "Category=Unit" };
        input.Validate().ShouldBe("Category=Unit");
    }

    [Fact]
    public void ShouldReturnDefaultWhenSuppliedInputNull()
    {
        var input = new TestCaseFilterInput { SuppliedInput = null };
        input.Validate().ShouldBe("");
    }

    [Fact]
    public void ShouldReturnDefaultWhenSuppliedInputWhiteSpace()
    {
        var input = new TestCaseFilterInput { SuppliedInput = "    " };
        input.Validate().ShouldBe("");
    }
}
