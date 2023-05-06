using System.IO;
using Shouldly;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class TestProjectsInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new TestProjectsInput();
        target.HelpText.ShouldBe(@"Specify the test projects. | default: []");
    }

    [Fact]
    public void ShouldUseDefaultWhenNull()
    {
        var input = new TestProjectsInput { SuppliedInput = null };

        input.Validate().ShouldBeEmpty();
    }

    [Fact]
    public void ShouldIgnoreEmptyString()
    {
        var input = new TestProjectsInput { SuppliedInput = new[] { "", "", "" } };

        input.Validate().ShouldBeEmpty();
    }

    [Fact]
    public void ShouldNormalizePaths()
    {
        var paths = new[] { "/c/root/bla/test.csproj" };
        var expected = new[] { Path.GetFullPath(FilePathUtils.NormalizePathSeparators(paths[0])) };
        var input = new TestProjectsInput { SuppliedInput = paths };

        input.Validate().ShouldBe(expected);
    }
}
