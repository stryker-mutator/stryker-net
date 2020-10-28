using Moq;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Xunit;
using Language = Stryker.Core.LanguageFactory.Language;

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
            var reporterMock = new Mock<IReporter>(MockBehavior.Loose);

            var folder = new FolderComposite()
            {
                Name = "ProjectRoot"
            };
            var file = new FileLeaf()
            {
                Name = "SomeFile.cs",
                Mutants = new List<Mutant> { new Mutant { Id = 1 } }
            };
            folder.Add(file);

            var mutationTestInput = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectContents = folder
                },
            };
            initialisationMock.Setup(x => x.Initialize(It.IsAny<StrykerOptions>())).Returns(mutationTestInput);
            var options = new StrykerOptions(basePath: "c:/test", fileSystem: fileSystemMock);
            var nbTests = 0;
            initialisationMock.Setup(x => x.InitialTest(options, out nbTests)).Returns(0);

            mutationTestProcessMock.Setup(x => x.Mutate());
            mutationTestProcessMock.Setup(x => x.GetCoverage());
            mutationTestProcessMock.Setup(x => x.FilterMutants());
            mutationTestProcessMock.Setup(x => x.Test(It.IsAny<StrykerOptions>()))
                .Returns(new StrykerRunResult(It.IsAny<StrykerOptions>(), It.IsAny<double>()));



            var target = new StrykerRunner(initialisationMock.Object, mutationTestProcessMock.Object, reporter: reporterMock.Object);

            target.RunMutationTest(options);

            initialisationMock.Verify(x => x.Initialize(It.IsAny<StrykerOptions>()), Times.Once);
            mutationTestProcessMock.Verify(x => x.Mutate(), Times.Once);
            mutationTestProcessMock.Verify(x => x.Test(It.IsAny<StrykerOptions>()), Times.Once);
        }
    }
}
