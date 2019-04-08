using System;
using System.Collections.Generic;
using Moq;
using Stryker.Core.Initialisation;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
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

            initialisationMock.Setup(x => x.Initialize(It.IsAny<StrykerOptions>())).Returns(new MutationTestInput()
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
            });
            var options = new StrykerOptions(basePath: "c:/test", fileSystem: fileSystemMock);
            var coveredMutants = new[]{2,3};
            initialisationMock.SetupGet(x => x.CoveredMutants).Returns(coveredMutants);
            initialisationMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>())).Returns(0);

            mutationTestProcessMock.Setup(x => x.Mutate(options));
            mutationTestProcessMock.Setup(x => x.Test(It.IsAny<StrykerOptions>()))
                .Returns(new StrykerRunResult(It.IsAny<StrykerOptions>(), It.IsAny<decimal?>()));

            mutationTestProcessMock.Setup( x=> x.Optimize(It.IsAny<IEnumerable<int>>()));
            var target = new StrykerRunner(initialisationMock.Object, mutationTestProcessMock.Object, fileSystemMock);


            target.RunMutationTest(options);

            initialisationMock.Verify(x => x.Initialize(It.IsAny<StrykerOptions>()), Times.Once);
            initialisationMock.VerifyGet(x => x.CoveredMutants, Times.Once);
            mutationTestProcessMock.Verify(x => x.Mutate(options), Times.Once);
            mutationTestProcessMock.Verify(x => x.Optimize(coveredMutants), Times.Once);
            mutationTestProcessMock.Verify(x => x.Test(It.IsAny<StrykerOptions>()), Times.Once);
        }
    }
}
