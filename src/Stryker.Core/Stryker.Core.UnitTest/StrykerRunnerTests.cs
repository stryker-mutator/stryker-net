using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
using Moq;
using Shouldly;
using Stryker.Core.DashboardCompare;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
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
            var reporterFactoryMock = new Mock<IReporterFactory>(MockBehavior.Strict);
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            var fileSystemMock = new MockFileSystem();

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf()
            {
                Mutants = new List<Mutant> { new Mutant { Id = 1 } }
            });

            var mutationTestInput = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectContents = folder
                },
            };
            var options = new StrykerOptions(basePath: "c:/test", fileSystem: fileSystemMock);

            projectOrchestratorMock.Setup(x => x.MutateProjects(options, It.IsAny<IReporter>()))
                .Returns(new List<IMutationTestProcess>()
                {
                    mutationTestProcessMock.Object
                });

            reporterFactoryMock.Setup(x => x.Create(It.IsAny<StrykerOptions>(), It.IsAny<IGitInfoProvider>())).Returns(reporterMock.Object);

            reporterMock.Setup(x => x.OnStartMutantTestRun(It.IsAny<IEnumerable<IReadOnlyMutant>>()));
            reporterMock.Setup(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>()));

            mutationTestProcessMock.SetupGet(x => x.Input).Returns(mutationTestInput);
            mutationTestProcessMock.Setup(x => x.GetCoverage());
            mutationTestProcessMock.Setup(x => x.Test(It.IsAny<IEnumerable<Mutant>>()))
                .Returns(new StrykerRunResult(It.IsAny<StrykerOptions>(), It.IsAny<double>()));

            // Set up sequence-critical methods:
            // * FilterMutants must be called before OnMutantsCreated to get valid reports
            var seq = new MockSequence();
            mutationTestProcessMock.InSequence(seq).Setup(x => x.FilterMutants());
            reporterMock.InSequence(seq).Setup(x => x.OnMutantsCreated(It.IsAny<IReadOnlyProjectComponent>()));

            var target = new StrykerRunner(projectOrchestratorMock.Object, reporterFactory: reporterFactoryMock.Object);

            target.RunMutationTest(options);

            projectOrchestratorMock.Verify(x => x.MutateProjects(options, It.IsAny<IReporter>()), Times.Once);
            mutationTestProcessMock.Verify(x => x.GetCoverage(), Times.Once);
            mutationTestProcessMock.Verify(x => x.Test(It.IsAny<IEnumerable<Mutant>>()), Times.Once);
            reporterMock.Verify(x => x.OnMutantsCreated(It.IsAny<IReadOnlyProjectComponent>()), Times.Once);
            reporterMock.Verify(x => x.OnStartMutantTestRun(It.IsAny<IEnumerable<IReadOnlyMutant>>()), Times.Once);
            reporterMock.Verify(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>()), Times.Once);
        }

        [Fact]
        public void ShouldStop_WhenAllMutationsWereIgnored()
        {
            var projectOrchestratorMock = new Mock<IProjectOrchestrator>(MockBehavior.Strict);
            var mutationTestProcessMock = new Mock<IMutationTestProcess>(MockBehavior.Strict);
            var reporterFactoryMock = new Mock<IReporterFactory>(MockBehavior.Strict);
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            var fileSystemMock = new MockFileSystem();

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf
            {
                Mutants = new Collection<Mutant>() { new Mutant() { Id = 1, ResultStatus = MutantStatus.Ignored } }
            });
            var mutationTestInput = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectContents = folder
                }
            };
            var options = new StrykerOptions(basePath: "c:/test", fileSystem: fileSystemMock, coverageAnalysis: "off");

            projectOrchestratorMock.Setup(x => x.MutateProjects(options, It.IsAny<IReporter>()))
                .Returns(new List<IMutationTestProcess>() { mutationTestProcessMock.Object });

            mutationTestProcessMock.Setup(x => x.FilterMutants());
            mutationTestProcessMock.SetupGet(x => x.Input).Returns(mutationTestInput);

            reporterFactoryMock.Setup(x => x.Create(It.IsAny<StrykerOptions>(), It.IsAny<IGitInfoProvider>())).Returns(reporterMock.Object);

            reporterMock.Setup(x => x.OnMutantsCreated(It.IsAny<IReadOnlyProjectComponent>()));
            reporterMock.Setup(x => x.OnStartMutantTestRun(It.IsAny<IEnumerable<IReadOnlyMutant>>()));

            var target = new StrykerRunner(projectOrchestratorMock.Object, reporterFactory: reporterFactoryMock.Object);

            var result = target.RunMutationTest(options);

            result.MutationScore.ShouldBe(double.NaN);

            reporterMock.Verify(x => x.OnStartMutantTestRun(It.IsAny<IList<Mutant>>()), Times.Never);
            reporterMock.Verify(x => x.OnMutantTested(It.IsAny<Mutant>()), Times.Never);
            reporterMock.Verify(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>()), Times.Never);
        }
    }
}
