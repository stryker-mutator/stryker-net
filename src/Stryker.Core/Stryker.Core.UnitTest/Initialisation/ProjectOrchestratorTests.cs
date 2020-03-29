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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class ProjectOrchestratorTests
    {
        private Mock<IInitialisationProcessProvider> initialisationProcessProviderMock;
        private Mock<IMutationTestProcessProvider> mutationTestProcessProviderMock;
        private Mock<IMutationTestProcess> mutationTestProcessMock;
        private Mock<IInitialisationProcess> initialisationProcessMock;
        private Mock<IReporter> reporterMock;
        private MutationTestInput mutationTestInput;
        private Mock<IBuildalyzerProvider> buildalyzerProviderMock;
        private Mock<IBuildalyzer> buildalyzerMock;

        public ProjectOrchestratorTests()
        {
            initialisationProcessProviderMock = new Mock<IInitialisationProcessProvider>(MockBehavior.Strict);
            mutationTestProcessProviderMock = new Mock<IMutationTestProcessProvider>(MockBehavior.Strict);
            mutationTestProcessMock = new Mock<IMutationTestProcess>(MockBehavior.Strict);
            initialisationProcessMock = new Mock<IInitialisationProcess>(MockBehavior.Strict);
            reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            buildalyzerProviderMock = new Mock<IBuildalyzerProvider>(MockBehavior.Strict);
            buildalyzerMock = new Mock<IBuildalyzer>(MockBehavior.Strict);
            initialisationProcessProviderMock.Setup(x => x.Provide())
                .Returns(initialisationProcessMock.Object);
            mutationTestProcessProviderMock.Setup(x => x.Provide(It.IsAny<MutationTestInput>(),
                It.IsAny<IReporter>(),
                It.IsAny<IMutationTestExecutor>(),
                It.IsAny<IStrykerOptions>()))
                .Returns(mutationTestProcessMock.Object);
            mutationTestProcessMock.Setup(x => x.Mutate());

            mutationTestInput = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectContents = new FolderComposite()
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent>() {
                            new FileLeaf() {
                                Name = "SomeFile.cs",
                                Mutants = new List<Mutant> { new Mutant { Id = 1 } }
                            }
                        }
                    }
                },
            };
        }

        [Fact]
        public void ShouldInitializeProject()
        {
            var options = new StrykerOptions();
            var target = new ProjectOrchestrator(initialisationProcessProviderMock.Object, mutationTestProcessProviderMock.Object, buildalyzerProviderMock.Object);

            initialisationProcessMock.Setup(x => x.Initialize(It.IsAny<StrykerProjectOptions>()))
                .Returns(mutationTestInput);
            initialisationProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerProjectOptions>()))
                .Returns(5);

            var result = target.MutateProjects(options, reporterMock.Object);

            result.ShouldHaveSingleItem();
        }

        [Fact]
        public void ShouldInitializeEachProjectInSolution()
        {
            var buildalyzerProjectAnalyzerMock = new Mock<IBuildalyzerProjectAnalyzer>(MockBehavior.Strict);
            var projectAnalyzerResultMock = new Mock<IProjectAnalyzerResult>(MockBehavior.Strict);
            var buildalyzerProviderMock = new Mock<IBuildalyzerProvider>(MockBehavior.Strict);
            // when a solutionpath is given and it's inside the current directory (basepath)
            var options = new StrykerOptions(basePath: "C:/MyProject", solutionPath: "C:/MyProject/MyProject.sln");
            var target = new ProjectOrchestrator(initialisationProcessProviderMock.Object, mutationTestProcessProviderMock.Object, buildalyzerProviderMock.Object);

            initialisationProcessMock.Setup(x => x.Initialize(It.IsAny<StrykerProjectOptions>()))
                .Returns(mutationTestInput);
            initialisationProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerProjectOptions>()))
                .Returns(5);
            buildalyzerProviderMock.Setup(x => x.Provide(It.IsAny<string>(), It.IsAny<AnalyzerManagerOptions>())).Returns(buildalyzerMock.Object);
            buildalyzerMock.Setup(x => x.Projects).Returns(
                new Dictionary<string, IBuildalyzerProjectAnalyzer>()
                {
                    { "1", buildalyzerProjectAnalyzerMock.Object }
                });
            buildalyzerProjectAnalyzerMock.Setup(x => x.Build()).Returns(projectAnalyzerResultMock.Object);
            buildalyzerProjectAnalyzerMock.Setup(x => x.ProjectFile).Returns(new ProjectFile());

            var result = target.MutateProjects(options, reporterMock.Object);

            result.ShouldHaveSingleItem();
        }
    }
}
