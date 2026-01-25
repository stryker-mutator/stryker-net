using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Reporting;
using Stryker.Configuration.Options;
using Stryker.Core.CoverageAnalysis;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
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
    private CsharpFolderComposite Folder { get; } = new CsharpFolderComposite();
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
