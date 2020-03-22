using Moq;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Stryker.Core.UnitTest
{
    public class StrykerRunnerTests
    {
        [Fact]
        public void Stryker_ShouldInvokeAllProcesses()
        {
            var projectOrchestratorMock = new Mock<IProjectOrchestrator>(MockBehavior.Strict);
            var mutationTestProcessMock = new Mock<IMutationTestProcess>(MockBehavior.Strict);
            var fileSystemMock = new MockFileSystem();

            var mutationTestInput = new MutationTestInput()
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
            var options = new StrykerOptions(basePath: "c:/test", fileSystem: fileSystemMock);

            projectOrchestratorMock.Setup(x => x.MutateProjects(options, It.IsAny<IReporter>()))
                .Returns(new List<IMutationTestProcess>()
                {
                    mutationTestProcessMock.Object
                });

            mutationTestProcessMock.SetupGet(x => x.Input).Returns(mutationTestInput);
            mutationTestProcessMock.Setup(x => x.Mutate());
            mutationTestProcessMock.Setup(x => x.GetCoverage());
            mutationTestProcessMock.Setup(x => x.Test(It.IsAny<IEnumerable<Mutant>>()))
                .Returns(new StrykerRunResult(It.IsAny<StrykerProjectOptions>(), It.IsAny<double>()));

            var target = new StrykerRunner(projectOrchestratorMock.Object, fileSystemMock);

            target.RunMutationTest(options);

            projectOrchestratorMock.Verify(x => x.MutateProjects(options, It.IsAny<IReporter>()), Times.Once);
            mutationTestProcessMock.Verify(x => x.Mutate(), Times.Once);
            mutationTestProcessMock.Verify(x => x.Test(It.IsAny<IEnumerable<Mutant>>()), Times.Once);
        }
    }
}
