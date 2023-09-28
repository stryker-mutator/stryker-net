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
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.SourceProjects;
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

            new MutationTestInput()
            {
                SourceProjectInfo = new SourceProjectInfo()
                {
                    ProjectContents = new CsharpFolderComposite()
                }
            };
            _projectPath = _fileSystem.Path.Combine(filesystemRoot, "sourceproject");
        }

        [Fact]

        public void ShouldInitializeEachProjectInSolution()
        {
            // arrange
            // when a solutionPath is given and it's inside the current directory (basePath)
            var options = new StrykerOptions
            {
                ProjectPath = _projectPath,
                SolutionPath = _fileSystem.Path.Combine(_projectPath, "MySolution.sln")
            };

            var target = SetupMockAnalysis(out var mockRunner);

            // act
            var result = target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();

            // assert
            result.ShouldHaveSingleItem();
        }

        [Fact]

        public void ShouldPassWhenProjectNameIsGiven()
        {
            // arrange
            // when a solutionPath is given and it's inside the current directory (basePath)
            var options = new StrykerOptions
            {
                ProjectPath = _projectPath,
                SourceProjectName = _projectPath,
                SolutionPath = _fileSystem.Path.Combine(_projectPath, "MySolution.sln")
            };

            var target = SetupMockAnalysis(out var mockRunner);

            // act
            var result = target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();

            // assert
            result.ShouldHaveSingleItem();
        }

        [Fact]
        public void ShouldFailIfNoProjectFound()
        {
            // when a solutionPath is given and it's inside the current directory (basePath)
            var options = new StrykerOptions
            {
                ProjectPath = _projectPath,
                SourceProjectName = "sourceprojec.csproj",
                SolutionPath = _fileSystem.Path.Combine(_projectPath, "MySolution.sln")
            };

            var target = SetupMockAnalysis(out var mockRunner);
            
            // act
            var mutateAction = () => target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();

            // assert
            mutateAction.ShouldThrow<InputException>();
        }


        private ProjectOrchestrator SetupMockAnalysis(out Mock<ITestRunner> mockRunner)
        {
            var csprojPathName = _fileSystem.Path.Combine(_projectPath, "sourceproject.csproj");
            var csPathName = _fileSystem.Path.Combine(_projectPath, "someFile.cs");
            var testCsprojPathName = _fileSystem.Path.Combine(_projectPath, "testproject.csproj");
            var projectUnderTestBin = _fileSystem.Path.Combine(_projectPath, "bin");
            var testDll = "test.dll";
            _fileSystem.AddFile(csprojPathName, new MockFileData(""));
            _fileSystem.AddFile(csPathName, new MockFileData(""));
            _fileSystem.AddFile(testCsprojPathName, new MockFileData(""));
            _fileSystem.AddDirectory(projectUnderTestBin);
            _fileSystem.AddFile(_fileSystem.Path.Combine(projectUnderTestBin, testDll), new MockFileData(""));


            var buildalyzerAnalyzerManagerMock = new Mock<IAnalyzerManager>(MockBehavior.Strict);
            var sourceProjectAnalyzerMock = new Mock<IProjectAnalyzer>(MockBehavior.Strict);
            var sourceProjectAnalyzerResultsMock = new Mock<IAnalyzerResults>(MockBehavior.Strict);
            var sourceProjectAnalyzerResultMock = new Mock<IAnalyzerResult>(MockBehavior.Strict);

            var testProjectAnalyzerMock = new Mock<IProjectAnalyzer>(MockBehavior.Strict);
            var testProjectAnalyzerResultsMock = new Mock<IAnalyzerResults>(MockBehavior.Strict);
            var testProjectAnalyzerResultMock = new Mock<IAnalyzerResult>(MockBehavior.Strict);

            var sourceProjectFileMock = new Mock<IProjectFile>(MockBehavior.Strict);
            var testProjectProjectFileMock = new Mock<IProjectFile>(MockBehavior.Strict);

            var fileResolver = new InputFileResolver(_fileSystem, new ProjectFileReader(null, _buildalyzerProviderMock.Object));

            var initialBuildProcessMock = new Mock<IInitialBuildProcess>();

            var target = new ProjectOrchestrator(_projectMutatorMock.Object,
                initialBuildProcessMock.Object, fileResolver);


            // The analyzer finds two projects
            buildalyzerAnalyzerManagerMock.Setup(x => x.Projects)
                .Returns(new Dictionary<string, IProjectAnalyzer>
                {
                    { "MyProject", sourceProjectAnalyzerMock.Object },
                    { "MyProject.UnitTests", testProjectAnalyzerMock.Object }
                });
            buildalyzerAnalyzerManagerMock.Setup(x => x.GetProject(csprojPathName)).Returns(sourceProjectAnalyzerMock.Object);
            buildalyzerAnalyzerManagerMock.Setup(x => x.GetProject(testCsprojPathName)).Returns(testProjectAnalyzerMock.Object);
            _buildalyzerProviderMock.Setup(x => x.Provide(It.IsAny<string>(), It.IsAny<AnalyzerManagerOptions>()))
                .Returns(buildalyzerAnalyzerManagerMock.Object);
            _buildalyzerProviderMock.Setup(x => x.Provide(It.IsAny<AnalyzerManagerOptions>()))
                .Returns(buildalyzerAnalyzerManagerMock.Object);

            testProjectAnalyzerMock.Setup(x => x.ProjectFile).Returns(testProjectProjectFileMock.Object);
            testProjectAnalyzerMock.Setup(x => x.Build()).Returns(testProjectAnalyzerResultsMock.Object);
            IEnumerable<IAnalyzerResult> testAnalyzerResults = new[] { testProjectAnalyzerResultMock.Object };
            testProjectAnalyzerResultsMock.Setup(x => x.Results).Returns(testAnalyzerResults);
            testProjectAnalyzerResultsMock
                .Setup(m => m.GetEnumerator())
                .Returns(() => testAnalyzerResults.GetEnumerator());

            testProjectAnalyzerResultMock.Setup(x => x.ProjectReferences).Returns(new[] { csprojPathName });
            testProjectAnalyzerResultMock.Setup(x => x.ProjectFilePath).Returns(testCsprojPathName);
            testProjectAnalyzerResultMock.Setup(x => x.TargetFramework).Returns("net6.0");
            testProjectAnalyzerResultMock.Setup(x => x.References).Returns(Array.Empty<string>());
            testProjectAnalyzerResultMock.Setup(x => x.Succeeded).Returns(true);
            // The test project references the microsoft.net.test.sdk
            testProjectAnalyzerResultMock.Setup(x => x.Properties).Returns(new Dictionary<string, string>
            {
                { "IsTestProject", "True" }, { "TargetDir", projectUnderTestBin }, { "TargetFileName", testDll },
                { "Language", "C#" }
            });
            testProjectAnalyzerResultMock.Setup(x => x.SourceFiles).Returns(Array.Empty<string>());

            sourceProjectAnalyzerResultMock.Setup(x => x.ProjectReferences).Returns(new List<string>());
            sourceProjectAnalyzerResultMock.Setup(x => x.References).Returns(Array.Empty<string>());
            sourceProjectAnalyzerResultMock.Setup(x => x.SourceFiles).Returns(new[] { csPathName });
            sourceProjectAnalyzerResultMock.Setup(x => x.PreprocessorSymbols).Returns(new[] { "NET" });
            sourceProjectAnalyzerResultMock.Setup(x => x.Properties).Returns(new Dictionary<string, string>
                { { "IsTestProject", "False" }, { "ProjectTypeGuids", "not testproject" }, { "Language", "C#" } });
            sourceProjectAnalyzerResultMock.Setup(x => x.ProjectFilePath).Returns(csprojPathName);
            sourceProjectAnalyzerResultMock.Setup(x => x.TargetFramework).Returns("net6.0");
            sourceProjectAnalyzerResultMock.Setup(x => x.Succeeded).Returns(true);

            IEnumerable<IAnalyzerResult> analyzerResults = new[] { sourceProjectAnalyzerResultMock.Object };
            sourceProjectAnalyzerResultsMock.Setup(x => x.Results).Returns(analyzerResults);
            sourceProjectAnalyzerResultsMock.Setup(x => x.GetEnumerator()).Returns(() => analyzerResults.GetEnumerator());

            sourceProjectAnalyzerMock.Setup(x => x.ProjectFile).Returns(sourceProjectFileMock.Object);
            sourceProjectAnalyzerMock.Setup(x => x.Build()).Returns(sourceProjectAnalyzerResultsMock.Object);

            sourceProjectFileMock.Setup(x => x.Path).Returns(_projectPath);

            testProjectProjectFileMock.Setup(x => x.Path).Returns(_projectPath);

            mockRunner = new Mock<ITestRunner>();
            mockRunner.Setup(r => r.DiscoverTests(It.IsAny<string>())).Returns(true);
            mockRunner.Setup(r => r.GetTests(It.IsAny<IProjectAndTests>())).Returns(new TestSet());
            mockRunner.Setup(r => r.InitialTest(It.IsAny<IProjectAndTests>())).Returns(new TestRunResult(true));
            return target;
        }

    }
}
