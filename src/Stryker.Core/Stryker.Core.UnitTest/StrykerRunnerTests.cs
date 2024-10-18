using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.ProjectComponents.Csharp;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;

namespace Stryker.Core.UnitTest
{
    [TestClass]
    public class StrykerRunnerTests : TestBase
    {
        [TestMethod]
        public void Stryker_ShouldInvokeAllProcesses()
        {
            var projectOrchestratorMock = new Mock<IProjectOrchestrator>(MockBehavior.Strict);
            var mutationTestProcessMock = new Mock<IMutationTestProcess>(MockBehavior.Strict);
            var reporterFactoryMock = new Mock<IReporterFactory>(MockBehavior.Strict);
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            var inputsMock = new Mock<IStrykerInputs>(MockBehavior.Strict);
            var fileSystemMock = new MockFileSystem();

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf()
            {
                Mutants = new List<IMutant> { new Mutant { Id = 1 } }
            });

            var projectInfo = Mock.Of<SourceProjectInfo>();
            projectInfo.ProjectContents = folder;

            var mutationTestInput = new MutationTestInput()
            {
                SourceProjectInfo = projectInfo
            };

            inputsMock.Setup(x => x.ValidateAll()).Returns(new StrykerOptions
            {
                ProjectPath = "C:/test",
                LogOptions = new LogOptions(),
                OptimizationMode = OptimizationModes.SkipUncoveredMutants
            });

            projectOrchestratorMock.Setup(x => x.MutateProjects(It.IsAny<StrykerOptions>(), It.IsAny<IReporter>(), It.IsAny<ITestRunner>()))
                .Returns(new List<IMutationTestProcess>()
                {
                    mutationTestProcessMock.Object
                });

            reporterFactoryMock.Setup(x => x.Create(It.IsAny<StrykerOptions>(), It.IsAny<IGitInfoProvider>())).Returns(reporterMock.Object);

            reporterMock.Setup(x => x.OnStartMutantTestRun(It.IsAny<IEnumerable<IReadOnlyMutant>>()));
            reporterMock.Setup(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>(), It.IsAny<TestProjectsInfo>()));

            mutationTestProcessMock.SetupGet(x => x.Input).Returns(mutationTestInput);
            mutationTestProcessMock.Setup(x => x.GetCoverage());
            mutationTestProcessMock.Setup(x => x.Test(It.IsAny<IEnumerable<IMutant>>()))
                .Returns(new StrykerRunResult(It.IsAny<StrykerOptions>(), It.IsAny<double>()));
            mutationTestProcessMock.Setup(x => x.Restore());

            // Set up sequence-critical methods:
            // * FilterMutants must be called before OnMutantsCreated to get valid reports
            var seq = new MockSequence();
            mutationTestProcessMock.InSequence(seq).Setup(x => x.FilterMutants());
            reporterMock.InSequence(seq).Setup(x => x.OnMutantsCreated(It.IsAny<IReadOnlyProjectComponent>(), It.IsAny<TestProjectsInfo>()));

            var target = new StrykerRunner(reporterFactory: reporterFactoryMock.Object);

            target.RunMutationTest(inputsMock.Object, new LoggerFactory(), projectOrchestratorMock.Object);

            projectOrchestratorMock.Verify(x => x.MutateProjects(It.Is<StrykerOptions>(x => x.ProjectPath == "C:/test"), It.IsAny<IReporter>(), It.IsAny<ITestRunner>()), Times.Once);
            mutationTestProcessMock.Verify(x => x.GetCoverage(), Times.Once);
            mutationTestProcessMock.Verify(x => x.Test(It.IsAny<IEnumerable<IMutant>>()), Times.Once);
            reporterMock.Verify(x => x.OnMutantsCreated(It.IsAny<IReadOnlyProjectComponent>(), It.IsAny<TestProjectsInfo>()), Times.Once);
            reporterMock.Verify(x => x.OnStartMutantTestRun(It.IsAny<IEnumerable<IReadOnlyMutant>>()), Times.Once);
            reporterMock.Verify(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>(), It.IsAny<TestProjectsInfo>()), Times.Once);
        }

        [TestMethod]
        public void ShouldStop_WhenAllMutationsWereIgnored()
        {
            var projectOrchestratorMock = new Mock<IProjectOrchestrator>(MockBehavior.Strict);
            var mutationTestProcessMock = new Mock<IMutationTestProcess>(MockBehavior.Strict);
            var reporterFactoryMock = new Mock<IReporterFactory>(MockBehavior.Strict);
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            var inputsMock = new Mock<IStrykerInputs>(MockBehavior.Strict);
            var fileSystemMock = new MockFileSystem();

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf
            {
                Mutants = new Collection<IMutant>() { new Mutant() { Id = 1, ResultStatus = MutantStatus.Ignored } }
            });
            var mutationTestInput = new MutationTestInput()
            {
                SourceProjectInfo = new SourceProjectInfo()
                {
                    ProjectContents = folder
                }
            };

            inputsMock.Setup(x => x.ValidateAll()).Returns(new StrykerOptions
            {
                ProjectPath = "C:/test",
                OptimizationMode = OptimizationModes.None,
                LogOptions = new LogOptions()
            });

            projectOrchestratorMock.Setup(x => x.MutateProjects(It.IsAny<StrykerOptions>(), It.IsAny<IReporter>(), It.IsAny<ITestRunner>()))
                .Returns(new List<IMutationTestProcess>() { mutationTestProcessMock.Object });

            mutationTestProcessMock.Setup(x => x.FilterMutants());
            mutationTestProcessMock.SetupGet(x => x.Input).Returns(mutationTestInput);

            reporterFactoryMock.Setup(x => x.Create(It.IsAny<StrykerOptions>(), It.IsAny<IGitInfoProvider>())).Returns(reporterMock.Object);

            reporterMock.Setup(x => x.OnMutantsCreated(It.IsAny<IReadOnlyProjectComponent>(), It.IsAny<TestProjectsInfo>()));
            reporterMock.Setup(x => x.OnStartMutantTestRun(It.IsAny<IEnumerable<IReadOnlyMutant>>()));
            reporterMock.Setup(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>(), It.IsAny<TestProjectsInfo>()));

            var target = new StrykerRunner(reporterFactory: reporterFactoryMock.Object);

            var result = target.RunMutationTest(inputsMock.Object, new LoggerFactory(), projectOrchestratorMock.Object);

            result.MutationScore.ShouldBe(double.NaN);

            reporterMock.Verify(x => x.OnStartMutantTestRun(It.IsAny<IList<IMutant>>()), Times.Never);
            reporterMock.Verify(x => x.OnMutantTested(It.IsAny<IMutant>()), Times.Never);
            reporterMock.Verify(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>(), It.IsAny<TestProjectsInfo>()), Times.Once);
        }

        [TestMethod]
        public void ShouldThrow_WhenNoProjectsFound()
        {
            var projectOrchestratorMock = new Mock<IProjectOrchestrator>(MockBehavior.Strict);
            var reporterFactoryMock = new Mock<IReporterFactory>(MockBehavior.Strict);
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            var inputsMock = new Mock<IStrykerInputs>(MockBehavior.Strict);
            var fileSystemMock = new MockFileSystem();

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf
            {
                Mutants = new Collection<IMutant>() { new Mutant() { Id = 1, ResultStatus = MutantStatus.Ignored } }
            });
            var mutationTestInput = new MutationTestInput()
            {
                SourceProjectInfo = new SourceProjectInfo()
                {
                    ProjectContents = folder
                }
            };

            inputsMock.Setup(x => x.ValidateAll()).Returns(new StrykerOptions
            {
                ProjectPath = "C:/test",
                OptimizationMode = OptimizationModes.None,
                LogOptions = new LogOptions()
            });

            projectOrchestratorMock.Setup(x => x.MutateProjects(It.IsAny<StrykerOptions>(), It.IsAny<IReporter>(), It.IsAny<ITestRunner>()))
                .Returns(new List<IMutationTestProcess>() { });

            reporterFactoryMock.Setup(x => x.Create(It.IsAny<StrykerOptions>(), It.IsAny<IGitInfoProvider>())).Returns(reporterMock.Object);

            var target = new StrykerRunner(reporterFactory: reporterFactoryMock.Object);

            Should.Throw<NoTestProjectsException>(() => target.RunMutationTest(inputsMock.Object, new LoggerFactory(), projectOrchestratorMock.Object));

            reporterMock.Verify(x => x.OnStartMutantTestRun(It.IsAny<IList<IMutant>>()), Times.Never);
            reporterMock.Verify(x => x.OnMutantTested(It.IsAny<IMutant>()), Times.Never);
            reporterMock.Verify(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>(), It.IsAny<TestProjectsInfo>()), Times.Never);
        }
    }
}
