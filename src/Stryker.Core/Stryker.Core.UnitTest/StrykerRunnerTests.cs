using Moq;
using Shouldly;
using Stryker.Core.DashboardCompare;
using Stryker.Core.Initialisation;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
            var reporterFactoryMock = new Mock<IReporterFactory>(MockBehavior.Strict);
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            var fileSystemMock = new MockFileSystem();

            var folder = new FolderComposite
            {
                Name = "ProjectRoot"
            };
            folder.Add(new FileLeaf()
            {
                Name = "SomeFile.cs",
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

            reporterMock.Setup(x => x.OnMutantsCreated(It.IsAny<IReadOnlyProjectComponent>()));
            reporterMock.Setup(x => x.OnStartMutantTestRun(It.IsAny<IEnumerable<IReadOnlyMutant>>()));
            reporterMock.Setup(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>()));

            mutationTestProcessMock.SetupGet(x => x.Input).Returns(mutationTestInput);
            mutationTestProcessMock.Setup(x => x.GetCoverage());
            mutationTestProcessMock.Setup(x => x.Test(It.IsAny<IEnumerable<Mutant>>()))
                .Returns(new StrykerRunResult(It.IsAny<StrykerOptions>(), It.IsAny<double>()));

            var target = new StrykerRunner(projectOrchestratorMock.Object, fileSystem: fileSystemMock, reporterFactory: reporterFactoryMock.Object);

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

            var folder = new FolderComposite()
            {
                Name = "ProjectRoot"
            };
            folder.Add(new FileLeaf
            {
                Name = "SomeFile.cs",
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

            mutationTestProcessMock.SetupGet(x => x.Input).Returns(mutationTestInput);

            reporterFactoryMock.Setup(x => x.Create(It.IsAny<StrykerOptions>(), It.IsAny<IGitInfoProvider>())).Returns(reporterMock.Object);

            reporterMock.Setup(x => x.OnMutantsCreated(It.IsAny<IReadOnlyProjectComponent>()));
            reporterMock.Setup(x => x.OnStartMutantTestRun(It.IsAny<IEnumerable<IReadOnlyMutant>>()));

            var target = new StrykerRunner(projectOrchestratorMock.Object, fileSystem: fileSystemMock, reporterFactory: reporterFactoryMock.Object);

            var result = target.RunMutationTest(options);

            result.MutationScore.ShouldBe(double.NaN);

            reporterMock.Verify(x => x.OnStartMutantTestRun(It.IsAny<IList<Mutant>>()), Times.Never);
            reporterMock.Verify(x => x.OnMutantTested(It.IsAny<Mutant>()), Times.Never);
            reporterMock.Verify(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>()), Times.Never);
        }

        [Fact]
        public void ShouldStop_WhenNoTestableMutants()
        {
            var projectOrchestratorMock = new Mock<IProjectOrchestrator>(MockBehavior.Strict);
            var mutationTestProcessMock = new Mock<IMutationTestProcess>(MockBehavior.Strict);
            var reporterFactoryMock = new Mock<IReporterFactory>(MockBehavior.Strict);
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            var fileSystemMock = new MockFileSystem();

            Mutant CreateMutant(MutantStatus status) => new Mutant() { Id = 1, ResultStatus = status };
            var folder = new FolderComposite()
            {
                Name = "ProjectRoot"
            };
            folder.Add(new FileLeaf()
            {
                Name = "SomeFile.cs",
                Mutants = new Collection<Mutant>() { CreateMutant(MutantStatus.Ignored), CreateMutant(MutantStatus.NoCoverage) }
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

            mutationTestProcessMock.SetupGet(x => x.Input).Returns(mutationTestInput);

            reporterFactoryMock.Setup(x => x.Create(It.IsAny<StrykerOptions>(), It.IsAny<IGitInfoProvider>())).Returns(reporterMock.Object);

            reporterMock.Setup(x => x.OnMutantsCreated(It.IsAny<IReadOnlyProjectComponent>()));
            reporterMock.Setup(x => x.OnStartMutantTestRun(It.IsAny<IEnumerable<IReadOnlyMutant>>()));

            var target = new StrykerRunner(projectOrchestratorMock.Object, fileSystem: fileSystemMock, reporterFactory: reporterFactoryMock.Object);

            var result = target.RunMutationTest(options);

            result.MutationScore.ShouldBe(double.NaN);

            reporterMock.Verify(x => x.OnStartMutantTestRun(It.IsAny<IList<Mutant>>()), Times.Never);
            reporterMock.Verify(x => x.OnMutantTested(It.IsAny<Mutant>()), Times.Never);
            reporterMock.Verify(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>()), Times.Never);
        }


        [Fact]
        public void ShouldNotTest_WhenAllMutationsWereSkipped()
        {
            var mutant = new Mutant() { Id = 1, ResultStatus = MutantStatus.Ignored };
            string basePath = Path.Combine("C:", "ExampleProject.Test");
            var folder = new FolderComposite()
            {
                Name = "ProjectRoot"
            };
            folder.Add(new FileLeaf()
            {
                Name = "SomeFile.cs",
                Mutants = new Collection<Mutant>() { mutant }
            });
            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectContents = folder
                },
                AssemblyReferences = null
            };
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<Mutant>()));
            reporterMock.Setup(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>()));
            reporterMock.Setup(x => x.OnStartMutantTestRun(It.IsAny<IList<Mutant>>()));

            var runnerMock = new Mock<ITestRunner>();
            runnerMock.Setup(x => x.DiscoverNumberOfTests()).Returns(1);
            var executorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            executorMock.SetupGet(x => x.TestRunner).Returns(runnerMock.Object);
            executorMock.Setup(x => x.Test(It.IsAny<IList<Mutant>>(), It.IsAny<int>(), It.IsAny<TestUpdateHandler>()));

            var mutantFilterMock = new Mock<IMutantFilter>(MockBehavior.Loose);

            var options = new StrykerOptions(fileSystem: new MockFileSystem(), basePath: basePath);

            var target = new MutationTestProcess(input,
                reporterMock.Object,
                executorMock.Object,
                mutantFilter: mutantFilterMock.Object,
                options: options);

            var testResult = target.Test(input.ProjectInfo.ProjectContents.Mutants);

            executorMock.Verify(x => x.Test(new List<Mutant> { mutant }, It.IsAny<int>(), It.IsAny<TestUpdateHandler>()), Times.Never);
            reporterMock.Verify(x => x.OnStartMutantTestRun(It.IsAny<IList<Mutant>>()), Times.Never);
            reporterMock.Verify(x => x.OnMutantTested(mutant), Times.Never);
            reporterMock.Verify(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>()), Times.Once);
            testResult.MutationScore.ShouldBe(double.NaN);
        }

        [Fact]
        public void ShouldNotTest_WhenThereAreNoTestableMutations()
        {
            var mutant = new Mutant() { Id = 1, ResultStatus = MutantStatus.Ignored };
            var mutant2 = new Mutant() { Id = 2, ResultStatus = MutantStatus.CompileError };
            string basePath = Path.Combine("C:", "ExampleProject.Test");

            var folder = new FolderComposite()
            {
                Name = "ProjectRoot"
            };
            folder.Add(new FileLeaf()
            {
                Name = "SomeFile.cs",
                Mutants = new Collection<Mutant>() { mutant, mutant2 }
            });
            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectContents = folder
                },
                AssemblyReferences = null
            };
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<Mutant>()));
            reporterMock.Setup(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>()));
            reporterMock.Setup(x => x.OnStartMutantTestRun(It.IsAny<IList<Mutant>>()));

            var runnerMock = new Mock<ITestRunner>();
            runnerMock.Setup(x => x.DiscoverNumberOfTests()).Returns(1);
            var executorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            executorMock.SetupGet(x => x.TestRunner).Returns(runnerMock.Object);
            executorMock.Setup(x => x.Test(It.IsAny<IList<Mutant>>(), It.IsAny<int>(), It.IsAny<TestUpdateHandler>()));

            var mutantFilterMock = new Mock<IMutantFilter>(MockBehavior.Loose);

            var options = new StrykerOptions(fileSystem: new MockFileSystem(), basePath: basePath);

            var target = new MutationTestProcess(input,
                reporterMock.Object,
                executorMock.Object,
                mutantFilter: mutantFilterMock.Object,
                options: options);

            var testResult = target.Test(input.ProjectInfo.ProjectContents.Mutants);

            executorMock.Verify(x => x.Test(It.IsAny<IList<Mutant>>(), It.IsAny<int>(), It.IsAny<TestUpdateHandler>()), Times.Never);
            reporterMock.Verify(x => x.OnStartMutantTestRun(It.IsAny<IList<Mutant>>()), Times.Never);
            reporterMock.Verify(x => x.OnMutantTested(It.IsAny<Mutant>()), Times.Never);
            reporterMock.Verify(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>()), Times.Once);
            testResult.MutationScore.ShouldBe(double.NaN);
        }
    }
}
