using System.Linq;
using Shouldly;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class IgnoreMethodsInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new IgnoreMethodsInput();
        target.HelpText.ShouldBe(@"Ignore mutations on method parameters. | default: []");
    }

    [Fact]
    public void ShouldReturnRegex()
    {
        var target = new IgnoreMethodsInput { SuppliedInput = new[] { "Dispose" } };

        var result = target.Validate();

        result.ShouldHaveSingleItem().ToString().ShouldBe(@"^(?:[^.]*\.)*Dispose$");
    }

    [Fact]
    public void ShouldReturnMultipleItems()
    {
        var target = new IgnoreMethodsInput { SuppliedInput = new[] { "Dispose", "Test" } };

        var result = target.Validate();

        result.Count().ShouldBe(2);
        result.First().ToString().ShouldBe(@"^(?:[^.]*\.)*Dispose$");
        result.Last().ToString().ShouldBe(@"^(?:[^.]*\.)*Test$");
    }

    [Fact]
    public void ShouldHaveDefault()
    {
        var target = new IgnoreMethodsInput { SuppliedInput = null };

        var result = target.Validate();

        result.ShouldBeEmpty();
    }
}
