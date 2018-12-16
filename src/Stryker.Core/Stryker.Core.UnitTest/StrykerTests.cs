using Moq;
using Stryker.Core.Initialisation;
using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using System.Collections.ObjectModel;
using Xunit;

namespace Stryker.Core.UnitTest
{
    public class StrykerTests
    {
        [Fact]
        public void Stryker_ShouldInvokeAllProcesses()
        {
            string basePath = @"c:\TestProject\";

            var initialisationMock = new Mock<IInitialisationProcess>(MockBehavior.Strict);
            var mutationTestProcessMock = new Mock<IMutationTestProcess>(MockBehavior.Strict);

            initialisationMock.Setup(x => x.Initialize(It.IsAny<StrykerOptions>())).Returns(new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    TestProjectPath = basePath,
                    ProjectContents = new FolderComposite()
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent>() {
                            new FileLeaf() {
                                Name = "SomeFile.cs"
                            }
                        }
                    },
                    ProjectUnderTestAssemblyName = "ExampleProject.dll",
                    ProjectUnderTestPath = @"c:\ExampleProject\",
                    TargetFramework = "netcoreapp2.0"
                },
            });
            var options = new StrykerOptions();

            mutationTestProcessMock.Setup(x => x.Mutate(options));
            mutationTestProcessMock.Setup(x => x.Test(It.IsAny<StrykerOptions>()))
                .Returns(new StrykerRunResult(It.IsAny<StrykerOptions>(), It.IsAny<decimal?>()));

            var target = new StrykerRunner(initialisationMock.Object, mutationTestProcessMock.Object);

            target.RunMutationTest(options);

            initialisationMock.Verify(x => x.Initialize(It.IsAny<StrykerOptions>()), Times.Once);
            mutationTestProcessMock.Verify(x => x.Mutate(options), Times.Once);
            mutationTestProcessMock.Verify(x => x.Test(It.IsAny<StrykerOptions>()), Times.Once);
        }
    }
}
