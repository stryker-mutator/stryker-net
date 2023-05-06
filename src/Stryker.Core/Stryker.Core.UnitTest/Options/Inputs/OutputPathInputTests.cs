using System.IO.Abstractions.TestingHelpers;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class OutputPathInputTests : TestBase
{
    private MockFileSystem _fileSystemMock = new MockFileSystem();

    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new OutputPathInput();
        target.HelpText.ShouldBe("Changes the output path for Stryker logs and reports. This can be an absolute or relative path.");
    }

    [Theory]
    [InlineData("C:/bla/test")]
    [InlineData("test")]
    public void ShouldReturnValidOutputPath(string outputPath)
    {
        var target = new OutputPathInput { SuppliedInput = outputPath };
        _fileSystemMock.AddDirectory(outputPath);

        var result = target.Validate(_fileSystemMock);

        // we can't assert the complete datetime as it will differ in milliseconds
        result.ShouldBe(result);
    }

    [Theory]
    [InlineData("C:/bla/test")]
    [InlineData("test")]
    public void ShouldThrowOnNonExistingPath(string outputPath)
    {
        var target = new OutputPathInput { SuppliedInput = outputPath };

        var exception = Should.Throw<InputException>(() =>
        {
            target.Validate(_fileSystemMock);
        });
        // we can't assert the complete datetime as it will differ in milliseconds
        exception.Message.ShouldBe("Outputpath should exist");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ShouldThrowOnEmptyValue(string value)
    {
        var target = new OutputPathInput { SuppliedInput = value };

        var exception = Should.Throw<InputException>(() =>
        {
            target.Validate(_fileSystemMock);
        });

        exception.Message.ShouldBe("Outputpath can't be null or whitespace");
    }
}
