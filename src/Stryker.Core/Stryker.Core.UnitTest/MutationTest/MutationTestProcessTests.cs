using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.Tests;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Reporting;
using Stryker.Configuration.Options;
using Stryker.Core.CoverageAnalysis;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.Csharp;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ProjectComponents.TestProjects;

namespace Stryker.Core.UnitTest.MutationTest;

[TestClass]
public class MutationTestProcessTests : TestBase
{
    private string CurrentDirectory { get; }
    private string FilesystemRoot { get; }
    private string SourceFile { get; }
    private MockFileSystem FileSystemMock { get; } = new MockFileSystem();
    private FolderComposite Folder { get; } = new FolderComposite();
    private FullRunScenario TestScenario { get; } = new FullRunScenario();

    private MutationTestInput Input { get; }

    public MutationTestProcessTests()
    {
        CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        FilesystemRoot = Path.GetPathRoot(CurrentDirectory);
        SourceFile = File.ReadAllText(CurrentDirectory + "/TestResources/ExampleSourceFile.cs");
        var testProjectsInfo = new TestProjectsInfo(FileSystemMock)
        {
            TestProjects = new List<TestProject> {
                new TestProject(FileSystemMock, TestHelper.SetupProjectAnalyzerResult(
                    projectFilePath: Path.Combine(FilesystemRoot, "TestProject", "TestProject.csproj"),
                    properties: new Dictionary<string, string>()
                {
                    { "TargetDir", Path.Combine(FilesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0") },
                    { "TargetFileName", "TestName.dll" },
                    { "Language", "C#" }
                }).Object)
            }
        };
        Input = new MutationTestInput()
        {
            SourceProjectInfo = new SourceProjectInfo()
            {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    projectFilePath: Path.Combine(FilesystemRoot, "ProjectUnderTest", "ProjectUnderTest.csproj"),
                    properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                        { "TargetFileName", "ProjectUnderTest.dll" },
                        { "AssemblyName", "ProjectUnderTest" },
                        { "Language", "C#" }
                    }).Object,
                ProjectContents = Folder,
                TestProjectsInfo = testProjectsInfo
            },
            TestProjectsInfo = testProjectsInfo,
        };
    }

    [TestMethod]
    public void ShouldCallMutationProcess_MutateAndFilterMutants()
    {
        // Arrange
        var options = new StrykerOptions()
        {
            ExcludedMutations = new Mutator[] { }
        };

        var executorMock = new Mock<IMutationTestExecutor>();
        var coverageAnalyzerMock = new Mock<ICoverageAnalyser>();

        // Create a strict mock for the IMutationProcess and verify the calls on that instance.
        var mutationProcessMock = new Mock<IMutationProcess>(MockBehavior.Strict);
        mutationProcessMock.Setup(x => x.Mutate(It.IsAny<MutationTestInput>(), It.IsAny<IStrykerOptions>()));
        mutationProcessMock.Setup(x => x.FilterMutants(It.IsAny<MutationTestInput>()));

        var target = new MutationTestProcess(executorMock.Object, coverageAnalyzerMock.Object, mutationProcessMock.Object, TestLoggerFactory.CreateLogger<MutationTestProcess>());

        // Act
        target.Initialize(Input, options, null);
        target.Mutate();

        target.FilterMutants();

        // Assert
        mutationProcessMock.Verify(x => x.Mutate(It.IsAny<MutationTestInput>(), It.IsAny<IStrykerOptions>()), Times.Once);
        mutationProcessMock.Verify(x => x.FilterMutants(It.IsAny<MutationTestInput>()), Times.Once);
    }

    [TestMethod]
    public async Task ShouldCallExecutorForEveryCoveredMutantAsync()
    {
        TestScenario.CreateMutants(1, 2);
        // we need at least one test
        TestScenario.CreateTest(1);
        // and we need to declare that the mutant is covered
        TestScenario.DeclareCoverageForMutant(1);
        var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");

        Folder.Add(new CsharpFileLeaf()
        {
            SourceCode = SourceFile,
            Mutants = TestScenario.GetMutants()
        });

        var loggerMock = new Mock<ILogger<MutationTestExecutor>>();
        var mutationExecutor = new MutationTestExecutor(loggerMock.Object);
        mutationExecutor.TestRunner = TestScenario.GetTestRunnerMock().Object;
        var coverageAnalyzer = new CoverageAnalyser(TestLoggerFactory.CreateLogger<CoverageAnalyser>());

        var options = new StrykerOptions()
        {
            ProjectPath = basePath,
            Concurrency = 1,
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };
        Input.InitialTestRun = new InitialTestRun(TestScenario.GetInitialRunResult(), new TimeoutValueCalculator(500));

        var mutationProcessMock = Mock.Of<IMutationProcess>();
        var target = new MutationTestProcess(mutationExecutor, coverageAnalyzer, mutationProcessMock, TestLoggerFactory.CreateLogger<MutationTestProcess>());

        target.Initialize(Input, options, null);
        target.GetCoverage();
        await target.TestAsync(TestScenario.GetCoveredMutants());

        TestScenario.GetMutantStatus(1).ShouldBe(MutantStatus.Survived);
        TestScenario.GetMutantStatus(2).ShouldBe(MutantStatus.NoCoverage);
    }

    [TestMethod]
    public async Task OnMutantTested_ReportsEachMutantOnce_WhenProgressCallbackOverlapsFinalReporting()
    {
        // Regression for #3727. The per-group `reportedMutants` set is written from two threads: the
        // runner's progress callback (TestUpdateHandler) and OnMutantsTested after `await TestAsync`.
        // On the timeout/abort path the executor can return while a final callback is still in flight,
        // so the two overlap. This reproduces that overlap DETERMINISTICALLY: the progress callback
        // (writer B) is parked inside the reporter while OnMutantsTested (writer C) runs for the same
        // mutant. The two builds then diverge by ordering — without the lock, B reaches the reporter
        // before it has Added, so C still passes the Contains check and reports the mutant a second
        // time; with the lock, B has already Added (under the lock) by the time it reports, so C's Add
        // short-circuits and the mutant is reported exactly once.
        var mutant = new Mutant { Id = 1 };
        Folder.Add(new CsharpFileLeaf { SourceCode = SourceFile, Mutants = new List<IMutant> { mutant } });

        var callbackInReporter = new ManualResetEventSlim(false);
        var releaseCallback = new ManualResetEventSlim(false);
        var reportCount = 0;

        var reporterMock = new Mock<IReporter>();
        reporterMock.Setup(x => x.OnMutantTested(It.IsAny<IMutant>())).Callback(() =>
        {
            // Park the first reporter call (writer B, the progress callback) here — the one point both
            // builds pass through — long enough for OnMutantsTested (writer C) to race it for this mutant.
            if (Interlocked.Increment(ref reportCount) == 1)
            {
                callbackInReporter.Set();
                releaseCallback.Wait(TimeSpan.FromSeconds(10));
            }
        });

        Task callbackTask = null;
        var executorMock = new Mock<IMutationTestExecutor>();
        executorMock
            .Setup(x => x.TestAsync(It.IsAny<IProjectAndTests>(), It.IsAny<IList<IMutant>>(),
                It.IsAny<ITimeoutValueCalculator>(), It.IsAny<ITestRunner.TestUpdateHandler>()))
            .Returns((IProjectAndTests _, IList<IMutant> group, ITimeoutValueCalculator _, ITestRunner.TestUpdateHandler update) =>
            {
                // Deliver the progress callback on the runner's own thread and wait until it has
                // reached the reporter (its hold point) before returning WITHOUT joining it — exactly
                // the timeout/abort ordering. OnMutantsTested then runs while that callback is parked.
                // Fail loudly if it never reaches the hold point, so the test cannot pass without
                // actually setting up the overlap.
                callbackTask = Task.Run(() => update(group.ToList(),
                    TestIdentifierList.NoTest(), TestIdentifierList.EveryTest(), TestIdentifierList.NoTest()));
                if (!callbackInReporter.Wait(TimeSpan.FromSeconds(10)))
                {
                    throw new InvalidOperationException("progress callback never reached the reporter");
                }
                return Task.CompletedTask;
            });

        var options = new StrykerOptions
        {
            OutputPath = Path.Combine(FilesystemRoot, "ExampleProject.Test"),
            Concurrency = 1,
            OptimizationMode = OptimizationModes.None
        };
        Input.InitialTestRun = new InitialTestRun(TestScenario.GetInitialRunResult(), new TimeoutValueCalculator(500));

        var target = new MutationTestProcess(executorMock.Object, new Mock<ICoverageAnalyser>().Object,
            Mock.Of<IMutationProcess>(), TestLoggerFactory.CreateLogger<MutationTestProcess>());
        target.Initialize(Input, options, reporterMock.Object);

        try
        {
            // TestAsync returns only after the callback reached its hold point; OnMutantsTested has
            // then already run for the same mutant while the callback was parked.
            await target.TestAsync(new List<IMutant> { mutant });

            // The callback must actually have parked at the reporter, otherwise the overlap never
            // happened. Locked: OnMutantsTested's Add short-circuits, so the mutant is reported once.
            // Unlocked: it passed Contains while the callback was parked, so it is reported twice.
            callbackInReporter.IsSet.ShouldBeTrue();
            reportCount.ShouldBe(1);
            reporterMock.Verify(x => x.OnMutantTested(mutant), Times.Once);
        }
        finally
        {
            releaseCallback.Set();
        }

        // Observe the progress-callback task so a fault in it can never pass silently.
        await callbackTask;
    }

    [TestMethod]
    public async Task ShouldCallExecutorForEveryMutantWhenNoOptimizationAsync()
    {
        var scenario = new FullRunScenario();
        TestScenario.CreateMutants(1, 2);
        // we need at least one test
        TestScenario.CreateTest(1);
        // and we need to declare that the mutant is covered
        TestScenario.DeclareCoverageForMutant(1);
        TestScenario.SetMode(OptimizationModes.None);
        var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");

        Folder.Add(new CsharpFileLeaf()
        {
            SourceCode = SourceFile,
            Mutants = TestScenario.GetMutants()
        });

        var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
        reporterMock.Setup(x => x.OnMutantTested(It.IsAny<IMutant>()));

        var loggerMock2 = new Mock<ILogger<MutationTestExecutor>>();
        var mutationExecutor = new MutationTestExecutor(loggerMock2.Object);
        mutationExecutor.TestRunner = TestScenario.GetTestRunnerMock().Object;

        var options = new StrykerOptions()
        {
            OutputPath = basePath,
            Concurrency = 1,
            OptimizationMode = OptimizationModes.None
        };
        Input.InitialTestRun = new InitialTestRun(TestScenario.GetInitialRunResult(), new TimeoutValueCalculator(500));

        var coverageAnalyzerMock2 = new Mock<ICoverageAnalyser>();
        var mutationProcessMock = Mock.Of<IMutationProcess>();
        var target = new MutationTestProcess(mutationExecutor, coverageAnalyzerMock2.Object, mutationProcessMock, TestLoggerFactory.CreateLogger<MutationTestProcess>());

        target.Initialize(Input, options, null);
        target.GetCoverage();
        await target.TestAsync(TestScenario.GetMutants());

        TestScenario.GetMutantStatus(1).ShouldBe(MutantStatus.Survived);
        TestScenario.GetMutantStatus(2).ShouldBe(MutantStatus.Survived);
    }

    [TestMethod]
    public async Task ShouldHandleCoverageAsync()
    {
        var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");
        TestScenario.CreateMutants(1, 2, 3);

        Folder.Add(new CsharpFileLeaf()
        {
            SourceCode = SourceFile,
            Mutants = TestScenario.GetMutants()
        });
        TestScenario.CreateTests(1, 2);

        // mutant 1 is covered by both tests
        TestScenario.DeclareCoverageForMutant(1);
        // mutant 2 is covered only by test 1
        TestScenario.DeclareCoverageForMutant(2, 1);
        // mutant 3 as no coverage
        // test 1 succeeds, test 2 fails
        TestScenario.DeclareTestsFailingWhenTestingMutant(1, 2);

        // setup coverage
        var loggerMock3 = new Mock<ILogger<MutationTestExecutor>>();
        var executor = new MutationTestExecutor(loggerMock3.Object);
        executor.TestRunner = TestScenario.GetTestRunnerMock().Object;

        var options = new StrykerOptions
        {
            ProjectPath = basePath,
            Concurrency = 1,
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };
        Input.InitialTestRun = new InitialTestRun(TestScenario.GetInitialRunResult(), new TimeoutValueCalculator(500));

        var coverageAnalyzer = new CoverageAnalyser(TestLoggerFactory.CreateLogger<CoverageAnalyser>());
        var mutationProcessMock = Mock.Of<IMutationProcess>();
        var target = new MutationTestProcess(executor, coverageAnalyzer, mutationProcessMock, TestLoggerFactory.CreateLogger<MutationTestProcess>());
        // test mutants
        target.Initialize(Input, options, null);
        target.GetCoverage();

        var mutantsToTest = Input.SourceProjectInfo.ProjectContents.Mutants
            .Where(m => m.ResultStatus == MutantStatus.Pending)
            .ToList();
        await target.TestAsync(mutantsToTest);
        // first mutant should be killed by test 2
        TestScenario.GetMutantStatus(1).ShouldBe(MutantStatus.Killed);
        // other mutant survives
        TestScenario.GetMutantStatus(2).ShouldBe(MutantStatus.Survived);
        // third mutant appears as no coverage
        TestScenario.GetMutantStatus(3).ShouldBe(MutantStatus.NoCoverage);
    }

    [TestMethod]
    public async Task ShouldNotKillMutantIfOnlyKilledByFailingTest()
    {
        var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");
        TestScenario.CreateMutants(1);

        Folder.Add(new CsharpFileLeaf()
        {
            SourceCode = SourceFile,
            Mutants = TestScenario.GetMutants()
        });
        TestScenario.CreateTests(1, 2, 3);

        // mutant 1 is covered by all tests
        TestScenario.DeclareCoverageForMutant(1);
        // mutant 2 is covered only by test 1
        TestScenario.DeclareTestsFailingAtInit(1);
        // test 1 succeeds, test 2 fails
        TestScenario.DeclareTestsFailingWhenTestingMutant(1, 1);

        // setup coverage
        var loggerMock4 = new Mock<ILogger<MutationTestExecutor>>();
        var executor = new MutationTestExecutor(loggerMock4.Object);
        executor.TestRunner = TestScenario.GetTestRunnerMock().Object;

        var options = new StrykerOptions
        {
            ProjectPath = basePath,
            Concurrency = 1,
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };

        Input.InitialTestRun = new InitialTestRun(TestScenario.GetInitialRunResult(), new TimeoutValueCalculator(500));

        var coverageAnalyzer = new CoverageAnalyser(TestLoggerFactory.CreateLogger<CoverageAnalyser>());
        var mutationProcessMock = Mock.Of<IMutationProcess>();
        var target = new MutationTestProcess(executor, coverageAnalyzer, mutationProcessMock, TestLoggerFactory.CreateLogger<MutationTestProcess>());

        // test mutants
        target.Initialize(Input, options, null);
        target.GetCoverage();

        await target.TestAsync(Input.SourceProjectInfo.ProjectContents.Mutants);
        // first mutant should be marked as survived
        TestScenario.GetMutantStatus(1).ShouldBe(MutantStatus.Survived);
    }

    [TestMethod]
    public void ShouldNotKillMutantIfOnlyCoveredByFailingTest()
    {
        var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");
        TestScenario.CreateMutants(1);

        Folder.Add(new CsharpFileLeaf()
        {
            SourceCode = SourceFile,
            Mutants = TestScenario.GetMutants()
        });
        TestScenario.CreateTests(1, 2);

        // mutant 1 is covered by both tests
        TestScenario.DeclareCoverageForMutant(1, 1, 2);
        // mutant 2 is covered only by test 1
        TestScenario.DeclareTestsFailingAtInit(1, 2);
        // test 1 succeeds, test 2 fails
        TestScenario.DeclareTestsFailingWhenTestingMutant(1, 1, 2);

        // setup coverage
        var loggerMock5 = new Mock<ILogger<MutationTestExecutor>>();
        var executor = new MutationTestExecutor(loggerMock5.Object);
        executor.TestRunner = TestScenario.GetTestRunnerMock().Object;

        var options = new StrykerOptions
        {
            ProjectPath = basePath,
            Concurrency = 1,
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };
        Input.InitialTestRun = new InitialTestRun(TestScenario.GetInitialRunResult(), new TimeoutValueCalculator(500));

        var coverageAnalyzer = new CoverageAnalyser(TestLoggerFactory.CreateLogger<CoverageAnalyser>());
        var mutationProcessMock = Mock.Of<IMutationProcess>();
        var target = new MutationTestProcess(executor, coverageAnalyzer, mutationProcessMock, TestLoggerFactory.CreateLogger<MutationTestProcess>());
        // test mutants
        target.Initialize(Input, options, null);
        target.GetCoverage();

        // first mutant should be marked as survived without any test
        TestScenario.GetMutantStatus(1).ShouldBe(MutantStatus.Survived);
    }

    [TestMethod]
    public async Task ShouldKillMutantKilledByFailingTestAndNormalTestAsync()
    {
        var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");
        TestScenario.CreateMutants(1);

        Folder.Add(new CsharpFileLeaf()
        {
            SourceCode = SourceFile,
            Mutants = TestScenario.GetMutants()
        });
        TestScenario.CreateTests(1, 2, 3);

        // mutant 1 is covered by both tests
        TestScenario.DeclareCoverageForMutant(1);
        // mutant 2 is covered only by test 1
        TestScenario.DeclareTestsFailingAtInit(1);
        // test 1 succeeds, test 2 fails
        TestScenario.DeclareTestsFailingWhenTestingMutant(1, 1, 2);

        // setup coverage
        var loggerMock6 = new Mock<ILogger<MutationTestExecutor>>();
        var executor = new MutationTestExecutor(loggerMock6.Object);
        executor.TestRunner = TestScenario.GetTestRunnerMock().Object;

        var options = new StrykerOptions
        {
            ProjectPath = basePath,
            Concurrency = 1,
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };
        Input.InitialTestRun = new InitialTestRun(TestScenario.GetInitialRunResult(), new TimeoutValueCalculator(500));

        var coverageAnalyzerMock6 = new Mock<ICoverageAnalyser>();
        var mutationProcessMock = Mock.Of<IMutationProcess>();

        var target = new MutationTestProcess(executor, coverageAnalyzerMock6.Object, mutationProcessMock, TestLoggerFactory.CreateLogger<MutationTestProcess>());

        // test mutants
        target.Initialize(Input, options, null);
        target.GetCoverage();

        await target.TestAsync(Input.SourceProjectInfo.ProjectContents.Mutants);
        // first mutant should be killed by test 2
        TestScenario.GetMutantStatus(1).ShouldBe(MutantStatus.Killed);
    }

    [TestMethod]
    [DataRow(MutantStatus.Ignored)]
    [DataRow(MutantStatus.CompileError)]
    public void ShouldThrowExceptionWhenOtherStatusThanNotRunIsPassed(MutantStatus status)
    {
        var mutants = new List<Mutant> { new Mutant { Id = 1, ResultStatus = status } };

        var mutationTestExecutor = Mock.Of<IMutationTestExecutor>();
        var coverageAnalyzer = Mock.Of<ICoverageAnalyser>();
        var mutationProcessMock = Mock.Of<IMutationProcess>();
        var target = new MutationTestProcess(mutationTestExecutor, coverageAnalyzer, mutationProcessMock, TestLoggerFactory.CreateLogger<MutationTestProcess>());
        target.Initialize(Input, new StrykerOptions(), null);
        Should.Throw<GeneralStrykerException>(() => target.TestAsync(mutants));
    }

    [TestMethod]
    public void ShouldNotTest_WhenThereAreNoMutations()
    {
        var reporter = Mock.Of<IReporter>();
        var mutationTestExecutor = Mock.Of<IMutationTestExecutor>();
        var coverageAnalyzer = Mock.Of<ICoverageAnalyser>();
        var mutationProcessMock = Mock.Of<IMutationProcess>();
        var target = new MutationTestProcess(mutationTestExecutor, coverageAnalyzer, mutationProcessMock, TestLoggerFactory.CreateLogger<MutationTestProcess>());
        target.Initialize(Input, new StrykerOptions(), reporter);
        var result = target.TestAsync(Enumerable.Empty<Mutant>());

        Mock.Get(reporter).VerifyNoOtherCalls();
        Mock.Get(mutationTestExecutor).VerifyNoOtherCalls();
        result.Result.MutationScore.ShouldBe(double.NaN);
    }
}
