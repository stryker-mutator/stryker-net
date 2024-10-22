using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Options;
using Stryker.CLI.Logging;

namespace Stryker.CLI.UnitTest;

[TestClass]
public class InputBuilderTests
{
    [TestMethod]
    public void ShouldAddGitIgnore()
    {
        var fileSystemMock = new MockFileSystem();
        var basePath = Directory.GetCurrentDirectory();
        var target = new LoggingInitializer();

        var inputs = new StrykerInputs();
        inputs.BasePathInput.SuppliedInput = basePath;
        target.SetupLogOptions(inputs, fileSystemMock);

        var gitIgnoreFile =
            fileSystemMock.AllFiles.Single(x => x.EndsWith(Path.Combine(".gitignore")));
        gitIgnoreFile.ShouldNotBeNull();
        DateTime.TryParse(Directory.GetParent(gitIgnoreFile)!.Name.Split(".")[0], out _).ShouldBeTrue();
        var fileContents = fileSystemMock.GetFile(gitIgnoreFile).Contents;
        Encoding.Default.GetString(fileContents).ShouldBe("*");
    }

    [TestMethod]
    public void ShouldAddGitIgnoreWithAbsolutePath()
    {
        var fileSystemMock = new MockFileSystem();
        var target = new LoggingInitializer();

        var inputs = new StrykerInputs();
        inputs.OutputPathInput.SuppliedInput = Path.Combine(Path.GetPathRoot(Directory.GetCurrentDirectory())!, "tmp", "path");
        target.SetupLogOptions(inputs, fileSystemMock);

        var gitIgnoreFile =
            fileSystemMock.AllFiles.FirstOrDefault(x => x.EndsWith(Path.Combine("tmp", "path", ".gitignore")));
        gitIgnoreFile.ShouldNotBeNull();
        var fileContents = fileSystemMock.GetFile(gitIgnoreFile).Contents;
        Encoding.Default.GetString(fileContents).ShouldBe("*");
    }

    [TestMethod]
    public void ShouldAddGitIgnoreWithRelativePath()
    {
        var fileSystemMock = new MockFileSystem();
        var basePath = Directory.GetCurrentDirectory();
        var target = new LoggingInitializer();

        var inputs = new StrykerInputs();
        inputs.BasePathInput.SuppliedInput = basePath;
        inputs.OutputPathInput.SuppliedInput = "output";
        target.SetupLogOptions(inputs, fileSystemMock);

        var gitIgnoreFile =
            fileSystemMock.AllFiles.FirstOrDefault(x => x.EndsWith(Path.Combine("output", ".gitignore")));
        gitIgnoreFile.ShouldNotBeNull();
        var fileContents = fileSystemMock.GetFile(gitIgnoreFile).Contents;
        Encoding.Default.GetString(fileContents).ShouldBe("*");
    }
}
