using System.Linq;
using Shouldly;
using Stryker.Abstractions.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class IgnoreMethodsInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new IgnoreMethodsInput();
        target.HelpText.ShouldBe(@"Ignore mutations on method parameters. | default: []");
    }

    [TestMethod]
    public void ShouldReturnRegex()
    {
        var target = new IgnoreMethodsInput { SuppliedInput = new[] { "Dispose" } };

        var result = target.Validate();

        result.ShouldHaveSingleItem().ToString().ShouldBe(@"^(?:[^.]*\.)*Dispose(<[^>]*>)?$");
    }

    [TestMethod]
    public void ShouldReturnMultipleItems()
    {
        var target = new IgnoreMethodsInput { SuppliedInput = new[] { "Dispose", "Test" } };

        var result = target.Validate();

        result.Count().ShouldBe(2);
        result.First().ToString().ShouldBe(@"^(?:[^.]*\.)*Dispose(<[^>]*>)?$");
        result.Last().ToString().ShouldBe(@"^(?:[^.]*\.)*Test(<[^>]*>)?$");
    }

    [TestMethod]
    public void ShouldHaveDefault()
    {
        var target = new IgnoreMethodsInput { SuppliedInput = null };

        var result = target.Validate();

        result.ShouldBeEmpty();
    }
}
