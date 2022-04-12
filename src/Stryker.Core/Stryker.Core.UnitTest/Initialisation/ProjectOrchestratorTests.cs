using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Moq;
using Shouldly;
using Stryker.Core.Initialisation;
using Stryker.Core.Initialisation.ProjectAnalyzer;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using Stryker.Core.Testing;
using Stryker.Core.TestRunners;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class ProjectOrchestratorTests : TestBase
    {
        private readonly Mock<IProjectsAnalyzerManagerProvider> _solutionAnalyzerManagerProviderMock = new Mock<IProjectsAnalyzerManagerProvider>(MockBehavior.Strict);
        private readonly Mock<IMutationTestProcess> _mutationTestProcessMock = new Mock<IMutationTestProcess>(MockBehavior.Strict);
        private readonly Mock<IProjectMutator> _projectMutatorMock = new Mock<IProjectMutator>(MockBehavior.Strict);
        private readonly Mock<IInitialisationProcess> _initialisationProcessMock = new Mock<IInitialisationProcess>(MockBehavior.Strict);
        private readonly Mock<IReporter> _reporterMock = new Mock<IReporter>(MockBehavior.Strict);
        private readonly MutationTestInput _mutationTestInput;

        public ProjectOrchestratorTests()
        {
            _mutationTestProcessMock.Setup(x => x.Mutate());
            _projectMutatorMock.Setup(x => x.MutateProject(It.IsAny<StrykerOptions>(), It.IsAny<IReporter>()))
                .Returns(new Mock<IMutationTestProcess>().Object);

            _mutationTestInput = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo(new MockFileSystem())
                {
                    ProjectContents = new CsharpFolderComposite()
                },
            };
        }

        [Fact]
        public void ShouldInitializeEachProjectInSolution()
        {
            // arrange
            var solutionAnalyzerManagerMock = new Mock<IProjectsAnalyzerManager>(MockBehavior.Strict);
            var projectUnderTestAnalyzerMock = new Mock<IProjectAnalyzer>(MockBehavior.Strict);
            var projectUnderTestAnalyzerResultMock = new Mock<IAnalysisResult>(MockBehavior.Strict);
            var testProjectAnalyzerMock = new Mock<IProjectAnalyzer>(MockBehavior.Strict);
            var testProjectAnalyzerResultMock = new Mock<IAnalysisResult>(MockBehavior.Strict);
            var buildLocatorMock = new Mock<BuildLocator>(MockBehavior.Strict);

            // when a solutionPath is given and it's inside the current directory (basePath)
            var options = new StrykerOptions
            {
                BasePath = "C:/MyProject",
                SolutionPath = "C:/MyProject/MyProject.sln"
            };
            var target = new ProjectOrchestrator(_solutionAnalyzerManagerProviderMock.Object, _projectMutatorMock.Object);

            _initialisationProcessMock.Setup(x => x.Initialize(It.IsAny<StrykerOptions>())).Returns(_mutationTestInput);
            _initialisationProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>())).Returns(new InitialTestRun(new TestRunResult(true), new TimeoutValueCalculator(5)));
            _solutionAnalyzerManagerProviderMock.Setup(x => x.Provide(It.IsAny<string>())).Returns(solutionAnalyzerManagerMock.Object);
            // The analyzer finds two projects
            solutionAnalyzerManagerMock.Setup(x => x.Projects)
                .Returns(new Dictionary<string, IProjectAnalyzer> {
                    { "MyProject", projectUnderTestAnalyzerMock.Object },
                    { "MyProject.UnitTests", testProjectAnalyzerMock.Object }
                });
            testProjectAnalyzerMock.Setup(x => x.ProjectFilePath).Returns("C:/testproject/");
            testProjectAnalyzerMock.Setup(x => x.Analyze(It.IsAny<string>())).Returns(testProjectAnalyzerResultMock.Object);
            testProjectAnalyzerResultMock.Setup(x => x.ProjectReferences).Returns(new[] { "C:/projectundertest/projectundertest.csproj" });
            testProjectAnalyzerResultMock.Setup(x => x.ProjectFilePath).Returns("C:/testproject/projectUnderTest.csproj");
            projectUnderTestAnalyzerMock.Setup(x => x.ProjectFilePath).Returns("C:/projectUnderTest/");
            projectUnderTestAnalyzerMock.Setup(x => x.Analyze(It.IsAny<string>())).Returns(projectUnderTestAnalyzerResultMock.Object);
            // The test project references the microsoft.net.test.sdk
            testProjectAnalyzerResultMock.Setup(x => x.Properties).Returns(new Dictionary<string, string> { { "IsTestProject", "True" } });
            projectUnderTestAnalyzerResultMock.Setup(x => x.Properties).Returns(new Dictionary<string, string> { { "IsTestProject", "False" }, { "ProjectTypeGuids", "not testproject" } });
            projectUnderTestAnalyzerResultMock.Setup(x => x.ProjectFilePath).Returns("C:/projectundertest/projectundertest.csproj");
            buildLocatorMock.Setup(x => x.Initialize());

            // act
            var result = target.MutateProjects(options, _reporterMock.Object, buildLocatorMock.Object).ToList();

            // assert
            var mutationTestProcess = result.ShouldHaveSingleItem();
        }
    }
}
