using System.IO.Abstractions.TestingHelpers;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class MsBuildPathInputTests : TestBase
{
    private readonly MockFileSystem _fileSystemMock = new();

    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new MsBuildPathInput();
        target.HelpText.ShouldBe("The path to the msbuild executable to use to build your .NET Framework application. Not used for .net (core).");
    }

    [Fact]
    public void ShouldReturnValidMsBuildPath()
    {
        var path = "C:/bla/test.exe";
        var target = new MsBuildPathInput { SuppliedInput = path };
        _fileSystemMock.AddFile(path, new(""));

        var result = target.Validate(_fileSystemMock);
        result.ShouldBe(result);
    }

    [Theory]
    [InlineData("C:/bla/test")]
    [InlineData("test")]
    public void ShouldThrowOnNonExistingPath(string path)
    {
        var target = new MsBuildPathInput { SuppliedInput = path };

        var exception = Should.Throw<InputException>(() => target.Validate(_fileSystemMock));
        exception.Message.ShouldBe($"Given MsBuild path '{path}' does not exist. Either provide a valid msbuild path or let stryker locate msbuild automatically.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ShouldThrowOnEmptyValue(string value)
    {
        var target = new MsBuildPathInput { SuppliedInput = value };

        var exception = Should.Throw<InputException>(() => target.Validate(_fileSystemMock));

        exception.Message.ShouldBe("MsBuild path cannot be empty. Either provide a valid msbuild path or let stryker locate msbuild automatically.");
    }
}
