using System.IO;
using System.Linq;
using Shouldly;
using Stryker.Abstractions.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class MutateInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new MutateInput();
        target.HelpText.ShouldBe(@"Allows to specify file that should in- or excluded for the mutations.
    Use glob syntax for wildcards: https://en.wikipedia.org/wiki/Glob_(programming)
    Use '!' at the start of a pattern to exclude all matched files.
    Use '{<start>..<end>}' at the end of a pattern to specify spans of text in files to in- or exclude.
    Example: ['**/*Service.cs','!**/MySpecialService.cs', '**/MyOtherService.cs{1..10}{32..45}'] | default: ['**/*']");
    }

    [TestMethod]
    public void ShouldHaveDefault()
    {
        var target = new MutateInput { SuppliedInput = new string[] { } };

        var result = target.Validate();

        var item = result.ShouldHaveSingleItem();
        item.Glob.ToString().ShouldBe(Path.Combine("**", "*"));
        item.IsExclude.ShouldBeFalse();
    }

    [TestMethod]
    public void ShouldReturnFiles()
    {
        var target = new MutateInput { SuppliedInput = new[] { Path.Combine("**", "*.cs") } };

        var result = target.Validate();

        var item = result.ShouldHaveSingleItem();
        item.Glob.ToString().ShouldBe(Path.Combine("**", "*.cs"));
        item.IsExclude.ShouldBeFalse();
    }

    [TestMethod]
    public void ShouldExcludeAll()
    {
        var target = new MutateInput { SuppliedInput = new[] { "!" + Path.Combine("**", "Test.cs") } };

        var result = target.Validate();

        result.Count().ShouldBe(2);
        result.First().Glob.ToString().ShouldBe(Path.Combine("**", "Test.cs"));
        result.First().IsExclude.ShouldBeTrue();
        result.Last().Glob.ToString().ShouldBe(Path.Combine("**", "*"));
        result.Last().IsExclude.ShouldBeFalse();
    }

}
