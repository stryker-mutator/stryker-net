using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Reporting;
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
    private MockFileSystem fileSystemMock { get; } = new MockFileSystem();
    private CsharpFolderComposite _folder { get; } = new CsharpFolderComposite();
    private FullRunScenario _testScenario { get; } = new FullRunScenario();

    private MutationTestInput _input { get; }

    public MutationTestProcessTests()
    {
        CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        FilesystemRoot = Path.GetPathRoot(CurrentDirectory);
        SourceFile = File.ReadAllText(CurrentDirectory + "/TestResources/ExampleSourceFile.cs");
        _input = new MutationTestInput()
        {
            SourceProjectInfo = new SourceProjectInfo()
            {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                        { "TargetFileName", "ProjectUnderTest.dll" },
                        { "Language", "C#" }
                    }).Object,
                ProjectContents = _folder
            },
            TestProjectsInfo = new TestProjectsInfo(fileSystemMock)
            {
                TestProjects = new List<TestProject> {
                    new TestProject(fileSystemMock, TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", Path.Combine(FilesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0") },
                        { "TargetFileName", "TestName.dll" },
                        { "Language", "C#" }
                    }).Object)
                }
            },
        };
    }

    [TestMethod]
    public void ShouldCallMutantionProcess_MutateAndFilterMutants()
    {
        // Arrange
        var options = new StrykerOptions()
        {
            ExcludedMutations = new Mutator[] { }
        };
        var mutationTestExecutorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
        var mutantionProcessMock = new Mock<IMutationProcess>(MockBehavior.Strict);
        mutantionProcessMock.Setup(x => x.Mutate(It.IsAny<MutationTestInput>()));
        mutantionProcessMock.Setup(x => x.FilterMutants(It.IsAny<MutationTestInput>()));

        var executorMock = new Mock<IMutationTestExecutor>();
        var coverageAnalyzerMock = new Mock<ICoverageAnalyser>();
        var target = new MutationTestProcess(executorMock.Object, coverageAnalyzerMock.Object, mutantionProcessMock.Object);

        // Act
        target.Initialize(_input, options, null);
        target.Mutate();

        target.FilterMutants();

        // Assert
        mutantionProcessMock.Verify(x => x.Mutate(It.IsAny<MutationTestInput>()), Times.Once);
        mutantionProcessMock.Verify(x => x.FilterMutants(It.IsAny<MutationTestInput>()), Times.Once);
    }

    [TestMethod]
    public void ShouldCallExecutorForEveryCoveredMutant()
    {
        _testScenario.CreateMutants(1, 2);
        // we need at least one test
        _testScenario.CreateTest(1);
        // and we need to declare that the mutant is covered
        _testScenario.DeclareCoverageForMutant(1);
        var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");

        _folder.Add(new CsharpFileLeaf()
        {
            SourceCode = SourceFile,
            Mutants = _testScenario.GetMutants()
        });

        var loggerMock = new Mock<ILogger<MutationTestExecutor>>();
        var mutationExecutor = new MutationTestExecutor(loggerMock.Object);
        var coverageAnalyzerMock = new Mock<ICoverageAnalyser>();

        var options = new StrykerOptions()
        {
            ProjectPath = basePath,
            Concurrency = 1,
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };
        _input.InitialTestRun = new InitialTestRun(_testScenario.GetInitialRunResult(), new TimeoutValueCalculator(500));

        var target = new MutationTestProcess(mutationExecutor, coverageAnalyzerMock.Object);

        target.Initialize(_input, options, null);
        target.GetCoverage();
        target.Test(_testScenario.GetCoveredMutants());

        _testScenario.GetMutantStatus(1).ShouldBe(MutantStatus.Survived);
        _testScenario.GetMutantStatus(2).ShouldBe(MutantStatus.NoCoverage);
    }

    [TestMethod]
    public void ShouldCallExecutorForEveryMutantWhenNoOptimization()
    {
        var scenario = new FullRunScenario();
        _testScenario.CreateMutants(1, 2);
        // we need at least one test
        _testScenario.CreateTest(1);
        // and we need to declare that the mutant is covered
        _testScenario.DeclareCoverageForMutant(1);
        _testScenario.SetMode(OptimizationModes.None);
        var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");

        _folder.Add(new CsharpFileLeaf()
        {
            SourceCode = SourceFile,
            Mutants = _testScenario.GetMutants()
        });

        var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
        reporterMock.Setup(x => x.OnMutantTested(It.IsAny<IMutant>()));

        var loggerMock2 = new Mock<ILogger<MutationTestExecutor>>();
        var mutationExecutor = new MutationTestExecutor(loggerMock2.Object);

        var options = new StrykerOptions()
        {
            OutputPath = basePath,
            Concurrency = 1,
            OptimizationMode = OptimizationModes.None
        };
        _input.InitialTestRun = new InitialTestRun(_testScenario.GetInitialRunResult(), new TimeoutValueCalculator(500));

        var coverageAnalyzerMock2 = new Mock<ICoverageAnalyser>();
        var target = new MutationTestProcess(mutationExecutor, coverageAnalyzerMock2.Object);

        target.Initialize(_input, options, null);
        target.GetCoverage();
        target.Test(_testScenario.GetMutants());

        _testScenario.GetMutantStatus(1).ShouldBe(MutantStatus.Survived);
        _testScenario.GetMutantStatus(2).ShouldBe(MutantStatus.Survived);
    }

    [TestMethod]
    public void ShouldHandleCoverage()
    {
        var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");
        _testScenario.CreateMutants(1, 2, 3);

        _folder.Add(new CsharpFileLeaf()
        {
            SourceCode = SourceFile,
            Mutants = _testScenario.GetMutants()
        });
        _testScenario.CreateTests(1, 2);

        // mutant 1 is covered by both tests
        _testScenario.DeclareCoverageForMutant(1);
        // mutant 2 is covered only by test 1
        _testScenario.DeclareCoverageForMutant(2, 1);
        // mutant 3 as no coverage
        // test 1 succeeds, test 2 fails
        _testScenario.DeclareTestsFailingWhenTestingMutant(1, 2);
        var runnerMock = _testScenario.GetTestRunnerMock();

        // setup coverage
        var loggerMock3 = new Mock<ILogger<MutationTestExecutor>>();
        var executor = new MutationTestExecutor(loggerMock3.Object);

        var options = new StrykerOptions
        {
            ProjectPath = basePath,
            Concurrency = 1,
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };
        _input.InitialTestRun = new InitialTestRun(_testScenario.GetInitialRunResult(), new TimeoutValueCalculator(500));

        var coverageAnalyzerMock3 = new Mock<ICoverageAnalyser>();
        var target = new MutationTestProcess(executor, coverageAnalyzerMock3.Object);
        // test mutants
        target.Initialize(_input, options, null);
        target.GetCoverage();

        target.Test(_input.SourceProjectInfo.ProjectContents.Mutants.Where(m => m.ResultStatus == MutantStatus.Pending));
        // first mutant should be killed by test 2
        _testScenario.GetMutantStatus(1).ShouldBe(MutantStatus.Killed);
        // other mutant survives
        _testScenario.GetMutantStatus(2).ShouldBe(MutantStatus.Survived);
        // third mutant appears as no coverage
        _testScenario.GetMutantStatus(3).ShouldBe(MutantStatus.NoCoverage);
    }

    [TestMethod]
    public void ShouldNotKillMutantIfOnlyKilledByFailingTest()
    {
        var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");
        _testScenario.CreateMutants(1);

        _folder.Add(new CsharpFileLeaf()
        {
            SourceCode = SourceFile,
            Mutants = _testScenario.GetMutants()
        });
        _testScenario.CreateTests(1, 2, 3);

        // mutant 1 is covered by all tests
        _testScenario.DeclareCoverageForMutant(1);
        // mutant 2 is covered only by test 1
        _testScenario.DeclareTestsFailingAtInit(1);
        // test 1 succeeds, test 2 fails
        _testScenario.DeclareTestsFailingWhenTestingMutant(1, 1);
        var runnerMock = _testScenario.GetTestRunnerMock();

        // setup coverage
        var loggerMock4 = new Mock<ILogger<MutationTestExecutor>>();
        var executor = new MutationTestExecutor(loggerMock4.Object);

        var options = new StrykerOptions
        {
            ProjectPath = basePath,
            Concurrency = 1,
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };

        _input.InitialTestRun = new InitialTestRun(_testScenario.GetInitialRunResult(), new TimeoutValueCalculator(500));

        var coverageAnalyzerMock4 = new Mock<ICoverageAnalyser>();
        var target = new MutationTestProcess(executor, coverageAnalyzerMock4.Object);

        // test mutants
        target.Initialize(_input, options, null);
        target.GetCoverage();

        target.Test(_input.SourceProjectInfo.ProjectContents.Mutants);
        // first mutant should be marked as survived
        _testScenario.GetMutantStatus(1).ShouldBe(MutantStatus.Survived);
    }

    [TestMethod]
    public void ShouldNotKillMutantIfOnlyCoveredByFailingTest()
    {
        var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");
        _testScenario.CreateMutants(1, 2);

        _folder.Add(new CsharpFileLeaf()
        {
            SourceCode = SourceFile,
            Mutants = _testScenario.GetMutants()
        });
        _testScenario.CreateTests(1, 2, 3);

        // mutant 1 is covered by both tests
        _testScenario.DeclareCoverageForMutant(1, 1, 2, 3);
        // mutant 2 is covered only by test 1
        _testScenario.DeclareTestsFailingAtInit(1, 2, 3);
        // test 1 succeeds, test 2 fails
        _testScenario.DeclareTestsFailingWhenTestingMutant(1, 1, 2, 3);
        var runnerMock = _testScenario.GetTestRunnerMock();

        // setup coverage
        var loggerMock5 = new Mock<ILogger<MutationTestExecutor>>();
        var executor = new MutationTestExecutor(loggerMock5.Object);

        var options = new StrykerOptions
        {
            ProjectPath = basePath,
            Concurrency = 1,
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };
        _input.InitialTestRun = new InitialTestRun(_testScenario.GetInitialRunResult(), new TimeoutValueCalculator(500));

        var coverageAnalyzerMock5 = new Mock<ICoverageAnalyser>();
        var target = new MutationTestProcess(executor, coverageAnalyzerMock5.Object);
        // test mutants
        target.Initialize(_input, options, null);
        target.GetCoverage();

        // first mutant should be marked as survived without any test
        _testScenario.GetMutantStatus(1).ShouldBe(MutantStatus.Survived);
    }

    [TestMethod]
    public void ShouldKillMutantKilledByFailingTestAndNormalTest()
    {
        var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");
        _testScenario.CreateMutants(1);

        _folder.Add(new CsharpFileLeaf()
        {
            SourceCode = SourceFile,
            Mutants = _testScenario.GetMutants()
        });
        _testScenario.CreateTests(1, 2, 3);

        // mutant 1 is covered by both tests
        _testScenario.DeclareCoverageForMutant(1);
        // mutant 2 is covered only by test 1
        _testScenario.DeclareTestsFailingAtInit(1);
        // test 1 succeeds, test 2 fails
        _testScenario.DeclareTestsFailingWhenTestingMutant(1, 1, 2);
        var runnerMock = _testScenario.GetTestRunnerMock();

        // setup coverage
        var loggerMock6 = new Mock<ILogger<MutationTestExecutor>>();
        var executor = new MutationTestExecutor(loggerMock6.Object);

        var options = new StrykerOptions
        {
            ProjectPath = basePath,
            Concurrency = 1,
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };
        _input.InitialTestRun = new InitialTestRun(_testScenario.GetInitialRunResult(), new TimeoutValueCalculator(500));

        var coverageAnalyzerMock6 = new Mock<ICoverageAnalyser>();
        var target = new MutationTestProcess(executor, coverageAnalyzerMock6.Object);

        // test mutants
        target.Initialize(_input, options, null);
        target.GetCoverage();

        target.Test(_input.SourceProjectInfo.ProjectContents.Mutants);
        // first mutant should be killed by test 2
        _testScenario.GetMutantStatus(1).ShouldBe(MutantStatus.Killed);
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
        var target = new MutationTestProcess(mutationTestExecutor, coverageAnalyzer, mutationProcessMock);
        target.Initialize(_input, new StrykerOptions(), null);
        Should.Throw<GeneralStrykerException>(() => target.Test(mutants));
    }

    [TestMethod]
    public void ShouldNotTest_WhenThereAreNoMutations()
    {
        var reporter = Mock.Of<IReporter>();
        var mutationTestExecutor = Mock.Of<IMutationTestExecutor>();
        var coverageAnalyzer = Mock.Of<ICoverageAnalyser>();
        var mutationProcessMock = Mock.Of<IMutationProcess>();
        var target = new MutationTestProcess(mutationTestExecutor, coverageAnalyzer, mutationProcessMock);
        target.Initialize(_input, new StrykerOptions(), reporter);
        var result = target.Test(Enumerable.Empty<Mutant>());

        Mock.Get(reporter).VerifyNoOtherCalls();
        Mock.Get(mutationTestExecutor).VerifyNoOtherCalls();
        result.MutationScore.ShouldBe(double.NaN);
    }
}
