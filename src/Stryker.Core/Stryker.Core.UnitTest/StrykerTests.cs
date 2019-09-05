using Moq;
using Stryker.Core.Initialisation;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
using Stryker.Core.Mutants;
using Stryker.Core.TestRunners;
using Xunit;

namespace Stryker.Core.UnitTest
{
    public class StrykerTests
    {
        [Fact]
        public void Stryker_ShouldInvokeAllProcesses()
        {
            var initialisationMock = new Mock<IInitialisationProcess>(MockBehavior.Strict);
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
                                Name = "SomeFile.cs"
                            }
                        }
                    }
                },
            };
            initialisationMock.Setup(x => x.Initialize(It.IsAny<StrykerOptions>())).Returns(mutationTestInput);
            var options = new StrykerOptions(basePath: "c:/test", fileSystem: fileSystemMock);
            var nbTests = 0;
            initialisationMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), out nbTests)).Returns(0);
            initialisationMock.Setup(x => x.GetCoverage(It.IsAny<StrykerOptions>(), mutationTestInput.ProjectInfo.ProjectContents.Mutants));

            mutationTestProcessMock.Setup(x => x.Mutate());
            mutationTestProcessMock.Setup(x => x.Test(It.IsAny<StrykerOptions>()))
                .Returns(new StrykerRunResult(It.IsAny<StrykerOptions>(), It.IsAny<decimal?>()));

            var target = new StrykerRunner(initialisationMock.Object, mutationTestProcessMock.Object, fileSystemMock);

            
            target.RunMutationTest(options);

            initialisationMock.Verify(x => x.Initialize(It.IsAny<StrykerOptions>()), Times.Once);
            mutationTestProcessMock.Verify(x => x.Mutate(), Times.Once);
            mutationTestProcessMock.Verify(x => x.Test(It.IsAny<StrykerOptions>()), Times.Once);
        }
    }
}
