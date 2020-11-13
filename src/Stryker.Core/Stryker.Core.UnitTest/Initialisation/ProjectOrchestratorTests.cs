using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Stryker.Core.Reporters;
using Stryker.Core.Testing;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class ProjectOrchestratorTests
    {
        private readonly Mock<IInitialisationProcessProvider> _initialisationProcessProviderMock = new Mock<IInitialisationProcessProvider>(MockBehavior.Strict);
        private readonly Mock<IMutationTestProcessProvider> _mutationTestProcessProviderMock = new Mock<IMutationTestProcessProvider>(MockBehavior.Strict);
        private readonly Mock<IMutationTestProcess> _mutationTestProcessMock = new Mock<IMutationTestProcess>(MockBehavior.Strict);
        private readonly Mock<IInitialisationProcess> _initialisationProcessMock = new Mock<IInitialisationProcess>(MockBehavior.Strict);
        private readonly Mock<IReporter> _reporterMock = new Mock<IReporter>(MockBehavior.Strict);
        private readonly MutationTestInput _mutationTestInput;
        private readonly Mock<IBuildalyzerProvider> _buildalyzerProviderMock = new Mock<IBuildalyzerProvider>(MockBehavior.Strict);

        public ProjectOrchestratorTests()
        {
            _initialisationProcessProviderMock.Setup(x => x.Provide()).Returns(_initialisationProcessMock.Object);

            _mutationTestProcessProviderMock
                .Setup(x => x.Provide(
                    It.IsAny<MutationTestInput>(),
                    It.IsAny<IReporter>(),
                    It.IsAny<IMutationTestExecutor>(),
                    It.IsAny<IStrykerOptions>()))
                .Returns(_mutationTestProcessMock.Object);

            _mutationTestProcessMock.Setup(x => x.Mutate());

            var components = new Collection<IProjectComponent>() {
                new FileLeaf() {
                    Name = "SomeFile.cs",
                    Mutants = new List<Mutant> { new Mutant { Id = 1 } }
                }
            };

            _mutationTestInput = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectContents = new FolderComposite()
                    {
                        Name = "ProjectRoot"
                    }
                },
            };
        }

        [Fact]
        public void ShouldInitializeProject()
        {
            var options = new StrykerOptions();
            var target = new ProjectOrchestrator(_initialisationProcessProviderMock.Object, _mutationTestProcessProviderMock.Object, _buildalyzerProviderMock.Object);

            _initialisationProcessMock.Setup(x => x.Initialize(It.IsAny<IStrykerOptions>()))
                .Returns(_mutationTestInput);
            _initialisationProcessMock.Setup(x => x.InitialTest(It.IsAny<IStrykerOptions>()))
                .Returns(5);

            var result = target.MutateProjects(options, _reporterMock.Object);

            result.ShouldHaveSingleItem();
        }

        // This test is a bit flaky :/ If it fails, run it again a couple times. Perhaps as single run, run all or debug run.
        [Fact]
        public void ShouldInitializeEachProjectInSolution()
        {
            var buildalyzerAnalyzerManagerMock = new Mock<IAnalyzerManager>(MockBehavior.Strict);
            var projectUnderTestAnalyzerMock = new Mock<IProjectAnalyzer>(MockBehavior.Strict);
            var projectUnderTestAnalyzerResultsMock = new Mock<IAnalyzerResults>(MockBehavior.Strict);
            var projectUnderTestAnalyzerResultMock = new Mock<IAnalyzerResult>(MockBehavior.Strict);
            var testProjectAnalyzerMock = new Mock<IProjectAnalyzer>(MockBehavior.Strict);
            var testProjectAnalyzerResultsMock = new Mock<IAnalyzerResults>(MockBehavior.Strict);
            var testProjectAnalyzerResultMock = new Mock<IAnalyzerResult>(MockBehavior.Strict);
            var projectUnderTestProjectFileMock = new Mock<IProjectFile>(MockBehavior.Strict);
            var testProjectProjectFileMock = new Mock<IProjectFile>(MockBehavior.Strict);
            var buildalyzerProviderMock = new Mock<IBuildalyzerProvider>(MockBehavior.Strict);
            var testProjectPackagereferenceMock = new Mock<IPackageReference>();

            // when a solutionpath is given and it's inside the current directory (basepath)
            var options = new StrykerOptions(basePath: "C:/MyProject", solutionPath: "C:/MyProject/MyProject.sln");
            var target = new ProjectOrchestrator(_initialisationProcessProviderMock.Object, _mutationTestProcessProviderMock.Object, buildalyzerProviderMock.Object);

            _initialisationProcessMock.Setup(x => x.Initialize(It.IsAny<IStrykerOptions>())).Returns(_mutationTestInput);
            _initialisationProcessMock.Setup(x => x.InitialTest(It.IsAny<IStrykerOptions>())).Returns(5);
            buildalyzerProviderMock.Setup(x => x.Provide(It.IsAny<string>(), It.IsAny<AnalyzerManagerOptions>())).Returns(buildalyzerAnalyzerManagerMock.Object);
            // The analyzer finds two projects
            buildalyzerAnalyzerManagerMock.Setup(x => x.Projects).Returns(new Dictionary<string, IProjectAnalyzer> {
                { "put", projectUnderTestAnalyzerMock.Object }, { "test", testProjectAnalyzerMock.Object }
            });
            testProjectAnalyzerMock.Setup(x => x.ProjectFile).Returns(testProjectProjectFileMock.Object);
            testProjectAnalyzerMock.Setup(x => x.Build()).Returns(testProjectAnalyzerResultsMock.Object);
            testProjectAnalyzerResultsMock.Setup(x => x.Results).Returns(new[] { testProjectAnalyzerResultMock.Object });
            testProjectAnalyzerResultMock.Setup(x => x.ProjectReferences).Returns(new[] { "C:/projectundertest/projectundertest.csproj" });
            testProjectAnalyzerResultMock.Setup(x => x.ProjectFilePath).Returns("C:/testproject/projectUnderTest.csproj");
            projectUnderTestAnalyzerMock.Setup(x => x.ProjectFile).Returns(projectUnderTestProjectFileMock.Object);
            projectUnderTestAnalyzerMock.Setup(x => x.Build()).Returns(projectUnderTestAnalyzerResultsMock.Object);
            projectUnderTestAnalyzerResultsMock.Setup(x => x.Results).Returns(new[] { projectUnderTestAnalyzerResultMock.Object });
            projectUnderTestProjectFileMock.Setup(x => x.PackageReferences).Returns(new List<IPackageReference>());
            projectUnderTestProjectFileMock.Setup(x => x.Path).Returns("C:/projectUnderTest/");
            // The test project references the microsoft.net.test.sdk
            testProjectAnalyzerResultMock.Setup(x => x.Properties).Returns(new Dictionary<string, string> { { "IsTestProject", "True" } });
            projectUnderTestAnalyzerResultMock.Setup(x => x.Properties).Returns(new Dictionary<string, string> { { "IsTestProject", "False" }, { "ProjectTypeGuids", "not testproject" } });
            projectUnderTestAnalyzerResultMock.Setup(x => x.ProjectFilePath).Returns("C:/projectundertest/projectundertest.csproj");
            testProjectProjectFileMock.Setup(x => x.PackageReferences).Returns(new List<IPackageReference>() {
                testProjectPackagereferenceMock.Object
            });
            testProjectProjectFileMock.Setup(x => x.Path).Returns("C:/testproject/");
            var result = target.MutateProjects(options, _reporterMock.Object).ToList();

            var mutationTestProcess = result.ShouldHaveSingleItem();
        }
    }
}
