using System.IO.Abstractions.TestingHelpers;
using Buildalyzer;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Fsharp.ProjectComponents;

namespace Stryker.Fsharp.UnitTest;

[TestClass]
public class FsharpProjectComponentsBuilderTests
{
    private const string ProjectDir = "/proj";
    private const string Valid = "module M\nlet x = 1\n";
    private const string Invalid = "let let = = (";

    private static (FsharpProjectComponentsBuilder Builder, Mock<ILogger> Logger) CreateBuilder(
        Dictionary<string, string> files, params string[] compileItems)
    {
        var fileSystem = new MockFileSystem(files.ToDictionary(f => f.Key, f => new MockFileData(f.Value)));
        var analyzerResult = new Mock<IAnalyzerResult>();
        analyzerResult.Setup(x => x.ProjectFilePath).Returns($"{ProjectDir}/proj.fsproj");
        analyzerResult.Setup(x => x.SourceFiles).Returns(compileItems);
        analyzerResult.Setup(x => x.PreprocessorSymbols).Returns([]);
        var logger = new Mock<ILogger>();
        logger.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        var info = new SourceProjectInfo(analyzerResult.Object, null);
        return (new FsharpProjectComponentsBuilder(info, logger.Object, fileSystem), logger);
    }

    private static void VerifyLogged(Mock<ILogger> logger, LogLevel level) =>
        logger.Verify(x => x.Log(level, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.AtLeastOnce);

    [TestMethod]
    public void FsharpComponentsBuilder_BuildsLeaves_InCompileItemOrder()
    {
        // directory enumeration would yield A.fs first; compile items say B.fs first
        var (builder, _) = CreateBuilder(new()
        {
            [$"{ProjectDir}/A.fs"] = "module A\nlet a = 1\n",
            [$"{ProjectDir}/B.fs"] = "module B\nlet b = 1\n",
        }, $"{ProjectDir}/B.fs", $"{ProjectDir}/A.fs");

        var leaves = builder.Build().GetAllFiles().Cast<FsharpFileLeaf>().ToList();

        leaves.Select(l => l.RelativePath).ShouldBe(["B.fs", "A.fs"]);
        leaves.ShouldAllBe(l => l.ParseTree != null && l.SourceCode.Length > 0);
    }

    [TestMethod]
    public void FsharpComponentsBuilder_RelativeCompileItems_AreResolved_AndGeneratedFilesExcluded()
    {
        // Buildalyzer reports project-relative paths and includes generated files from obj/
        var (builder, _) = CreateBuilder(new()
        {
            [$"{ProjectDir}/A.fs"] = "module A\nlet a = 1\n",
            [$"{ProjectDir}/obj/Debug/net10.0/Lib.AssemblyInfo.fs"] = "module Info\nlet i = 1\n",
            [$"{ProjectDir}/obj/Debug/net10.0/.NETCoreApp,Version=v10.0.AssemblyAttributes.fs"] = "module Attr\nlet i = 1\n",
        }, "A.fs", "obj/Debug/net10.0/Lib.AssemblyInfo.fs", "obj/Debug/net10.0/.NETCoreApp,Version=v10.0.AssemblyAttributes.fs");

        var leaves = builder.Build().GetAllFiles().Cast<FsharpFileLeaf>().ToList();

        leaves.Select(l => l.RelativePath).ShouldBe(["A.fs"]);
        leaves.Single().FullPath.ShouldBe($"{ProjectDir}/A.fs");
    }

    [TestMethod]
    public void FsharpComponentsBuilder_SkipsUnparseableFile_WithWarning()
    {
        var (builder, logger) = CreateBuilder(new()
        {
            [$"{ProjectDir}/A.fs"] = Valid,
            [$"{ProjectDir}/Bad.fs"] = Invalid,
            [$"{ProjectDir}/C.fs"] = "module C\nlet c = 1\n",
        }, $"{ProjectDir}/A.fs", $"{ProjectDir}/Bad.fs", $"{ProjectDir}/C.fs");

        var leaves = builder.Build().GetAllFiles().Cast<FsharpFileLeaf>().ToList();

        leaves.Select(l => l.RelativePath).ShouldBe(["A.fs", "C.fs"]);
        VerifyLogged(logger, LogLevel.Warning);
    }

    [TestMethod]
    public void FsharpComponentsBuilder_ExcludesSignatureAndScriptItems_WithDebugLog()
    {
        var (builder, logger) = CreateBuilder(new()
        {
            [$"{ProjectDir}/A.fsi"] = "module A\nval a : int\n",
            [$"{ProjectDir}/A.fs"] = "module A\nlet a = 1\n",
            [$"{ProjectDir}/S.fsx"] = "let s = 1\n",
        }, $"{ProjectDir}/A.fsi", $"{ProjectDir}/A.fs", $"{ProjectDir}/S.fsx");

        var leaves = builder.Build().GetAllFiles().Cast<FsharpFileLeaf>().ToList();

        leaves.Select(l => l.RelativePath).ShouldBe(["A.fs"]);
        VerifyLogged(logger, LogLevel.Debug);
    }
}
