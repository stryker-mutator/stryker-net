using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using Buildalyzer;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Configuration.Options;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.Csharp;
using Stryker.Core.ProjectComponents.SourceProjects;

namespace Stryker.Core.UnitTest.MutationTest;

[TestClass]
public class AutoIgnoreTimeoutsProcessTests : TestBase
{
    private const string FilePath = "/project/File.cs";
    private const string Source = "class C\n{\n    int M(int x)\n    {\n        return x + 1;\n    }\n}\n";

    private readonly Mock<IInitialBuildProcess> _buildProcess = new(MockBehavior.Strict);
    private readonly MockFileSystem _fileSystem = new();
    private readonly StrykerOptions _options = new();

    private AutoIgnoreTimeoutsProcess CreateTarget() =>
        new(_fileSystem, _buildProcess.Object, TestLoggerFactory.CreateLogger<AutoIgnoreTimeoutsProcess>());

    private static SourceProjectInfo CreateProject(string source, MutantStatus status, out Mutant mutant)
    {
        var tree = CSharpSyntaxTree.ParseText(source, path: FilePath);
        var node = tree.GetRoot().DescendantNodes().First(n => n.ToString() == "x + 1");
        mutant = new Mutant { Id = 1, ResultStatus = status, Mutation = new Mutation { OriginalNode = node, Type = Mutator.Arithmetic } };

        var file = new CsharpFileLeaf { FullPath = FilePath, SourceCode = source, SyntaxTree = tree, Mutants = [mutant] };
        var folder = new FolderComposite();
        folder.Add(file);

        var analyzerResult = new Mock<IAnalyzerResult>();
        analyzerResult.SetupGet(a => a.ProjectFilePath).Returns("/project/project.csproj");
        analyzerResult.SetupGet(a => a.Properties).Returns(new Dictionary<string, string>());
        return new SourceProjectInfo(analyzerResult.Object, null) { ProjectContents = folder };
    }

    private void SetupBuild(bool succeeds)
    {
        var setup = _buildProcess.Setup(b => b.InitialBuild(false, "/project/project.csproj", null, null, null, null, null));
        if (succeeds)
        {
            setup.Verifiable();
        }
        else
        {
            setup.Throws(new InputException("build failed"));
        }
    }

    [TestMethod]
    public void ShouldAddCommentAndBuildWhenMutantTimedOut()
    {
        _fileSystem.AddFile(FilePath, new MockFileData(Source));
        var project = CreateProject(Source, MutantStatus.Timeout, out _);
        SetupBuild(true);

        CreateTarget().IgnoreTimeouts([project], _options);

        var content = _fileSystem.File.ReadAllText(FilePath);
        content.ShouldContain("// Stryker disable once Arithmetic");
        CSharpSyntaxTree.ParseText(content).GetDiagnostics().ShouldBeEmpty();
        _buildProcess.VerifyAll();
    }

    [TestMethod]
    public void ShouldRevertChangesWhenBuildFails()
    {
        _fileSystem.AddFile(FilePath, new MockFileData(Source));
        var project = CreateProject(Source, MutantStatus.Timeout, out _);
        SetupBuild(false);

        CreateTarget().IgnoreTimeouts([project], _options);

        _fileSystem.File.ReadAllText(FilePath).ShouldBe(Source);
    }

    [TestMethod]
    [DataRow(MutantStatus.Killed)]
    [DataRow(MutantStatus.Survived)]
    [DataRow(MutantStatus.Ignored)]
    public void ShouldNotTouchFilesWithoutTimeouts(MutantStatus status)
    {
        _fileSystem.AddFile(FilePath, new MockFileData(Source));
        var project = CreateProject(Source, status, out _);

        // strict mock: any build would throw
        CreateTarget().IgnoreTimeouts([project], _options);

        _fileSystem.File.ReadAllText(FilePath).ShouldBe(Source);
    }

    [TestMethod]
    public void ShouldNotOverwriteFileChangedSinceAnalysis()
    {
        var changed = Source + "// edited\n";
        _fileSystem.AddFile(FilePath, new MockFileData(changed));
        var project = CreateProject(Source, MutantStatus.Timeout, out _);

        CreateTarget().IgnoreTimeouts([project], _options);

        _fileSystem.File.ReadAllText(FilePath).ShouldBe(changed);
    }

    [TestMethod]
    public void ShouldPreserveByteOrderMark()
    {
        var bytes = new UTF8Encoding(true).GetPreamble().Concat(Encoding.UTF8.GetBytes(Source)).ToArray();
        _fileSystem.AddFile(FilePath, new MockFileData(bytes));
        var project = CreateProject(Source, MutantStatus.Timeout, out _);
        SetupBuild(true);

        CreateTarget().IgnoreTimeouts([project], _options);

        _fileSystem.File.ReadAllBytes(FilePath).Take(3).ShouldBe(new UTF8Encoding(true).GetPreamble());
    }

    [TestMethod]
    public void ShouldNotAddBomWhenFileHadNone()
    {
        _fileSystem.AddFile(FilePath, new MockFileData(Encoding.UTF8.GetBytes(Source)));
        var project = CreateProject(Source, MutantStatus.Timeout, out _);
        SetupBuild(true);

        CreateTarget().IgnoreTimeouts([project], _options);

        _fileSystem.File.ReadAllBytes(FilePath).Take(3).ShouldNotBe(new UTF8Encoding(true).GetPreamble());
    }
}
