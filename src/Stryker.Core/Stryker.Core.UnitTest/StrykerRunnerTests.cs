using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;
using Stryker.Abstractions.Testing;
using Stryker.Configuration.Options;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.ProjectComponents.Csharp;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters;

namespace Stryker.Core.UnitTest;

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

        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        testRunnerMock.Setup(x => x.ResetTestProcessesAsync()).Returns(Task.CompletedTask);

        var mutationTestInput = new MutationTestInput()
        {
            SourceProjectInfo = projectInfo,
            TestRunner = testRunnerMock.Object
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
        mutationTestProcessMock.Setup(x => x.TestAsync(It.IsAny<IEnumerable<IMutant>>()))
            .Returns(Task.FromResult(new StrykerRunResult(It.IsAny<StrykerOptions>(), It.IsAny<double>())));
        mutationTestProcessMock.Setup(x => x.Restore());

        // Set up sequence-critical methods:
        // * FilterMutants must be called before OnMutantsCreated to get valid reports
        var seq = new MockSequence();
        mutationTestProcessMock.InSequence(seq).Setup(x => x.FilterMutants());
        reporterMock.InSequence(seq).Setup(x => x.OnMutantsCreated(It.IsAny<IReadOnlyProjectComponent>(), It.IsAny<TestProjectsInfo>()));

        // Setup Dispose for ProjectOrchestrator
        projectOrchestratorMock.Setup(x => x.Dispose());

        var target = new StrykerRunner(reporterFactoryMock.Object, projectOrchestratorMock.Object, TestLoggerFactory.CreateLogger<StrykerRunner>());

        target.RunMutationTestAsync(inputsMock.Object);

        projectOrchestratorMock.Verify(x => x.MutateProjects(It.Is<StrykerOptions>(x => x.ProjectPath == "C:/test"), It.IsAny<IReporter>(), It.IsAny<ITestRunner>()), Times.Once);
        mutationTestProcessMock.Verify(x => x.GetCoverage(), Times.Once);
        mutationTestProcessMock.Verify(x => x.TestAsync(It.IsAny<IEnumerable<IMutant>>()), Times.Once);
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
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        testRunnerMock.Setup(x => x.ResetTestProcessesAsync()).Returns(Task.CompletedTask);
        
        var mutationTestInput = new MutationTestInput()
        {
            SourceProjectInfo = new SourceProjectInfo()
            {
                ProjectContents = folder
            },
            TestRunner = testRunnerMock.Object
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

        // Setup Dispose for ProjectOrchestrator
        projectOrchestratorMock.Setup(x => x.Dispose());

        var target = new StrykerRunner(reporterFactoryMock.Object, projectOrchestratorMock.Object, TestLoggerFactory.CreateLogger<StrykerRunner>());

        var result = target.RunMutationTestAsync(inputsMock.Object);

        result.Result.MutationScore.ShouldBe(double.NaN);

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
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        testRunnerMock.Setup(x => x.ResetTestProcessesAsync()).Returns(Task.CompletedTask);
        
        var mutationTestInput = new MutationTestInput()
        {
            SourceProjectInfo = new SourceProjectInfo()
            {
                ProjectContents = folder
            },
            TestRunner = testRunnerMock.Object
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

        // Setup Dispose for ProjectOrchestrator (even though exception is thrown, Dispose may still be called in cleanup)
        projectOrchestratorMock.Setup(x => x.Dispose());

        var target = new StrykerRunner(reporterFactoryMock.Object, projectOrchestratorMock.Object, TestLoggerFactory.CreateLogger<StrykerRunner>());

        Should.Throw<NoTestProjectsException>(() => target.RunMutationTestAsync(inputsMock.Object));

        reporterMock.Verify(x => x.OnStartMutantTestRun(It.IsAny<IList<IMutant>>()), Times.Never);
        reporterMock.Verify(x => x.OnMutantTested(It.IsAny<IMutant>()), Times.Never);
        reporterMock.Verify(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>(), It.IsAny<TestProjectsInfo>()), Times.Never);
    }
}
