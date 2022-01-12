using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using Stryker.Core.UnitTest.MutationTest;
using Xunit;

namespace Stryker.Core.UnitTest
{
    public class StrykerRunnerTests : TestBase
    {
        [Fact]
        public void Stryker_ShouldInvokeAllProcesses()
        {
            var projectOrchestratorMock = new Mock<IProjectOrchestrator>(MockBehavior.Strict);
            var mutationTestProcessMock = new Mock<IMutationTestProcess>(MockBehavior.Strict);
            var reporterFactoryMock = new Mock<IReporterFactory>(MockBehavior.Strict);
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            var inputsMock = new Mock<IStrykerInputs>(MockBehavior.Strict);

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf()
            {
                Mutants = new List<Mutant> { new() { Id = 1 } }
            });

            var projectInfo = Mock.Of<ProjectInfo>();
            projectInfo.ProjectContents = folder;
            Mock.Get(projectInfo).Setup(p => p.RestoreOriginalAssembly());
            var mutationTestInput = new MutationTestInput()
            {
                ProjectInfo = projectInfo
            };

            inputsMock.Setup(x => x.ValidateAll()).Returns(new StrykerOptions
            {
                BasePath = "C:/test",
                LogOptions = new LogOptions(),
                OptimizationMode = OptimizationModes.SkipUncoveredMutants
            });

            inputsMock.SetupProperty(x => x.MutantToDiagnose);
            inputsMock.Object.MutantToDiagnose = new MutantToDiagnoseInput();

            projectOrchestratorMock.Setup(x => x.MutateProjects(It.IsAny<StrykerOptions>(), It.IsAny<IReporter>()))
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
            mutationTestProcessMock.Setup(x => x.Restore());

            // Set up sequence-critical methods:
            // * FilterMutants must be called before OnMutantsCreated to get valid reports
            var seq = new MockSequence();
            mutationTestProcessMock.InSequence(seq).Setup(x => x.FilterMutants());
            reporterMock.InSequence(seq).Setup(x => x.OnMutantsCreated(It.IsAny<IReadOnlyProjectComponent>()));

            var target = new StrykerRunner(reporterFactory: reporterFactoryMock.Object);

            target.SetupLogging(new LoggerFactory());
            target.RunMutationTest(inputsMock.Object, projectOrchestratorMock.Object);

            projectOrchestratorMock.Verify(x => x.MutateProjects(It.Is<StrykerOptions>(y => y.BasePath == "C:/test"), It.IsAny<IReporter>()), Times.Once);
            mutationTestProcessMock.Verify(x => x.GetCoverage(), Times.Once);
            mutationTestProcessMock.Verify(x => x.Test(It.IsAny<IEnumerable<Mutant>>()), Times.Once);
            reporterMock.Verify(x => x.OnMutantsCreated(It.IsAny<IReadOnlyProjectComponent>()), Times.Once);
            reporterMock.Verify(x => x.OnStartMutantTestRun(It.IsAny<IEnumerable<IReadOnlyMutant>>()), Times.Once);
            reporterMock.Verify(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>()), Times.Once);
        }

        [Fact]
        public void Stryker_ShouldRunDiagnosticAllProcesses()
        {
            var projectOrchestratorMock = new Mock<IProjectOrchestrator>(MockBehavior.Strict);
            var mutationTestProcessMock = new Mock<IMutationTestProcess>(MockBehavior.Strict);
            var inputsMock = new Mock<IStrykerInputs>(MockBehavior.Strict);

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf()
            {
                Mutants = new List<Mutant> { new() { Id = 1 } }
            });

            var projectInfo = Mock.Of<ProjectInfo>();
            projectInfo.ProjectContents = folder;
            Mock.Get(projectInfo).Setup(p => p.RestoreOriginalAssembly());
            var mutationTestInput = new MutationTestInput()
            {
                ProjectInfo = projectInfo,
            };

            inputsMock.Setup(x => x.ValidateAll()).Returns(new StrykerOptions
            {
                BasePath = "C:/test",
                LogOptions = new LogOptions(),
                OptimizationMode = OptimizationModes.SkipUncoveredMutants,
            });

            inputsMock.SetupProperty(x => x.MutantToDiagnose);
            inputsMock.Object.MutantToDiagnose = new MutantToDiagnoseInput {SuppliedInput = 1};

            projectOrchestratorMock.Setup(x => x.MutateProjects(It.IsAny<StrykerOptions>(), It.IsAny<IReporter>()))
                .Returns(new List<IMutationTestProcess>()
                {
                    mutationTestProcessMock.Object
                });

            mutationTestProcessMock.SetupGet(x => x.Input).Returns(mutationTestInput);
            mutationTestProcessMock.Setup(x => x.GetCoverage());
            var mutant = new Mutant();
            var mutantDiagnostic = new MutantDiagnostic( mutant, Enumerable.Empty<string>(), new []{1});
            mutantDiagnostic.DeclareResult(MutantStatus.Survived, Enumerable.Empty<string>());
            mutantDiagnostic.DeclareResult(MutantStatus.Survived, Enumerable.Empty<string>());
            mutantDiagnostic.DeclareResult(MutantStatus.Survived, Enumerable.Empty<string>());

            mutationTestProcessMock.Setup(x => x.DiagnoseMutant(It.IsAny<IEnumerable<Mutant>>(), 1)).Returns(mutantDiagnostic);
            mutationTestProcessMock.Setup(x => x.FilterMutants());

            var reporterFactoryMock = new Mock<IReporterFactory>(MockBehavior.Strict);
            var target = new StrykerRunner(reporterFactory: reporterFactoryMock.Object);

            target.SetupLogging(new LoggerFactory());
            target.RunMutationTest(inputsMock.Object, projectOrchestratorMock.Object);

            projectOrchestratorMock.Verify(x => x.MutateProjects(It.Is<StrykerOptions>(y => y.BasePath == "C:/test"), It.IsAny<IReporter>()), Times.Once);
            mutationTestProcessMock.Verify(x => x.GetCoverage(), Times.Once);
            mutationTestProcessMock.Verify(x => x.DiagnoseMutant(It.IsAny<IEnumerable<Mutant>>(), 1), Times.Once);
        }

        [Fact]
        public void ShouldGenerateCleanDiagnosisWhenConsistentlyKilled()
        {
            var mutant = new Mutant();
            var mutantDiagnostic = new MutantDiagnostic(mutant, Enumerable.Empty<string>(), new []{1});
            mutantDiagnostic.DeclareResult(MutantStatus.Killed, Enumerable.Empty<string>());
            mutantDiagnostic.DeclareResult(MutantStatus.Killed, Enumerable.Empty<string>());
            mutantDiagnostic.DeclareResult(MutantStatus.Killed, Enumerable.Empty<string>());
            var reporterFactoryMock = new Mock<IReporterFactory>(MockBehavior.Strict);
            var target = new StrykerRunner(reporterFactory: reporterFactoryMock.Object);

            var report = target.GenerateDiagnoseReport(mutantDiagnostic);

            report.ShouldBe($"Mutant consistently appears as {MutantStatus.Killed}. There is no visible issue.");
        }

        [Fact]
        public void ShouldGenerateCleanDiagnosisWhenConsistentIgnored()
        {
            var mutant = new Mutant();
            var status = MutantStatus.Ignored;
            var mutantDiagnostic = new MutantDiagnostic(mutant, Enumerable.Empty<string>(), new []{1});
            mutantDiagnostic.DeclareResult(status, Enumerable.Empty<string>());
            mutantDiagnostic.DeclareResult(status, Enumerable.Empty<string>());
            mutantDiagnostic.DeclareResult(status, Enumerable.Empty<string>());
            var reporterFactoryMock = new Mock<IReporterFactory>(MockBehavior.Strict);
            var target = new StrykerRunner(reporterFactory: reporterFactoryMock.Object);

            var report = target.GenerateDiagnoseReport(mutantDiagnostic);

            report.ShouldBe($"Mutant consistently appears as {status}. Check Stryker configuration file to see why it is ignored.");
        }

        [Fact]
        public void ShouldGenerateDiagnosisToAddStrykerCommentsForNonIsolatedTests()
        {
            var mutant = new Mutant();
            var mutantDiagnostic = new MutantDiagnostic(mutant, Enumerable.Empty<string>(), new []{1});
            mutantDiagnostic.ConflictingMutant = new Mutant { Id = 2};
            mutantDiagnostic.DeclareResult(MutantStatus.Survived, Enumerable.Empty<string>());
            mutantDiagnostic.DeclareResult(MutantStatus.Killed, new []{"SomeTest"});
            mutantDiagnostic.DeclareResult(MutantStatus.Killed, new []{"SomeTest"});
            var reporterFactoryMock = new Mock<IReporterFactory>(MockBehavior.Strict);
            var target = new StrykerRunner(reporterFactory: reporterFactoryMock.Object);
            target.SetupLogging(new LoggerFactory());

            var report = target.GenerateDiagnoseReport(mutantDiagnostic);

            report.ShouldBe(@"Run results are not consistent!
The tests for this mutant was corrupted by another mutant. As a work around, you should
Add '// Stryker test apart once' before mutant 2 at Unknown location..
Diagnosed mutant 0 was killed by these test(s): 
SomeTest");
        }

        [Fact]
        public void ShouldGenerateDiagnosisToAddTest()
        {
            var mutant = new Mutant();
            const MutantStatus ConsistentStatus = MutantStatus.NoCoverage;
            var mutantDiagnostic = new MutantDiagnostic(mutant, Enumerable.Empty<string>(), new []{1});
            mutantDiagnostic.DeclareResult(ConsistentStatus, Enumerable.Empty<string>());
            mutantDiagnostic.DeclareResult(ConsistentStatus, Enumerable.Empty<string>());
            mutantDiagnostic.DeclareResult(ConsistentStatus, Enumerable.Empty<string>());
            var reporterFactoryMock = new Mock<IReporterFactory>(MockBehavior.Strict);
            var target = new StrykerRunner(reporterFactory: reporterFactoryMock.Object);

            var report = target.GenerateDiagnoseReport(mutantDiagnostic);

            report.ShouldBe($"Mutant consistently appears as {ConsistentStatus}. You need to add some tests to fix that.");
        }

        [Fact]
        public void ShouldGenerateDiagnosisToReportIssueOnNotRun()
        {
            var mutant = new Mutant();
            const MutantStatus ConsistentStatus = MutantStatus.NotRun;
            var mutantDiagnostic = new MutantDiagnostic(mutant, Enumerable.Empty<string>(), new []{1});
            mutantDiagnostic.DeclareResult(ConsistentStatus, Enumerable.Empty<string>());
            mutantDiagnostic.DeclareResult(ConsistentStatus, Enumerable.Empty<string>());
            mutantDiagnostic.DeclareResult(ConsistentStatus, Enumerable.Empty<string>());
            var reporterFactoryMock = new Mock<IReporterFactory>(MockBehavior.Strict);
            var target = new StrykerRunner(reporterFactory: reporterFactoryMock.Object);

            var report = target.GenerateDiagnoseReport(mutantDiagnostic);

            report.ShouldBe($"Mutant consistently appears as {ConsistentStatus}. This should happen. You can check on Github to see if there is an open issue about this and open one if you want help.");
        }

        [Fact]
        public void ShouldGenerateDiagnosisToModifyTests()
        {
            var mutant = new Mutant();
            const MutantStatus ConsistentStatus = MutantStatus.Survived;
            var mutantDiagnostic = new MutantDiagnostic(mutant, new []{"FirstTest", "OtherTest"}, new []{1});
            mutantDiagnostic.DeclareResult(ConsistentStatus, Enumerable.Empty<string>());
            mutantDiagnostic.DeclareResult(ConsistentStatus, Enumerable.Empty<string>());
            mutantDiagnostic.DeclareResult(ConsistentStatus, Enumerable.Empty<string>());
            var reporterFactoryMock = new Mock<IReporterFactory>(MockBehavior.Strict);
            var target = new StrykerRunner(reporterFactory: reporterFactoryMock.Object);

            var report = target.GenerateDiagnoseReport(mutantDiagnostic);

            report.ShouldBe($"Mutant consistently appears as {ConsistentStatus}. Modifying the following tests may help you kill this one: FirstTest, OtherTest.");
        }

        [Theory]
        [InlineData(MutantStatus.Survived)]
        [InlineData(MutantStatus.NoCoverage)]
        public void ShouldGenerateCoverageDiagnosisWhenAllTestsFix(MutantStatus firstStatus)
        {
            var mutant = FullRunScenario.BuildMutant(1);
            const MutantStatus lastStatus = MutantStatus.Killed;
            var mutantDiagnostic = new MutantDiagnostic(mutant, new []{"1","2"}, new []{1});
            mutantDiagnostic.DeclareResult(firstStatus, Enumerable.Empty<string>());
            mutantDiagnostic.DeclareResult(MutantStatus.Survived, Enumerable.Empty<string>());
            mutantDiagnostic.DeclareResult(lastStatus, new []{"1", "3"});
            var reporterFactoryMock = new Mock<IReporterFactory>(MockBehavior.Strict);
            var target = new StrykerRunner(reporterFactory: reporterFactoryMock.Object);
            target.SetupLogging(new LoggerFactory());
            var report= target.GenerateDiagnoseReport(mutantDiagnostic);

            report.ShouldBe(@"Run results are not consistent!
The coverage for this mutant was not properly determined. You can workaround this problem.
Add '// Stryker test full once' to  line 0:0.
It was killed by these test(s): 3");
        }

        [Fact]
        public void ShouldStop_WhenAllMutationsWereIgnored()
        {
            var projectOrchestratorMock = new Mock<IProjectOrchestrator>(MockBehavior.Strict);
            var mutationTestProcessMock = new Mock<IMutationTestProcess>(MockBehavior.Strict);
            var reporterFactoryMock = new Mock<IReporterFactory>(MockBehavior.Strict);
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            var inputsMock = new Mock<IStrykerInputs>(MockBehavior.Strict);

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf
            {
                Mutants = new Collection<Mutant>() { new() { Id = 1, ResultStatus = MutantStatus.Ignored } }
            });
            var mutationTestInput = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo(new MockFileSystem())
                {
                    ProjectContents = folder
                }
            };

            inputsMock.Setup(x => x.ValidateAll()).Returns(new StrykerOptions
            {
                BasePath = "C:/test",
                OptimizationMode = OptimizationModes.None,
                LogOptions = new LogOptions()
            });

            inputsMock.SetupProperty(t => t.MutantToDiagnose);
            inputsMock.Object.MutantToDiagnose = new MutantToDiagnoseInput();

            projectOrchestratorMock.Setup(x => x.MutateProjects(It.IsAny<StrykerOptions>(), It.IsAny<IReporter>()))
                .Returns(new List<IMutationTestProcess>() { mutationTestProcessMock.Object });

            mutationTestProcessMock.Setup(x => x.FilterMutants());
            mutationTestProcessMock.SetupGet(x => x.Input).Returns(mutationTestInput);

            reporterFactoryMock.Setup(x => x.Create(It.IsAny<StrykerOptions>(), It.IsAny<IGitInfoProvider>())).Returns(reporterMock.Object);

            reporterMock.Setup(x => x.OnMutantsCreated(It.IsAny<IReadOnlyProjectComponent>()));
            reporterMock.Setup(x => x.OnStartMutantTestRun(It.IsAny<IEnumerable<IReadOnlyMutant>>()));
            reporterMock.Setup(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>()));


            var target = new StrykerRunner(reporterFactory: reporterFactoryMock.Object);

            target.SetupLogging(new LoggerFactory());
            var result = target.RunMutationTest(inputsMock.Object, projectOrchestratorMock.Object);

            result.MutationScore.ShouldBe(double.NaN);

            reporterMock.Verify(x => x.OnStartMutantTestRun(It.IsAny<IList<IReadOnlyMutant>>()), Times.Once);
            reporterMock.Verify(x => x.OnMutantTested(It.IsAny<Mutant>()), Times.Never);
            reporterMock.Verify(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>()), Times.Once);
        }
    }
}
