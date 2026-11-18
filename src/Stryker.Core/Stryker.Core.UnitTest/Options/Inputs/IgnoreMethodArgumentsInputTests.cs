using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Configuration.Options.Inputs;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class IgnoreMethodArgumentsInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new IgnoreMethodArgumentsInput();
        target.HelpText.ShouldBe(@"Ignore mutations in the arguments of the given method calls, but not in the rest of the call. | default: []");
    }

    [TestMethod]
    public void ShouldReturnTheSameRegexAsIgnoreMethods()
    {
        var target = new IgnoreMethodArgumentsInput { SuppliedInput = new[] { "ConfigureAwait", "Console.Write*" } };

        var result = target.Validate();

        result.Select(r => r.ToString()).ShouldBe(
            new IgnoreMethodsInput { SuppliedInput = new[] { "ConfigureAwait", "Console.Write*" } }.Validate().Select(r => r.ToString()));
    }

    [TestMethod]
    public void ShouldHaveDefault()
    {
        var target = new IgnoreMethodArgumentsInput { SuppliedInput = null };

        var result = target.Validate();

        result.ShouldBeEmpty();
    }
}
