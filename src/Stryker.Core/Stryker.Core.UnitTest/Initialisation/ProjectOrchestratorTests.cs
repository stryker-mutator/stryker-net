using System;
using System.Collections.Generic;
using System.Linq;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation;


public class ProjectOrchestratorTests : BuildAnalyzerTestsBase
{
    private readonly Mock<IMutationTestProcess> _mutationTestProcessMock = new(MockBehavior.Strict);
    private readonly Mock<IProjectMutator> _projectMutatorMock = new(MockBehavior.Strict);
    private readonly Mock<IReporter> _reporterMock = new(MockBehavior.Strict);

    public ProjectOrchestratorTests()
    {
        _mutationTestProcessMock.Setup(x => x.Mutate());
        _projectMutatorMock.Setup(x => x.MutateProject(It.IsAny<StrykerOptions>(),  It.IsAny<MutationTestInput>(), It.IsAny<IReporter>()))
            .Returns(   (StrykerOptions options, MutationTestInput input, IReporter reporter) =>
            {
                var mock= new Mock<IMutationTestProcess>();
                mock.Setup( m => m.Input).Returns(input);
                return mock.Object;
            });

    }

    [Fact]
    public void ShouldInitializeEachProjectInSolution()
    {
        // arrange
        // when a solutionPath is given, and it's inside the current directory (basePath)
        var testCsprojPathName = FileSystem.Path.Combine(ProjectPath, "testproject.csproj");
        var csprojPathName = FileSystem.Path.Combine(ProjectPath, "sourceproject.csproj");
        var options = new StrykerOptions
        {
            ProjectPath = FileSystem.Path.GetFullPath(testCsprojPathName),
            SolutionPath = FileSystem.Path.Combine(ProjectPath, "MySolution.sln")
        };

        var csPathName = FileSystem.Path.Combine(ProjectPath, "someFile.cs");
        var sourceProjectAnalyzerMock = SourceProjectAnalyzerMock(csprojPathName, new[] { csPathName }).Object;
        var target = BuildProjectOrchestratorForSimpleProject(sourceProjectAnalyzerMock, 
            TestProjectAnalyzerMock(testCsprojPathName, csprojPathName).Object, out var mockRunner);

        FileSystem.Directory.SetCurrentDirectory(FileSystem.Path.GetFullPath(testCsprojPathName));
            
        // act
        var result = target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();

        // assert
        result.ShouldHaveSingleItem();
    }

    [Fact]
    public void ShouldDiscoverContentFiles()
    {
        // arrange
        // when a solutionPath is given, and it's inside the current directory (basePath)
        var testCsprojPathName = FileSystem.Path.Combine(ProjectPath, "testproject.csproj");
        var csprojPathName = FileSystem.Path.Combine(ProjectPath, "sourceproject.csproj");
        var options = new StrykerOptions
        {
            ProjectPath = FileSystem.Path.GetFullPath(testCsprojPathName),
            SolutionPath = FileSystem.Path.Combine(ProjectPath, "MySolution.sln")
        };
        var contentFolder = FileSystem.Path.Combine(ProjectPath, "Contents");
        FileSystem.AddDirectory(contentFolder);
        var contentFile = FileSystem.Path.Combine(contentFolder, "SomeStuff.cs");
        FileSystem.AddFile(contentFile, "using System");
        var csPathName = FileSystem.Path.Combine(ProjectPath, "someFile.cs");

        var properties = GetSourceProjectDefaultProperties();
        properties["ContentPreprocessorOutputDirectory"] = contentFolder;
        var sourceProjectAnalyzerMock = BuildProjectAnalyzerMock(csprojPathName, new[] { csPathName }, properties).Object;
        var target = BuildProjectOrchestratorForSimpleProject(sourceProjectAnalyzerMock, 
            TestProjectAnalyzerMock(testCsprojPathName, csprojPathName).Object, out var mockRunner);

        FileSystem.Directory.SetCurrentDirectory(FileSystem.Path.GetFullPath(testCsprojPathName));
            
        // act
        var result = target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();

        // assert we see the content file
        var scan = ((ProjectComponent<SyntaxTree>)result.First().Input.SourceProjectInfo.ProjectContents).CompilationSyntaxTrees;
        scan.Count(f => f?.FilePath == contentFile).ShouldBe(1);
    }

    [Fact]
    public void ShouldPassWhenProjectNameIsGiven()
    {
        // arrange
        // when a solutionPath is given, and it's inside the current directory (basePath)
        var csprojPathName = FileSystem.Path.Combine(ProjectPath, "sourceproject.csproj");
        var testCsprojPathName = FileSystem.Path.Combine(ProjectPath, "test", "testproject.csproj");
        var options = new StrykerOptions
        {
            ProjectPath = FileSystem.Path.GetFullPath(testCsprojPathName),
            SourceProjectName = csprojPathName,
        };

        var csPathName = FileSystem.Path.Combine(ProjectPath, "someFile.cs");
        var sourceProjectAnalyzerMock = SourceProjectAnalyzerMock(csprojPathName, new[] { csPathName }).Object;
        var target = BuildProjectOrchestratorForSimpleProject(sourceProjectAnalyzerMock, 
            TestProjectAnalyzerMock(testCsprojPathName, csprojPathName).Object, out var mockRunner);

        // act
        var result = target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();

        // assert
        result.ShouldHaveSingleItem();
    }

    [Fact]
    public void ShouldRestoreWhenAnalysisFails()
    {
        // arrange
        // when a solutionPath is given, and it's inside the current directory (basePath)
        var csprojPathName = FileSystem.Path.Combine(ProjectPath, "sourceproject.csproj");
        var testCsprojPathName = FileSystem.Path.Combine(ProjectPath, "test", "testproject.csproj");
        var options = new StrykerOptions
        {
            ProjectPath = FileSystem.Path.GetFullPath(testCsprojPathName),
            SourceProjectName = csprojPathName,
            SolutionPath = FileSystem.Path.Combine(ProjectPath, "MySolution.sln")
        };

        var csPathName = FileSystem.Path.Combine(ProjectPath, "someFile.cs");
        var buildSuccess = false;
        var sourceProjectAnalyzerMock = SourceProjectAnalyzerMock(csprojPathName, new[] { csPathName }, framework:"net4.5", success: () => buildSuccess).Object;

        var testProjectAnalyzerMock = TestProjectAnalyzerMock(testCsprojPathName, csprojPathName, "net4.5" ).Object;
        // The analyzer finds two projects
        BuildBuildAnalyzerMock(new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectAnalyzerMock },
            { "MyProject.UnitTests", testProjectAnalyzerMock }
        });

        var mockRunner = new Mock<ITestRunner>();
        mockRunner.Setup(r => r.DiscoverTests(It.IsAny<string>())).Returns(true);
        mockRunner.Setup(r => r.GetTests(It.IsAny<IProjectAndTests>())).Returns(new TestSet());
        mockRunner.Setup(r => r.InitialTest(It.IsAny<IProjectAndTests>())).Returns(new TestRunResult(true));

        var initialBuildProcessMock = new Mock<IInitialBuildProcess>();
        var target = new ProjectOrchestrator(_projectMutatorMock.Object,
            initialBuildProcessMock.Object,
            new InputFileResolver(FileSystem, _buildalyzerProviderMock.Object));

        // act
        var result = target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();

        // assert
        result.ShouldHaveSingleItem();
    }

    [Fact]
    public void ShouldFailIfNoProjectFound()
    {
        var testCsprojPathName = FileSystem.Path.Combine(ProjectPath, "testproject.csproj");
        var csprojPathName = FileSystem.Path.Combine(ProjectPath, "sourceproject.csproj");
        var options = new StrykerOptions
        {
            ProjectPath = FileSystem.Path.GetFullPath(testCsprojPathName),
            SourceProjectName = "sourceprojec.csproj",
        };

        var csPathName = FileSystem.Path.Combine(ProjectPath, "someFile.cs");
        var sourceProjectAnalyzer = SourceProjectAnalyzerMock(csprojPathName, new[] { csPathName }).Object;
        var target = BuildProjectOrchestratorForSimpleProject(sourceProjectAnalyzer, 
            TestProjectAnalyzerMock(testCsprojPathName, csprojPathName).Object, out var mockRunner);
            
        FileSystem.Directory.SetCurrentDirectory(FileSystem.Path.GetFullPath(testCsprojPathName));
            
        // act
        var mutateAction = () => target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();

        // assert
        mutateAction.ShouldThrow<InputException>();
    }

    [Fact]
    public void ShouldFilterInSolutionMode()
    {
        // when a solutionPath is given and it's inside the current directory (basePath)
        var testCsprojPathName = FileSystem.Path.Combine(ProjectPath, "testproject.csproj");
        var csprojPathName = FileSystem.Path.Combine(ProjectPath, "sourceproject.csproj");
        var options = new StrykerOptions
        {
            ProjectPath = FileSystem.Path.GetFullPath(ProjectPath),
            // provide an invalid source project name which should normally fail
            SourceProjectName = "sourceprojec.csproj",
            SolutionPath = FileSystem.Path.Combine(ProjectPath, "MySolution.sln")
        };

        var csPathName = FileSystem.Path.Combine(ProjectPath, "someFile.cs");
        var sourceProjectAnalyzer = SourceProjectAnalyzerMock(csprojPathName, new[] { csPathName }).Object;
        var target = BuildProjectOrchestratorForSimpleProject(sourceProjectAnalyzer, 
            TestProjectAnalyzerMock(testCsprojPathName, csprojPathName).Object, out var mockRunner);
            
        FileSystem.Directory.SetCurrentDirectory(FileSystem.Path.GetFullPath(testCsprojPathName));
            
        // act
        var result = () => target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();
        // assert
        result.ShouldThrow<InputException>();
    }

    [Fact]
    public void ShouldProvideMinimalSupportForFSharp()
    {
        var testCsprojPathName = FileSystem.Path.Combine(ProjectPath, "testproject.fsproj");
        var csprojPathName = FileSystem.Path.Combine(ProjectPath, "sourceproject.fsproj");
        var options = new StrykerOptions
        {
            ProjectPath = FileSystem.Path.GetFullPath(testCsprojPathName),
            SolutionPath = FileSystem.Path.Combine(ProjectPath, "MySolution.sln")
        };

        var csPathName = FileSystem.Path.Combine(ProjectPath, "someFile.fs");
        var properties = new Dictionary<string, string>
            { { "IsTestProject", "False" }, { "ProjectTypeGuids", "not testproject" }, { "Language", "F#" } };
        var properties1 = new Dictionary<string, string>
            { { "IsTestProject", "True" }, { "Language", "F#" } };
        var sourceProjectAnalyzer = BuildProjectAnalyzerMock(csprojPathName, new[] { csPathName }, properties, null).Object;
        var target = BuildProjectOrchestratorForSimpleProject(
            sourceProjectAnalyzer,
            BuildProjectAnalyzerMock(testCsprojPathName, Array.Empty<string>(), properties1,
                new List<string>{ csprojPathName}).Object, out var mockRunner);
            
        FileSystem.Directory.SetCurrentDirectory(FileSystem.Path.GetFullPath(testCsprojPathName));
            
        // act
        var result = target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();

        // assert
        result.ShouldHaveSingleItem();
    }

    [Fact]
    public void ShouldDiscoverUpstreamProject()
    {
        // arrange
        var testCsprojPathName = FileSystem.Path.Combine(ProjectPath, "testproject.csproj");
        var csprojPathName = FileSystem.Path.Combine(ProjectPath, "sourceproject.csproj");
        var options = new StrykerOptions
        {
            ProjectPath = ProjectPath,
            SolutionPath = FileSystem.Path.Combine(ProjectPath, "MySolution.sln")
        };
        var libraryProject = FileSystem.Path.Combine(ProjectPath, "libraryproject.csproj");

        // The analyzer finds two projects
        var libraryAnalyzer = SourceProjectAnalyzerMock(libraryProject,
            new[] { FileSystem.Path.Combine(ProjectPath, "mylib.cs") }).Object;
        var projectAnalyzer = SourceProjectAnalyzerMock(csprojPathName, new[]
                { FileSystem.Path.Combine(ProjectPath, "someFile.cs")}
            , new[] {libraryProject}).Object;
        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", projectAnalyzer },
            { "MyLibrary", libraryAnalyzer },
            { "MyProject.UnitTests", TestProjectAnalyzerMock(testCsprojPathName, csprojPathName).Object }
        };
        var target = BuildProjectOrchestrator(analyzerResults, out var mockRunner);

        FileSystem.Directory.SetCurrentDirectory(FileSystem.Path.GetFullPath(testCsprojPathName));
            
        // act
        var result = target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();

        // assert
        result.Count.ShouldBe(2);
    }

    [Fact]
    public void ShouldDiscoverUpstreamProjectWithInvalidCasing()
    {
        // arrange
        var testCsprojPathName = FileSystem.Path.Combine(ProjectPath, "testproject.csproj");
        var csprojPathName = FileSystem.Path.Combine(ProjectPath, "sourceproject.csproj");
        var options = new StrykerOptions
        {
            ProjectPath = ProjectPath,
            SolutionPath = FileSystem.Path.Combine(ProjectPath, "MySolution.sln")
        };
        var libraryProject = FileSystem.Path.Combine(ProjectPath, "libraryproject.csproj");

        // The analyzer finds two projects
        var libraryAnalyzer = SourceProjectAnalyzerMock(libraryProject,
            new[] { FileSystem.Path.Combine(ProjectPath, "mylib.cs") }).Object;
        var projectAnalyzer = SourceProjectAnalyzerMock(csprojPathName, new[]
                { FileSystem.Path.Combine(ProjectPath, "someFile.cs")}
            , new[] {libraryProject}).Object;
        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", projectAnalyzer },
            { "MyLibrary", libraryAnalyzer },
            { "MyProject.UnitTests", TestProjectAnalyzerMock(testCsprojPathName, csprojPathName).Object }
        };
        var target = BuildProjectOrchestrator(analyzerResults, out var mockRunner);

        FileSystem.Directory.SetCurrentDirectory(FileSystem.Path.GetFullPath(testCsprojPathName));
        // act
        var result = target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();
        // assert
        result.Count.ShouldBe(2);
    }

    [Fact]
    public void ShouldDisregardInvalidReferences()
    {
        // arrange
        var testCsprojPathName = FileSystem.Path.Combine(ProjectPath, "testproject.csproj");
        var csprojPathName = FileSystem.Path.Combine(ProjectPath, "sourceproject.csproj");
        var options = new StrykerOptions
        {
            ProjectPath = ProjectPath,
            SolutionPath = FileSystem.Path.Combine(ProjectPath, "MySolution.sln")
        };
        var libraryProject = FileSystem.Path.Combine(ProjectPath, "libraryproject.csproj");
  
        // The analyzer finds two projects
        var projectAnalyzer = SourceProjectAnalyzerMock(csprojPathName, [FileSystem.Path.Combine(ProjectPath, "someFile.cs")]
            , new[] {libraryProject}).Object;
        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", projectAnalyzer },
            { "MyProject.UnitTests", TestProjectAnalyzerMock(testCsprojPathName, csprojPathName).Object }
        };
        var target = BuildProjectOrchestrator(analyzerResults, out var mockRunner);

        FileSystem.Directory.SetCurrentDirectory(FileSystem.Path.GetFullPath(testCsprojPathName));
        // act
        var result = target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();
        // assert
        result.Count.ShouldBe(1);
    }

    [Fact]
    public void ShouldIgnoreProjectWithoutTestAssemblies()
    {
        // arrange
        var testCsprojPathName = FileSystem.Path.Combine(ProjectPath, "testproject.csproj");
        var csprojPathName = FileSystem.Path.Combine(ProjectPath, "sourceproject.csproj");
        var options = new StrykerOptions
        {
            ProjectPath = FileSystem.Path.GetFullPath(testCsprojPathName),
            SolutionPath = FileSystem.Path.Combine(ProjectPath, "MySolution.sln")
        };
        var libraryProject = FileSystem.Path.Combine(ProjectPath, "libraryproject.csproj");

        // The analyzer finds two projects
        var projectAnalyzer = SourceProjectAnalyzerMock(csprojPathName, new[]
            { FileSystem.Path.Combine(ProjectPath, "someFile.cs")}).Object;
        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", projectAnalyzer },
            { "MyLibrary", SourceProjectAnalyzerMock(libraryProject,
                new[] { FileSystem.Path.Combine(ProjectPath, "mylib.cs") }).Object },
            { "MyProject.UnitTests", TestProjectAnalyzerMock(testCsprojPathName, csprojPathName).Object }
        };
        var target = BuildProjectOrchestrator(analyzerResults, out var mockRunner);

        FileSystem.Directory.SetCurrentDirectory(FileSystem.Path.GetFullPath(testCsprojPathName));
            
        // act
        var result = target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();

        // assert
        result.ShouldHaveSingleItem();
    }

    /// <summary>
    /// Build a project orchestrator for a single project with its single test project
    /// </summary>
    /// <param name="sourceProjectAnalyzerMock">code project analyzer</param>
    /// <param name="testProjectAnalyzerMock">test project analyzer</param>
    /// <param name="mockRunner"></param>
    /// <returns>A project orchestrator for the given projects</returns>
    private ProjectOrchestrator BuildProjectOrchestratorForSimpleProject(IProjectAnalyzer sourceProjectAnalyzerMock,
        IProjectAnalyzer testProjectAnalyzerMock, out Mock<ITestRunner> mockRunner)
    {
        // The analyzer finds two projects
        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectAnalyzerMock },
            { "MyProject.UnitTests", testProjectAnalyzerMock }
        };

        return BuildProjectOrchestrator(analyzerResults, out mockRunner);
    }

    /// <summary>
    /// Build a project orchestrator with a given set of analyzer results
    /// </summary>
    /// <param name="analyzerResults">analyzer results as a dictionary: name => result</param>
    /// <param name="mockRunner">a mock test runner that can be further customized to simulate desired test results.</param>
    /// <returns>A project orchestrator for the given projects</returns>
    private ProjectOrchestrator BuildProjectOrchestrator(Dictionary<string, IProjectAnalyzer> analyzerResults, out Mock<ITestRunner> mockRunner)
    {

        var buildalyzerAnalyzerManagerMock = BuildBuildAnalyzerMock(analyzerResults);

        mockRunner = new Mock<ITestRunner>();
        mockRunner.Setup(r => r.DiscoverTests(It.IsAny<string>())).Returns(true);
        mockRunner.Setup(r => r.GetTests(It.IsAny<IProjectAndTests>())).Returns(new TestSet());
        mockRunner.Setup(r => r.InitialTest(It.IsAny<IProjectAndTests>())).Returns(new TestRunResult(true));

        var initialBuildProcessMock = new Mock<IInitialBuildProcess>();
        return new ProjectOrchestrator(_projectMutatorMock.Object,
            initialBuildProcessMock.Object,
            new InputFileResolver(FileSystem, _buildalyzerProviderMock.Object));
    }
}
