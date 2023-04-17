using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Buildalyzer;
using Buildalyzer.Construction;
using Moq;
using Shouldly;
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
using Xunit.Sdk;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class ProjectOrchestratorTests : TestBase
    {
        private readonly Mock<IBuildalyzerProvider> _buildalyzerProviderMock = new Mock<IBuildalyzerProvider>(MockBehavior.Strict);
        private readonly Mock<IMutationTestProcess> _mutationTestProcessMock = new Mock<IMutationTestProcess>(MockBehavior.Strict);
        private readonly Mock<IProjectMutator> _projectMutatorMock = new Mock<IProjectMutator>(MockBehavior.Strict);
        private readonly Mock<IReporter> _reporterMock = new Mock<IReporter>(MockBehavior.Strict);
        private readonly MutationTestInput _mutationTestInput;

        public ProjectOrchestratorTests()
        {
            _mutationTestProcessMock.Setup(x => x.Mutate());
            _projectMutatorMock.Setup(x => x.MutateProject(It.IsAny<StrykerOptions>(),  It.IsAny<MutationTestInput>(), It.IsAny<IReporter>()))
                .Returns(new Mock<IMutationTestProcess>().Object);

            _mutationTestInput = new MutationTestInput()
            {
                SourceProjectInfo = new SourceProjectInfo()
                {
                    ProjectContents = new CsharpFolderComposite()
                },
            };
        }

        [Fact]
        public void ShouldInitializeEachProjectInSolution()
        {
            // arrange
            var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
            var buildalyzerAnalyzerManagerMock = new Mock<IAnalyzerManager>(MockBehavior.Strict);
            var sourceProjectAnalyzerMock = new Mock<IProjectAnalyzer>(MockBehavior.Strict);
            var sourceProjectAnalyzerResultsMock = new Mock<IAnalyzerResults>(MockBehavior.Strict);
            var sourceProjectAnalyzerResultMock = new Mock<IAnalyzerResult>(MockBehavior.Strict);
            var testProjectAnalyzerMock = new Mock<IProjectAnalyzer>(MockBehavior.Strict);
            var testProjectAnalyzerResultsMock = new Mock<IAnalyzerResults>(MockBehavior.Strict);
            var testProjectAnalyzerResultMock = new Mock<IAnalyzerResult>(MockBehavior.Strict);
            var sourceProjectFileMock = new Mock<IProjectFile>(MockBehavior.Strict);
            var testProjectProjectFileMock = new Mock<IProjectFile>(MockBehavior.Strict);
            var testProjectPackageReferenceMock = new Mock<IPackageReference>();

            var fileSystem = new MockFileSystem();
            var csprojPathName = "C:/projectundertest/projectundertest.csproj";
            var csPathName = "C:/projectundertest/someFile.cs";
            var testCsprojPathName = "C:/testproject/projectUnderTest.csproj";
            var ProjectUnderTestBin = "C:/projectundertest/bin";
            var testDll = "test.dll";
            fileSystem.AddFile(csprojPathName, new MockFileData(""));
            fileSystem.AddFile(csPathName, new MockFileData(""));
            fileSystem.AddFile(testCsprojPathName, new MockFileData(""));
            fileSystem.AddDirectory(ProjectUnderTestBin);
            fileSystem.AddFile(fileSystem.Path.Combine(ProjectUnderTestBin, testDll), new MockFileData(""));

            var fileResolver = new InputFileResolver(fileSystem, new ProjectFileReader());
            // when a solutionPath is given and it's inside the current directory (basePath)
            var options = new StrykerOptions
            {
                ProjectPath = "C:/MyProject",
                SolutionPath = "C:/MyProject/MyProject.sln"
            };

            var target = new ProjectOrchestrator(_buildalyzerProviderMock.Object, _projectMutatorMock.Object,
                initialBuildProcessMock.Object, fileResolver);

            initialBuildProcessMock.Setup(x =>
                x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            _buildalyzerProviderMock.Setup(x => x.Provide(It.IsAny<string>(), It.IsAny<AnalyzerManagerOptions>())).Returns(buildalyzerAnalyzerManagerMock.Object);
            // The analyzer finds two projects
            buildalyzerAnalyzerManagerMock.Setup(x => x.Projects)
                .Returns(new Dictionary<string, IProjectAnalyzer> {
                    { "MyProject", sourceProjectAnalyzerMock.Object },
                    { "MyProject.UnitTests", testProjectAnalyzerMock.Object }
                });
            testProjectAnalyzerMock.Setup(x => x.ProjectFile).Returns(testProjectProjectFileMock.Object);
            testProjectAnalyzerMock.Setup(x => x.Build()).Returns(testProjectAnalyzerResultsMock.Object);
            testProjectAnalyzerResultsMock.Setup(x => x.Results).Returns(new[] { testProjectAnalyzerResultMock.Object });
            testProjectAnalyzerResultMock.Setup(x => x.ProjectReferences).Returns(new[] { "C:/sourceproject/sourceproject.csproj" });
            testProjectAnalyzerResultMock.Setup(x => x.ProjectFilePath).Returns("C:/testproject/projectUnderTest.csproj");
            sourceProjectAnalyzerResultMock.Setup(x => x.ProjectReferences).Returns(new List<string>());
            sourceProjectAnalyzerMock.Setup(x => x.ProjectFile).Returns(sourceProjectFileMock.Object);
            sourceProjectAnalyzerMock.Setup(x => x.Build()).Returns(sourceProjectAnalyzerResultsMock.Object);
            sourceProjectAnalyzerResultsMock.Setup(x => x.Results).Returns(new[] { sourceProjectAnalyzerResultMock.Object });
            sourceProjectFileMock.Setup(x => x.PackageReferences).Returns(new List<IPackageReference>());
            sourceProjectFileMock.Setup(x => x.Path).Returns("C:/sourceproject/");
            testProjectAnalyzerResultMock.Setup(x => x.References).Returns(Array.Empty<string>());
            testProjectAnalyzerResultMock.Setup(x => x.Succeeded).Returns(true);
            // The test project references the microsoft.net.test.sdk
            testProjectAnalyzerResultMock.Setup(x => x.Properties).Returns(new Dictionary<string, string> { { "IsTestProject", "True" }, {"TargetDir", ProjectUnderTestBin}, {"TargetFileName", testDll}, {"Language", "C#"} });
            sourceProjectAnalyzerResultMock.Setup(x => x.Properties).Returns(new Dictionary<string, string> { { "IsTestProject", "False" }, { "ProjectTypeGuids", "not testproject" } });
            sourceProjectAnalyzerResultMock.Setup(x => x.ProjectFilePath).Returns("C:/sourceproject/sourceproject.csproj");
            sourceProjectAnalyzerResultMock.Setup(x => x.TargetFramework).Returns("net6.0");

            testProjectProjectFileMock.Setup(x => x.PackageReferences).Returns(new List<IPackageReference>
            {
                testProjectPackageReferenceMock.Object
            });
            testProjectProjectFileMock.Setup(x => x.Path).Returns("C:/testproject/");

            var mockRunner = new Mock<ITestRunner>();
            mockRunner.Setup(r => r.DiscoverTests(It.IsAny<string>())).Returns(true);
            mockRunner.Setup(r => r.GetTests(It.IsAny<IProjectAndTest>())).Returns(new TestSet());
            mockRunner.Setup(r => r.InitialTest(It.IsAny<IProjectAndTest>())).Returns(new TestRunResult(true));

            // act
            var result = target.MutateProjects(options, _reporterMock.Object, mockRunner.Object).ToList();

            // assert
            result.ShouldHaveSingleItem();
        }
    }
}
