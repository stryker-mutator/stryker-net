using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using Buildalyzer;
using Buildalyzer.Construction;
using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Stryker.Core.Testing;
using Stryker.Core.TestRunners;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class ProjectOrchestratorTests : TestBase
    {
        private readonly Mock<IBuildalyzerProvider> _buildalyzerProviderMock = new(MockBehavior.Strict);
        private readonly Mock<IMutationTestProcess> _mutationTestProcessMock = new(MockBehavior.Strict);
        private readonly Mock<IProjectMutator> _projectMutatorMock = new(MockBehavior.Strict);
        private readonly Mock<IReporter> _reporterMock = new(MockBehavior.Strict);
        private readonly MockFileSystem _fileSystem = new();
        private readonly string _projectPath;

        public ProjectOrchestratorTests()
        {
            _mutationTestProcessMock.Setup(x => x.Mutate());
            _projectMutatorMock.Setup(x => x.MutateProject(It.IsAny<StrykerOptions>(),  It.IsAny<MutationTestInput>(), It.IsAny<IReporter>()))
                .Returns(new Mock<IMutationTestProcess>().Object);
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filesystemRoot = Path.GetPathRoot(currentDirectory);

            _projectPath = _fileSystem.Path.Combine(filesystemRoot, "sourceproject");
        }

        [Fact]
        public void ShouldInitializeEachProjectInSolution()
        {
            // arrange
            // when a solutionPath is given and it's inside the current directory (basePath)
            var testCsprojPathName = _fileSystem.Path.Combine(_projectPath, "testproject.csproj");
            var csprojPathName = _fileSystem.Path.Combine(_projectPath, "sourceproject.csproj");
            var options = new StrykerOptions
            {
                ProjectPath = _fileSystem.Path.GetFullPath(testCsprojPathName),
                SolutionPath = _fileSystem.Path.Combine(_projectPath, "MySolution.sln")
            };

            var csPathName = _fileSystem.Path.Combine(_projectPath, "someFile.cs");
            var target = BuildProjectOrchestratorForSimpleProject(SourceProjectAnalyzerMock(csprojPathName, new[] { csPathName }).Object, 
                TestProjectAnalyzerMock(testCsprojPathName, csprojPathName).Object, out var mockRunner);

            _fileSystem.Directory.SetCurrentDirectory(_fileSystem.Path.GetFullPath(testCsprojPathName));
            
            // act
            var result = target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();

            // assert
            result.ShouldHaveSingleItem();
        }

        [Fact]
        public void ShouldPassWhenProjectNameIsGiven()
        {
            // arrange
            // when a solutionPath is given, and it's inside the current directory (basePath)
            var csprojPathName = _fileSystem.Path.Combine(_projectPath, "sourceproject.csproj");
            var testCsprojPathName = _fileSystem.Path.Combine(_projectPath, "test", "testproject.csproj");
            var options = new StrykerOptions
            {
                ProjectPath = _fileSystem.Path.GetFullPath(testCsprojPathName),
                SourceProjectName = csprojPathName,
            };

            var csPathName = _fileSystem.Path.Combine(_projectPath, "someFile.cs");
            var target = BuildProjectOrchestratorForSimpleProject(SourceProjectAnalyzerMock(csprojPathName, new[] { csPathName }).Object, 
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
            var csprojPathName = _fileSystem.Path.Combine(_projectPath, "sourceproject.csproj");
            var testCsprojPathName = _fileSystem.Path.Combine(_projectPath, "test", "testproject.csproj");
            var options = new StrykerOptions
            {
                ProjectPath = _fileSystem.Path.GetFullPath(testCsprojPathName),
                SourceProjectName = csprojPathName,
                SolutionPath = _fileSystem.Path.Combine(_projectPath, "MySolution.sln")
            };

            var csPathName = _fileSystem.Path.Combine(_projectPath, "someFile.cs");
            var target = BuildProjectOrchestratorForSimpleProject(SourceProjectAnalyzerMock(csprojPathName, new[] { csPathName }).Object, 
                TestProjectAnalyzerMock(testCsprojPathName, csprojPathName,  "net4.5", false).Object, out var mockRunner);

            // act
            var result = target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();

            // assert
            result.ShouldHaveSingleItem();
        }

        [Fact]
        public void ShouldFailIfNoProjectFound()
        {
            var testCsprojPathName = _fileSystem.Path.Combine(_projectPath, "testproject.csproj");
            var csprojPathName = _fileSystem.Path.Combine(_projectPath, "sourceproject.csproj");
            var options = new StrykerOptions
            {
                ProjectPath = _fileSystem.Path.GetFullPath(testCsprojPathName),
                SourceProjectName = "sourceprojec.csproj",
            };

            var csPathName = _fileSystem.Path.Combine(_projectPath, "someFile.cs");
            var target = BuildProjectOrchestratorForSimpleProject(SourceProjectAnalyzerMock(csprojPathName, new[] { csPathName }).Object, 
                TestProjectAnalyzerMock(testCsprojPathName, csprojPathName).Object, out var mockRunner);
            
            _fileSystem.Directory.SetCurrentDirectory(_fileSystem.Path.GetFullPath(testCsprojPathName));
            
            // act
            var mutateAction = () => target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();

            // assert
            mutateAction.ShouldThrow<InputException>();
        }

        [Fact]
        public void ShouldFilterInSolutionMode()
        {
            // when a solutionPath is given and it's inside the current directory (basePath)
            var testCsprojPathName = _fileSystem.Path.Combine(_projectPath, "testproject.csproj");
            var csprojPathName = _fileSystem.Path.Combine(_projectPath, "sourceproject.csproj");
            var options = new StrykerOptions
            {
                ProjectPath = _fileSystem.Path.GetFullPath(_projectPath),
                // provide an invalid source project name which should normally fail
                SourceProjectName = "sourceprojec.csproj",
                SolutionPath = _fileSystem.Path.Combine(_projectPath, "MySolution.sln")
            };

            var csPathName = _fileSystem.Path.Combine(_projectPath, "someFile.cs");
            var target = BuildProjectOrchestratorForSimpleProject(SourceProjectAnalyzerMock(csprojPathName, new[] { csPathName }).Object, 
                TestProjectAnalyzerMock(testCsprojPathName, csprojPathName).Object, out var mockRunner);
            
            _fileSystem.Directory.SetCurrentDirectory(_fileSystem.Path.GetFullPath(testCsprojPathName));
            
            // act
            var result = target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();

            // assert
            result.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldProvideMinimalSupportForFSharp()
        {
            var testCsprojPathName = _fileSystem.Path.Combine(_projectPath, "testproject.csproj");
            var csprojPathName = _fileSystem.Path.Combine(_projectPath, "sourceproject.csproj");
            var options = new StrykerOptions
            {
                ProjectPath = _fileSystem.Path.GetFullPath(testCsprojPathName),
                SolutionPath = _fileSystem.Path.Combine(_projectPath, "MySolution.sln")
            };

            var csPathName = _fileSystem.Path.Combine(_projectPath, "someFile.fs");
            var properties = new Dictionary<string, string>
                { { "IsTestProject", "False" }, { "ProjectTypeGuids", "not testproject" }, { "Language", "F#" } };
            var properties1 = new Dictionary<string, string>
                { { "IsTestProject", "True" }, { "Language", "F#" } };
            var target = BuildProjectOrchestratorForSimpleProject(BuildProjectAnalyzerMock(csprojPathName, new[] { csPathName }, properties, new List<string>()).Object,
                BuildProjectAnalyzerMock(testCsprojPathName, Array.Empty<string>(), properties1, new List<string>{ csprojPathName}).Object, out var mockRunner);
            
            _fileSystem.Directory.SetCurrentDirectory(_fileSystem.Path.GetFullPath(testCsprojPathName));
            
            // act
            var result = target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();

            // assert
            result.ShouldHaveSingleItem();
        }

        [Fact]
        public void ShouldDiscoverUpstreamProject()
        {
            // arrange
            var testCsprojPathName = _fileSystem.Path.Combine(_projectPath, "testproject.csproj");
            var csprojPathName = _fileSystem.Path.Combine(_projectPath, "sourceproject.csproj");
            var options = new StrykerOptions
            {
                ProjectPath = _projectPath,
                SolutionPath = _fileSystem.Path.Combine(_projectPath, "MySolution.sln")
            };
            var libraryProject = _fileSystem.Path.Combine(_projectPath, "libraryproject.csproj");

            // The analyzer finds two projects
            var analyzerResults = new Dictionary<string, IProjectAnalyzer>
            {
                { "MyProject", SourceProjectAnalyzerMock(csprojPathName, new[]
                        { _fileSystem.Path.Combine(_projectPath, "someFile.cs")}
                    , new[] {libraryProject}).Object },
                { "MyLibrary", SourceProjectAnalyzerMock(libraryProject,
                new[] { _fileSystem.Path.Combine(_projectPath, "mylib.cs") }).Object },
                { "MyProject.UnitTests", TestProjectAnalyzerMock(testCsprojPathName, csprojPathName).Object }
            };
            var target = BuildProjectOrchestrator(analyzerResults, out var mockRunner);

            _fileSystem.Directory.SetCurrentDirectory(_fileSystem.Path.GetFullPath(testCsprojPathName));
            
            // act
            var result = target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();

            // assert
            result.Count.ShouldBe(2);
        }

        [Fact]
        public void ShouldDiscoverUpstreamProjectWithInvalidCasing()
        {
            // arrange
            var testCsprojPathName = _fileSystem.Path.Combine(_projectPath, "testproject.csproj");
            var csprojPathName = _fileSystem.Path.Combine(_projectPath, "sourceproject.csproj");
            var options = new StrykerOptions
            {
                ProjectPath = _projectPath,
                SolutionPath = _fileSystem.Path.Combine(_projectPath, "MySolution.sln")
            };
            var libraryProject = _fileSystem.Path.Combine(_projectPath, "libraryproject.csproj");

            // The analyzer finds two projects
            var analyzerResults = new Dictionary<string, IProjectAnalyzer>
            {
                { "MyProject", SourceProjectAnalyzerMock(csprojPathName, new[]
                        { _fileSystem.Path.Combine(_projectPath, "someFile.cs")}
                    , new[] {libraryProject.ToUpper()}).Object },
                { "MyLibrary", SourceProjectAnalyzerMock(libraryProject,
                new[] { _fileSystem.Path.Combine(_projectPath, "mylib.cs") }).Object },
                { "MyProject.UnitTests", TestProjectAnalyzerMock(testCsprojPathName, csprojPathName).Object }
            };
            var target = BuildProjectOrchestrator(analyzerResults, out var mockRunner);

            _fileSystem.Directory.SetCurrentDirectory(_fileSystem.Path.GetFullPath(testCsprojPathName));
            // act
            var result = target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();
            // assert
            result.Count.ShouldBe(2);
        }

        [Fact]
        public void ShouldDisregardInvalidReferences()
        {
            // arrange
            var testCsprojPathName = _fileSystem.Path.Combine(_projectPath, "testproject.csproj");
            var csprojPathName = _fileSystem.Path.Combine(_projectPath, "sourceproject.csproj");
            var options = new StrykerOptions
            {
                ProjectPath = _projectPath,
                SolutionPath = _fileSystem.Path.Combine(_projectPath, "MySolution.sln")
            };
            var libraryProject = _fileSystem.Path.Combine(_projectPath, "libraryproject.csproj");

            // The analyzer finds two projects
            var analyzerResults = new Dictionary<string, IProjectAnalyzer>
            {
                { "MyProject", SourceProjectAnalyzerMock(csprojPathName, new[]
                        { _fileSystem.Path.Combine(_projectPath, "someFile.cs")}
                    , new[] {libraryProject}).Object },
                { "MyProject.UnitTests", TestProjectAnalyzerMock(testCsprojPathName, csprojPathName).Object }
            };
            var target = BuildProjectOrchestrator(analyzerResults, out var mockRunner);

            _fileSystem.Directory.SetCurrentDirectory(_fileSystem.Path.GetFullPath(testCsprojPathName));
            // act
            var result = target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();
            // assert
            result.Count.ShouldBe(1);
        }

        [Fact]
        public void ShouldIgnoreProjectWithoutTestAssemblies()
        {
            // arrange
            var testCsprojPathName = _fileSystem.Path.Combine(_projectPath, "testproject.csproj");
            var csprojPathName = _fileSystem.Path.Combine(_projectPath, "sourceproject.csproj");
            var options = new StrykerOptions
            {
                ProjectPath = _fileSystem.Path.GetFullPath(testCsprojPathName),
                SolutionPath = _fileSystem.Path.Combine(_projectPath, "MySolution.sln")
            };
            var libraryProject = _fileSystem.Path.Combine(_projectPath, "libraryproject.csproj");

            // The analyzer finds two projects
            var analyzerResults = new Dictionary<string, IProjectAnalyzer>
            {
                { "MyProject", SourceProjectAnalyzerMock(csprojPathName, new[]
                        { _fileSystem.Path.Combine(_projectPath, "someFile.cs")}).Object },
                { "MyLibrary", SourceProjectAnalyzerMock(libraryProject,
                new[] { _fileSystem.Path.Combine(_projectPath, "mylib.cs") }).Object },
                { "MyProject.UnitTests", TestProjectAnalyzerMock(testCsprojPathName, csprojPathName).Object }
            };
            var target = BuildProjectOrchestrator(analyzerResults, out var mockRunner);

            _fileSystem.Directory.SetCurrentDirectory(_fileSystem.Path.GetFullPath(testCsprojPathName));
            
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
            var initialBuildProcessMock = new Mock<IInitialBuildProcess>();

            var target = new ProjectOrchestrator(_projectMutatorMock.Object,
                initialBuildProcessMock.Object,
                new InputFileResolver(_fileSystem, new ProjectFileReader(new Mock<INugetRestoreProcess>().Object, _buildalyzerProviderMock.Object)));

            var buildalyzerAnalyzerManagerMock = new Mock<IAnalyzerManager>(MockBehavior.Strict);
            buildalyzerAnalyzerManagerMock.Setup(x => x.Projects)
                .Returns(analyzerResults);

            foreach (var analyzerResult in analyzerResults)
            {
                var filename = analyzerResult.Value.Build().Results.First().ProjectFilePath;
                buildalyzerAnalyzerManagerMock.Setup(x => x.GetProject(filename)).Returns(analyzerResult.Value);
            }

            mockRunner = new Mock<ITestRunner>();
            mockRunner.Setup(r => r.DiscoverTests(It.IsAny<string>())).Returns(true);
            mockRunner.Setup(r => r.GetTests(It.IsAny<IProjectAndTests>())).Returns(new TestSet());
            mockRunner.Setup(r => r.InitialTest(It.IsAny<IProjectAndTests>())).Returns(new TestRunResult(true));

            _buildalyzerProviderMock.Setup(x => x.Provide(It.IsAny<string>(), It.IsAny<AnalyzerManagerOptions>()))
                .Returns(buildalyzerAnalyzerManagerMock.Object);
            _buildalyzerProviderMock.Setup(x => x.Provide(It.IsAny<AnalyzerManagerOptions>()))
                .Returns(buildalyzerAnalyzerManagerMock.Object);

            // set current directory to the test project path
            return target;
        }

        /// <summary>
        /// Build a simple production project
        /// </summary>
        /// <param name="csprojPathName">project pathname</param>
        /// <param name="sourceFiles">project source files</param>
        /// <param name="projectReferences">project references</param>
        private Mock<IProjectAnalyzer> SourceProjectAnalyzerMock(string csprojPathName, string[] sourceFiles, IEnumerable<string> projectReferences = null)
        {
            var properties = new Dictionary<string, string>
                { { "IsTestProject", "False" }, { "ProjectTypeGuids", "not testproject" }, { "Language", "C#" } };
            projectReferences??= new List<string>();

            return BuildProjectAnalyzerMock(csprojPathName, sourceFiles, properties, projectReferences);
        }

        /// <summary>
        /// Build a simple test project
        /// </summary>
        /// <param name="testCsprojPathName">test project pathname</param>
        /// <param name="csprojPathName">production code project pathname</param>
        /// <param name="framework"></param>
        /// <param name="success"></param>
        /// <returns>a mock project analyzer</returns>
        /// <remarks>the test project references the production code project and contains no source file</remarks>
        private Mock<IProjectAnalyzer> TestProjectAnalyzerMock(string testCsprojPathName, string csprojPathName, string framework = "net6.0", bool success = true)
        {
            var properties = new Dictionary<string, string>{ { "IsTestProject", "True" }, { "Language", "C#" } };

            return BuildProjectAnalyzerMock(testCsprojPathName, Array.Empty<string>(), properties, new List<string> { csprojPathName }, framework, success);
        }

        /// <summary>
        /// build a project analyzer mock
        /// </summary>
        /// <param name="csprojPathName">project file name</param>
        /// <param name="sourceFiles">source files to return</param>
        /// <param name="properties">project properties</param>
        /// <param name="projectReferences">project references</param>
        /// <param name="success">analysis success</param>
        /// <returns>a mock project analyzer</returns>
        /// <remarks>
        /// 1. project and source files will be created (empty) in the file system
        /// 2. the project analyzer mock returns a single project result</remarks>
        private Mock<IProjectAnalyzer> BuildProjectAnalyzerMock(string csprojPathName,
            string[] sourceFiles, Dictionary<string, string> properties,
            IEnumerable<string> projectReferences,
            string framework = "net6.0",    
            bool success = true)
        {
            var sourceProjectAnalyzerMock = new Mock<IProjectAnalyzer>(MockBehavior.Strict);
            var sourceProjectAnalyzerResultsMock = new Mock<IAnalyzerResults>(MockBehavior.Strict);
            var sourceProjectAnalyzerResultMock = new Mock<IAnalyzerResult>(MockBehavior.Strict);
            var sourceProjectFileMock = new Mock<IProjectFile>(MockBehavior.Strict);

            // create dummy project and source files
            _fileSystem.AddFile(csprojPathName, new MockFileData(""));
            foreach (var file in sourceFiles)
            {
                _fileSystem.AddFile(file, new MockFileData(""));
            }

            // create bin folder
            var projectUnderTestBin = _fileSystem.Path.Combine(_projectPath, "bin");
            _fileSystem.AddDirectory(projectUnderTestBin);

            var projectBin =
                _fileSystem.Path.Combine(projectUnderTestBin, _fileSystem.Path.GetFileNameWithoutExtension(csprojPathName));
            _fileSystem.AddFile(_fileSystem.Path.Combine(projectUnderTestBin, projectBin), new MockFileData(""));

            sourceProjectAnalyzerResultMock.Setup(x => x.ProjectReferences).Returns(projectReferences);
            sourceProjectAnalyzerResultMock.Setup(x => x.References).Returns(Array.Empty<string>());
            sourceProjectAnalyzerResultMock.Setup(x => x.SourceFiles).Returns(sourceFiles);
            sourceProjectAnalyzerResultMock.Setup(x => x.PreprocessorSymbols).Returns(new[] { "NET" });
            properties.Add("TargetDir", projectUnderTestBin);
            properties.Add("TargetFileName", projectBin);

            sourceProjectAnalyzerResultMock.Setup(x => x.Properties).Returns(properties);
            sourceProjectAnalyzerResultMock.Setup(x => x.ProjectFilePath).Returns(csprojPathName);
            sourceProjectAnalyzerResultMock.Setup(x => x.TargetFramework).Returns(framework);
            sourceProjectAnalyzerResultMock.Setup(x => x.Succeeded).Returns(success);

            IEnumerable<IAnalyzerResult> analyzerResults = new[] { sourceProjectAnalyzerResultMock.Object };
            sourceProjectAnalyzerResultsMock.Setup(x => x.Results).Returns(analyzerResults);
            sourceProjectAnalyzerResultsMock.Setup(x => x.GetEnumerator()).Returns(() => analyzerResults.GetEnumerator());

            sourceProjectAnalyzerMock.Setup(x => x.ProjectFile).Returns(sourceProjectFileMock.Object);
            sourceProjectAnalyzerMock.Setup(x => x.Build()).Returns(sourceProjectAnalyzerResultsMock.Object);

            sourceProjectFileMock.Setup(x => x.Path).Returns(_projectPath);
            return sourceProjectAnalyzerMock;
        }

    }
}
