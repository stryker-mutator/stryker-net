using Moq;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.TestRunners;
using System.Collections.Generic;
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
                                Name = "SomeFile.cs",
                                Mutants = new List<Mutant> { new Mutant { Id = 1 } }
                            }
                        }
                    }
                },
            });
            var options = new StrykerOptions(basePath: "c:/test", fileSystem: fileSystemMock);
            var coveredMutants = new TestCoverageInfos();
            coveredMutants.DeclareMappingForATest(new TestDescription("1", "SomeTest"), new[] { 2, 3 }, new[] { 2 });
            var nbTests = 0;
            initialisationMock.Setup(x => x.InitialTest(options, out nbTests)).Returns(0);

            mutationTestProcessMock.Setup(x => x.Mutate());
            mutationTestProcessMock.Setup(x => x.GetCoverage()).Returns(new TestCoverageInfos());
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
