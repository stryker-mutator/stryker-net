using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.CLI.Logging;
using Stryker.Configuration.Options;

namespace Stryker.CLI.UnitTest.Logging;

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
        // the gitignore lives at the stable output root, not the per-run timestamped folder
        Directory.GetParent(gitIgnoreFile)!.Name.ShouldBe("StrykerOutput");
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

    [TestMethod]
    public void ShouldSetBaselineOutputToStableRoot()
    {
        var fileSystemMock = new MockFileSystem();
        var basePath = Directory.GetCurrentDirectory();
        var target = new LoggingInitializer();

        var inputs = new StrykerInputs();
        inputs.BasePathInput.SuppliedInput = basePath;
        target.SetupLogOptions(inputs, fileSystemMock);

        // the baseline follows the stable output root, not the per-run timestamped output path
        inputs.BaselineOutputInput.SuppliedInput.ShouldBe(Path.Combine(basePath, "StrykerOutput"));
        inputs.OutputPathInput.SuppliedInput.ShouldStartWith(Path.Combine(basePath, "StrykerOutput") + Path.DirectorySeparatorChar);
    }

    [TestMethod]
    public void ShouldSetBaselineOutputToSuppliedOutputPath()
    {
        var fileSystemMock = new MockFileSystem();
        var basePath = Directory.GetCurrentDirectory();
        var target = new LoggingInitializer();

        var inputs = new StrykerInputs();
        inputs.BasePathInput.SuppliedInput = basePath;
        inputs.OutputPathInput.SuppliedInput = "output";
        target.SetupLogOptions(inputs, fileSystemMock);

        // an explicit output path has no timestamp subfolder, so it is the root itself
        inputs.BaselineOutputInput.SuppliedInput.ShouldBe(Path.Combine(basePath, "output"));
    }
}
