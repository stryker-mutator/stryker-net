using System.IO.Abstractions.TestingHelpers;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class BasePathInputTests : TestBase
{
    [Fact]
    public void ShouldHaveHelpText()
    {
        var target = new BasePathInput();
        target.HelpText.ShouldBe(@$"The path from which stryker is started.");
    }

    [Fact]
    public void ShouldAllowExistingDir()
    {
        var target = new BasePathInput { SuppliedInput = "C:/MyDir/" };
        var fileSystemMock = new MockFileSystem();
        fileSystemMock.AddDirectory("C:/MyDir/");

        var result = target.Validate(fileSystemMock);

        result.ShouldBe("C:/MyDir/");
    }

    [Fact]
    public void ShouldThrowOnNonexistentDir()
    {
        var target = new BasePathInput { SuppliedInput = "C:/MyDir/" };
        var fileSystemMock = new MockFileSystem();

        var exception = Should.Throw<InputException>(() => target.Validate(fileSystemMock));

        exception.Message.ShouldBe("Base path must exist.");
    }

    [Fact]
    public void ShouldThrowOnNull()
    {
        var target = new BasePathInput { SuppliedInput = null };
        var fileSystemMock = new MockFileSystem();

        var exception = Should.Throw<InputException>(() => target.Validate(fileSystemMock));

        exception.Message.ShouldBe("Base path can't be null or empty.");
    }
}
