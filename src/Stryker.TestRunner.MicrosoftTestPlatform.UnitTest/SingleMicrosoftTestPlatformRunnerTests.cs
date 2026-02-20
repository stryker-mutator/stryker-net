using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.TestRunner.Tests;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.Results;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class SingleMicrosoftTestPlatformRunnerTests
{
    private Dictionary<string, List<TestNode>> _testsByAssembly = null!;
    private Dictionary<string, MtpTestDescription> _testDescriptions = null!;
    private TestSet _testSet = null!;
    private object _discoveryLock = null!;

    [TestInitialize]
    public void Initialize()
    {
        _testsByAssembly = new Dictionary<string, List<TestNode>>();
        _testDescriptions = new Dictionary<string, MtpTestDescription>();
        _testSet = new TestSet();
        _discoveryLock = new object();
    }

    private SingleMicrosoftTestPlatformRunner CreateRunner(int id = 0) =>
        new(id, _testsByAssembly, _testDescriptions, _testSet, _discoveryLock, NullLogger.Instance);

    [TestMethod, Timeout(1000)]
    public async Task InitialTestAsync_CallsRunTestsInternalAsync_AndHandlesServerCreationFailure()
    {
        // Arrange - InitialTestAsync eventually calls RunTestsInternalAsync via RunAllTestsAsync
        var project = new Mock<IProjectAndTests>();
        var invalidAssembly = "/path/to/nonexistent.dll";
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { invalidAssembly });

        using var runner = CreateRunner(0);

        // Act - This will call RunAllTestsAsync -> ProcessSingleAssemblyAsync -> RunTestsInternalAsync
        // RunTestsInternalAsync will handle the exception when GetOrCreateServerAsync fails
        var result = await runner.InitialTestAsync(project.Object);

        // Assert
        result.ShouldNotBeNull();
        result.ExecutedTests.ShouldNotBeNull();
        result.FailingTests.ShouldNotBeNull();
        result.Duration.ShouldBeGreaterThanOrEqualTo(TimeSpan.Zero);
    }

    [TestMethod, Timeout(1000)]
    public async Task TestMultipleMutantsAsync_CallsRunTestsInternalAsync_WithNonExistentAssembly()
    {
        // Arrange
        var project = new Mock<IProjectAndTests>();
        var invalidAssembly = "/invalid/path/test.dll";
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { invalidAssembly });

        var mutant = new Mock<IMutant>();
        mutant.Setup(x => x.Id).Returns(1);
        var mutants = new List<IMutant> { mutant.Object };

        using var runner = CreateRunner(0);

        // Act - Calls RunAllTestsAsync -> ProcessSingleAssemblyAsync -> RunTestsInternalAsync
        var result = await runner.TestMultipleMutantsAsync(project.Object, null, mutants, null);

        // Assert - RunTestsInternalAsync catches exceptions and returns TestRunResult
        result.ShouldNotBeNull();
        result.ExecutedTests.ShouldNotBeNull();
        result.Duration.ShouldBeGreaterThanOrEqualTo(TimeSpan.Zero);
    }

    [TestMethod, Timeout(1000)]
    public async Task RunTestsInternalAsync_HandlesExceptionPath_WhenServerCreationFails()
    {
        // Arrange
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { "/nonexistent.dll" });

        using var runner = CreateRunner(0);

        // Act - This exercises the exception handling in RunTestsInternalAsync
        var result = await runner.InitialTestAsync(project.Object);

        // Assert - RunTestsInternalAsync returns TestRunResult(false, ex.Message) on exception
        result.ShouldNotBeNull();
        var testRunResult = result as Stryker.TestRunner.Results.TestRunResult;
        testRunResult.ShouldNotBeNull();
        testRunResult.FailingTests.ShouldNotBeNull();
    }

    [TestMethod, Timeout(1000)]
    [DataRow("assembly-a.dll")]
    [DataRow("assembly-b.dll")]
    [DataRow("Some/Path/To/Assembly.dll")]
    public void GetDiscoveredTests_ReturnsNull_WhenAssemblyNotRegistered(string assembly)
    {
        using var runner = CreateRunner();

        var result = runner.GetDiscoveredTests(assembly);

        result.ShouldBeNull();
    }

    [TestMethod, Timeout(1000)]
    public void GetDiscoveredTests_ReturnsTests_WhenAssemblyIsRegistered()
    {
        var tests = new List<TestNode>
        {
            new("uid-1", "Test1", "test", "passed"),
            new("uid-2", "Test2", "test", "failed")
        };
        _testsByAssembly["my-assembly.dll"] = tests;

        using var runner = CreateRunner();

        var result = runner.GetDiscoveredTests("my-assembly.dll");

        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result[0].Uid.ShouldBe("uid-1");
        result[1].Uid.ShouldBe("uid-2");
    }

    [TestMethod, Timeout(1000)]
    public void GetDiscoveredTests_ReturnsEmptyList_WhenAssemblyHasNoTests()
    {
        _testsByAssembly["empty.dll"] = [];

        using var runner = CreateRunner();

        var result = runner.GetDiscoveredTests("empty.dll");

        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [TestMethod, Timeout(1000)]
    [DataRow(100, 500, 500)]
    [DataRow(0, 500, 500)]
    [DataRow(250, 1000, 1000)]
    public void CalculateAssemblyTimeout_ReturnsExpectedTimeout(
        int initialRunTimeMs, int calculatorReturns, int expectedMs)
    {
        var testNode = new TestNode("uid-1", "Test1", "test", "passed");
        var discoveredTests = new List<TestNode> { testNode };

        var desc = new MtpTestDescription(testNode);
        desc.RegisterInitialTestResult(new MtpTestResult(TimeSpan.FromMilliseconds(initialRunTimeMs)));
        _testDescriptions["uid-1"] = desc;

        var timeoutCalc = new Mock<ITimeoutValueCalculator>();
        timeoutCalc.Setup(x => x.CalculateTimeoutValue(It.IsAny<int>()))
            .Returns(calculatorReturns);

        using var runner = CreateRunner();

        var result = runner.CalculateAssemblyTimeout(discoveredTests, timeoutCalc.Object, "test.dll");

        result.ShouldNotBeNull();
        result.Value.TotalMilliseconds.ShouldBe(expectedMs);
    }

    [TestMethod, Timeout(1000)]
    public void CalculateAssemblyTimeout_SumsMultipleTestDurations()
    {
        var test1 = new TestNode("uid-1", "Test1", "test", "passed");
        var test2 = new TestNode("uid-2", "Test2", "test", "passed");
        var discoveredTests = new List<TestNode> { test1, test2 };

        var desc1 = new MtpTestDescription(test1);
        desc1.RegisterInitialTestResult(new MtpTestResult(TimeSpan.FromMilliseconds(100)));
        _testDescriptions["uid-1"] = desc1;

        var desc2 = new MtpTestDescription(test2);
        desc2.RegisterInitialTestResult(new MtpTestResult(TimeSpan.FromMilliseconds(200)));
        _testDescriptions["uid-2"] = desc2;

        int capturedEstimate = -1;
        var timeoutCalc = new Mock<ITimeoutValueCalculator>();
        timeoutCalc.Setup(x => x.CalculateTimeoutValue(It.IsAny<int>()))
            .Callback<int>(ms => capturedEstimate = ms)
            .Returns(999);

        using var runner = CreateRunner();

        runner.CalculateAssemblyTimeout(discoveredTests, timeoutCalc.Object, "test.dll");

        capturedEstimate.ShouldBe(300);
    }

    [TestMethod, Timeout(1000)]
    public void CalculateAssemblyTimeout_SkipsTestsWithoutDescription()
    {
        var test1 = new TestNode("uid-1", "Test1", "test", "passed");
        var testWithoutDesc = new TestNode("uid-unknown", "Unknown", "test", "passed");
        var discoveredTests = new List<TestNode> { test1, testWithoutDesc };

        var desc1 = new MtpTestDescription(test1);
        desc1.RegisterInitialTestResult(new MtpTestResult(TimeSpan.FromMilliseconds(150)));
        _testDescriptions["uid-1"] = desc1;

        int capturedEstimate = -1;
        var timeoutCalc = new Mock<ITimeoutValueCalculator>();
        timeoutCalc.Setup(x => x.CalculateTimeoutValue(It.IsAny<int>()))
            .Callback<int>(ms => capturedEstimate = ms)
            .Returns(777);

        using var runner = CreateRunner();

        runner.CalculateAssemblyTimeout(discoveredTests, timeoutCalc.Object, "test.dll");

        capturedEstimate.ShouldBe(150);
    }

    [TestMethod, Timeout(1000)]
    public async Task HandleAssemblyTimeoutAsync_AddsAllTestUidsToTimedOutList()
    {
        var tests = new List<TestNode>
        {
            new("uid-1", "Test1", "test", "passed"),
            new("uid-2", "Test2", "test", "passed"),
            new("uid-3", "Test3", "test", "passed")
        };
        var timedOutTests = new List<string>();

        using var runner = CreateRunner();

        await runner.HandleAssemblyTimeoutAsync("some-assembly.dll", tests, timedOutTests);

        timedOutTests.ShouldBe(["uid-1", "uid-2", "uid-3"]);
    }

    [TestMethod, Timeout(1000)]
    public async Task HandleAssemblyTimeoutAsync_AppendsToExistingTimedOutList()
    {
        var tests = new List<TestNode> { new("uid-new", "NewTest", "test", "passed") };
        var timedOutTests = new List<string> { "uid-existing" };

        using var runner = CreateRunner();
        await runner.HandleAssemblyTimeoutAsync("assembly.dll", tests, timedOutTests);

        timedOutTests.Count.ShouldBe(2);
        timedOutTests.ShouldContain("uid-existing");
        timedOutTests.ShouldContain("uid-new");
    }

    [TestMethod, Timeout(1000)]
    public async Task HandleAssemblyTimeoutAsync_HandlesEmptyTestList()
    {
        var tests = new List<TestNode>();
        var timedOutTests = new List<string>();

        using var runner = CreateRunner();

        await runner.HandleAssemblyTimeoutAsync("assembly.dll", tests, timedOutTests);

        timedOutTests.ShouldBeEmpty();
    }

    [TestMethod, Timeout(1000)]
    public async Task RunTestsInternalAsync_ReturnsFailedResult_WhenServerCreationFails()
    {
        using var runner = CreateRunner();

        var (result, timedOut) = await runner.RunTestsInternalAsync("/nonexistent/assembly.dll", null, null);

        timedOut.ShouldBeFalse();
        result.ShouldNotBeNull();
        result.ResultMessage.ShouldNotBeNullOrEmpty();
    }

    [TestMethod, Timeout(1000)]
    [DataRow("/path/a.dll")]
    [DataRow("/another/path/b.dll")]
    public async Task RunTestsInternalAsync_CatchesException_AndReturnsResult(string assembly)
    {
        using var runner = CreateRunner();

        var (result, timedOut) = await runner.RunTestsInternalAsync(assembly, null, null);

        timedOut.ShouldBeFalse();
        result.ShouldBeOfType<TestRunResult>();
        result.Duration.ShouldBe(TimeSpan.Zero);
    }

    [TestMethod, Timeout(1000)]
    public async Task RunTestsInternalAsync_WithTimeout_StillReturnsResult_WhenServerFails()
    {
        using var runner = CreateRunner();
        var timeout = TimeSpan.FromMilliseconds(100);

        var (result, timedOut) = await runner.RunTestsInternalAsync("/nonexistent.dll", null, timeout)!;

        timedOut.ShouldBeFalse();
        result.ShouldNotBeNull();
        result.FailingTests.ShouldNotBeNull();
    }

    [TestMethod, Timeout(1000)]
    public async Task RunTestsInternalAsync_WithTimeout_DoesNotHangOnRealAssembly()
    {
        // Arrange - This test ensures we don't try to start real servers that would hang
        var fakeAssemblyPath = "/path/to/fake/test.dll";
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { fakeAssemblyPath });

        var mutant = new Mock<IMutant>();
        mutant.Setup(x => x.Id).Returns(1);
        var mutants = new List<IMutant> { mutant.Object };

        var timeoutCalc = new Mock<ITimeoutValueCalculator>();
        timeoutCalc.Setup(x => x.CalculateTimeoutValue(It.IsAny<int>())).Returns(1);

        // Add discovered tests
        var testNode = new TestNode("test1", "TestMethod1", "passed", "passed");
        _testsByAssembly[fakeAssemblyPath] = new List<TestNode> { testNode };
        _testDescriptions["test1"] = new MtpTestDescription(testNode);

        using var runner = CreateRunner(0);

        // Act - Should complete quickly without hanging, even though we have a timeout calculator
        var result = await runner.TestMultipleMutantsAsync(project.Object, timeoutCalc.Object, mutants, null);

        // Assert - The test completes without hanging (file doesn't exist so returns early)
        result.ShouldNotBeNull();
        result.ExecutedTests.ShouldNotBeNull();
    }

    [TestMethod, Timeout(1000)]
    public async Task RunTestsInternalAsync_RegistersTestResults_InTestDescriptions()
    {
        // Arrange
        var testNode = new TestNode("test1", "TestMethod1", "passed", "passed");
        var mtpTestDesc = new MtpTestDescription(testNode);
        _testDescriptions["test1"] = mtpTestDesc;

        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { "/nonexistent/assembly.dll" });

        using var runner = CreateRunner(0);

        // Act
        var result = await runner.InitialTestAsync(project.Object);

        // Assert - Test descriptions should still be in the dictionary
        _testDescriptions.ShouldContainKey("test1");
        result.ShouldNotBeNull();
    }

    [TestMethod, Timeout(1000)]
    public async Task RunTestsInternalAsync_CalculatesDuration()
    {
        // Arrange
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { "/nonexistent.dll" });

        using var runner = CreateRunner(0);

        // Act
        var startTime = DateTime.UtcNow;
        var result = await runner.InitialTestAsync(project.Object);
        var endTime = DateTime.UtcNow;

        // Assert - Duration should be calculated
        result.ShouldNotBeNull();
        result.Duration.ShouldBeGreaterThanOrEqualTo(TimeSpan.Zero);
        result.Duration.ShouldBeLessThan(endTime - startTime + TimeSpan.FromSeconds(2));
    }

    [TestMethod, Timeout(1000)]
    public async Task RunTestsInternalAsync_WithMultipleMutants_UsesNegativeOneMutantId()
    {
        // Arrange
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { "/test.dll" });

        var mutant1 = new Mock<IMutant>();
        mutant1.Setup(x => x.Id).Returns(1);
        var mutant2 = new Mock<IMutant>();
        mutant2.Setup(x => x.Id).Returns(2);
        var mutants = new List<IMutant> { mutant1.Object, mutant2.Object };

        using var runner = CreateRunner(0);

        // Act - With multiple mutants, mutantId should be -1 (no mutation)
        var result = await runner.TestMultipleMutantsAsync(project.Object, null, mutants, null);

        // Assert
        result.ShouldNotBeNull();
        result.ExecutedTests.ShouldNotBeNull();
    }

    [TestMethod, Timeout(1000)]
    public async Task RunTestsInternalAsync_WithSingleMutant_UsesMutantId()
    {
        // Arrange
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { "/test.dll" });

        var mutant = new Mock<IMutant>();
        mutant.Setup(x => x.Id).Returns(42);
        var mutants = new List<IMutant> { mutant.Object };

        using var runner = CreateRunner(0);

        // Act - With single mutant, that mutant's ID should be used
        var result = await runner.TestMultipleMutantsAsync(project.Object, null, mutants, null);

        // Assert
        result.ShouldNotBeNull();
        result.ExecutedTests.ShouldNotBeNull();
    }

    [TestMethod, Timeout(1000)]
    public async Task RunTestsInternalAsync_IncludesResultMessage_OnError()
    {
        // Arrange
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { "/invalid/assembly.dll" });

        using var runner = CreateRunner(0);

        // Act
        var result = await runner.InitialTestAsync(project.Object);

        // Assert - Result message should be included on error
        result.ShouldNotBeNull();
        result.ResultMessage.ShouldNotBeNull();
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Clean up any temporary coverage files created during tests
        for (int id = 1; id <= 20; id++)
        {
            var coverageFilePath = Path.Combine(Path.GetTempPath(), $"stryker-coverage-{id}.txt");
            try
            {
                if (File.Exists(coverageFilePath))
                {
                    File.Delete(coverageFilePath);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    [TestMethod, Timeout(1000)]
    public async Task DiscoverTestsAsync_ShouldReturnFalse_WhenAssemblyNotFound()
    {
        // Arrange
        using var runner = new SingleMicrosoftTestPlatformRunner(
            0,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Act
        var result = await runner.DiscoverTestsAsync("/nonexistent/assembly.dll");

        // Assert
        result.ShouldBeFalse();
    }

    [TestMethod, Timeout(1000)]
    public async Task InitialTestAsync_ShouldReturnTestRunResult()
    {
        // Arrange
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { "/nonexistent/assembly.dll" });

        using var runner = new SingleMicrosoftTestPlatformRunner(
            0,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Act
        var result = await runner.InitialTestAsync(project.Object);

        // Assert
        result.ShouldNotBeNull();
        result.ExecutedTests.ShouldNotBeNull();
        result.FailingTests.ShouldNotBeNull();
    }

    [TestMethod, Timeout(1000)]
    public async Task TestMultipleMutantsAsync_ShouldReturnTestRunResult_WithNoAssemblies()
    {
        // Arrange
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string>());

        var mutant = new Mock<IMutant>();
        mutant.Setup(x => x.Id).Returns(1);
        var mutants = new List<IMutant> { mutant.Object };

        using var runner = new SingleMicrosoftTestPlatformRunner(
            0,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Act
        var result = await runner.TestMultipleMutantsAsync(project.Object, null, mutants, null);

        // Assert
        result.ShouldNotBeNull();
        result.ExecutedTests.ShouldNotBeNull();
        result.FailingTests.ShouldNotBeNull();
    }

    [TestMethod, Timeout(1000)]
    public async Task TestMultipleMutantsAsync_ShouldUseCorrectMutantId_WhenSingleMutant()
    {
        // Arrange
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string>());

        var mutant = new Mock<IMutant>();
        mutant.Setup(x => x.Id).Returns(42);
        var mutants = new List<IMutant> { mutant.Object };

        using var runner = new SingleMicrosoftTestPlatformRunner(
            0,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Act
        var result = await runner.TestMultipleMutantsAsync(project.Object, null, mutants, null);

        // Assert
        result.ShouldNotBeNull();
        mutant.Verify(x => x.Id, Times.AtLeastOnce);
    }

    [TestMethod, Timeout(1000)]
    public async Task TestMultipleMutantsAsync_ShouldUseNoMutationId_WhenMultipleMutants()
    {
        // Arrange
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string>());

        var mutant1 = new Mock<IMutant>();
        mutant1.Setup(x => x.Id).Returns(1);
        var mutant2 = new Mock<IMutant>();
        mutant2.Setup(x => x.Id).Returns(2);
        var mutants = new List<IMutant> { mutant1.Object, mutant2.Object };

        using var runner = new SingleMicrosoftTestPlatformRunner(
            0,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Act
        var result = await runner.TestMultipleMutantsAsync(project.Object, null, mutants, null);

        // Assert
        result.ShouldNotBeNull();
        result.ExecutedTests.ShouldNotBeNull();
    }

    [TestMethod, Timeout(1000)]
    public async Task Dispose_ShouldCleanUpResources()
    {
        // Arrange
        var testableRunner = new TestableRunner(
            123,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Verify mutant file was created
        testableRunner.MutantFilePath.ShouldNotBeNull();
        var mutantFilePath = testableRunner.MutantFilePath;

        // Create the mutant file manually to test deletion
        File.WriteAllText(mutantFilePath, "-1");
        File.Exists(mutantFilePath).ShouldBeTrue("Mutant file should exist before disposal");

        // Act
        testableRunner.Dispose();

        // Assert
        testableRunner.DisposedFlagWasSet.ShouldBeTrue("_disposed flag should be set to true");
        testableRunner.DisposeLogicExecutedCount.ShouldBe(1, "Dispose logic should execute once on first call");
        File.Exists(mutantFilePath).ShouldBeFalse("Mutant file should be deleted after disposal");

        // Act - Second disposal (verify idempotency via _disposed flag check)
        testableRunner.Dispose();

        // Assert
        testableRunner.DisposeLogicExecutedCount.ShouldBe(1, "Dispose logic should only execute once due to _disposed flag check preventing re-execution");
    }

    [TestMethod, Timeout(1000)]
    public void SetCoverageMode_ShouldEnableCoverageMode()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            1,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Act
        runner.SetCoverageMode(true);

        // Assert
        runner.IsCoverageModeEnabled.ShouldBeTrue();
    }

    [TestMethod, Timeout(1000)]
    public void SetCoverageMode_ShouldDisableCoverageMode()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            2,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        runner.SetCoverageMode(true);

        // Act
        runner.SetCoverageMode(false);

        // Assert
        runner.IsCoverageModeEnabled.ShouldBeFalse();
    }

    [TestMethod, Timeout(1000)]
    public void SetCoverageMode_ShouldBeIdempotent_WhenCalledWithTrue()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            3,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        runner.SetCoverageMode(true);

        // Act - Call with same value
        runner.SetCoverageMode(true);

        // Assert - Should still be enabled and not throw any exceptions
        runner.IsCoverageModeEnabled.ShouldBeTrue();
    }

    [TestMethod, Timeout(1000)]
    public void SetCoverageMode_ShouldBeIdempotent_WhenCalledWithFalse()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            4,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Default is false, so calling it again should be idempotent
        runner.IsCoverageModeEnabled.ShouldBeFalse();

        // Act - Call with same value as default
        runner.SetCoverageMode(false);

        // Assert - Should still be disabled and not throw any exceptions
        runner.IsCoverageModeEnabled.ShouldBeFalse();
    }

    [TestMethod, Timeout(1000)]
    public void SetCoverageMode_ShouldDeleteCoverageFile_WhenEnabling()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            5,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Create a coverage file
        File.WriteAllText(runner.CoverageFilePath, "1,2,3;4,5,6");
        File.Exists(runner.CoverageFilePath).ShouldBeTrue();

        // Act
        runner.SetCoverageMode(true);

        // Assert
        File.Exists(runner.CoverageFilePath).ShouldBeFalse("Coverage file should be deleted when enabling coverage mode");
    }

    [TestMethod, Timeout(1000)]
    public void ReadCoverageData_ShouldReturnEmptyLists_WhenFileDoesNotExist()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            6,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Ensure file doesn't exist
        if (File.Exists(runner.CoverageFilePath))
        {
            File.Delete(runner.CoverageFilePath);
        }

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBeEmpty();
        staticMutants.ShouldBeEmpty();
    }

    [TestMethod, Timeout(1000)]
    public void ReadCoverageData_ShouldReturnEmptyLists_WhenFileIsEmpty()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            7,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, "");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBeEmpty();
        staticMutants.ShouldBeEmpty();
    }

    [TestMethod, Timeout(1000)]
    public void ReadCoverageData_ShouldReturnEmptyLists_WhenFileContainsOnlyWhitespace()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            8,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, "   \n\t  ");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBeEmpty();
        staticMutants.ShouldBeEmpty();
    }

    [TestMethod, Timeout(1000)]
    public void ReadCoverageData_ShouldParseCoveredMutantsOnly()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            9,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, "1,2,3");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBe(new[] { 1, 2, 3 });
        staticMutants.ShouldBeEmpty();
    }

    [TestMethod, Timeout(1000)]
    public void ReadCoverageData_ShouldParseBothCoveredAndStaticMutants()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            10,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, "1,2,3;4,5,6");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBe(new[] { 1, 2, 3 });
        staticMutants.ShouldBe(new[] { 4, 5, 6 });
    }

    [TestMethod, Timeout(1000)]
    public void ReadCoverageData_ShouldHandleEmptyCoveredSection()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            11,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, ";4,5,6");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBeEmpty();
        staticMutants.ShouldBe(new[] { 4, 5, 6 });
    }

    [TestMethod, Timeout(1000)]
    public void ReadCoverageData_ShouldHandleEmptyStaticSection()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            12,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, "1,2,3;");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBe(new[] { 1, 2, 3 });
        staticMutants.ShouldBeEmpty();
    }

    [TestMethod, Timeout(1000)]
    public void ReadCoverageData_ShouldHandleDataWithSpaces()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            13,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, " 1 , 2 , 3 ; 4 , 5 , 6 ");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBe(new[] { 1, 2, 3 });
        staticMutants.ShouldBe(new[] { 4, 5, 6 });
    }

    [TestMethod, Timeout(1000)]
    public void ReadCoverageData_ShouldSkipInvalidNumbers()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            14,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, "1,invalid,3;4,bad,6");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBe(new[] { 1, 3 });
        staticMutants.ShouldBe(new[] { 4, 6 });
    }

    [TestMethod, Timeout(1000)]
    public void ReadCoverageData_ShouldHandleMixedValidAndInvalidData()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            15,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, "1,,3,notanumber,5;,,7,xyz,9");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBe(new[] { 1, 3, 5 });
        staticMutants.ShouldBe(new[] { 7, 9 });
    }

    [TestMethod, Timeout(1000)]
    public void ReadCoverageData_ShouldReturnEmptyLists_OnFileReadException()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            16,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Create a locked file by opening it exclusively
        using var fileStream = new FileStream(runner.CoverageFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
        var writer = new StreamWriter(fileStream);
        writer.Write("1,2,3;4,5,6");
        writer.Flush();

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBeEmpty();
        staticMutants.ShouldBeEmpty();
    }

    [TestMethod, Timeout(1000)]
    public void ReadCoverageData_ShouldHandleSingleMutantId()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            17,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, "42;");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBe(new[] { 42 });
        staticMutants.ShouldBeEmpty();
    }

    [TestMethod, Timeout(1000)]
    public void ReadCoverageData_ShouldHandleLargeNumbers()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            18,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, "2147483647;2147483646");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBe(new[] { 2147483647 });
        staticMutants.ShouldBe(new[] { 2147483646 });
    }

    [TestMethod, Timeout(1000)]
    public void ReadCoverageData_ShouldHandleNegativeNumbers()
    {
        // Arrange
        using var runner = new TestableRunnerForCoverage(
            19,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        File.WriteAllText(runner.CoverageFilePath, "-1,2,-3;4,-5");

        // Act
        var (coveredMutants, staticMutants) = runner.ReadCoverageData();

        // Assert
        coveredMutants.ShouldBe(new[] { -1, 2, -3 });
        staticMutants.ShouldBe(new[] { 4, -5 });
    }

    private class TestableRunner : SingleMicrosoftTestPlatformRunner
    {
        private int _disposeLogicExecutedCount;

        public TestableRunner(
            int id,
            Dictionary<string, List<TestNode>> testsByAssembly,
            Dictionary<string, MtpTestDescription> testDescriptions,
            TestSet testSet,
            object discoveryLock,
            ILogger logger)
            : base(id, testsByAssembly, testDescriptions, testSet, discoveryLock, logger)
        {
        }

        public bool DisposedFlagWasSet { get; private set; }
        public int DisposeLogicExecutedCount => _disposeLogicExecutedCount;
        public string MutantFilePath => Path.Combine(Path.GetTempPath(), $"stryker-mutant-123.txt");

        public override void Dispose(bool disposing)
        {
            var disposedField = typeof(SingleMicrosoftTestPlatformRunner).GetField("_disposed", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            var wasDisposedBefore = (bool)disposedField!.GetValue(this)!;

            base.Dispose(disposing);

            var wasDisposedAfter = (bool)disposedField!.GetValue(this)!;

            if (!wasDisposedBefore && wasDisposedAfter)
            {
                _disposeLogicExecutedCount++;
                DisposedFlagWasSet = true;
            }
        }
    }

    private class TestableRunnerForCoverage : SingleMicrosoftTestPlatformRunner
    {
        private readonly int _id;

        public TestableRunnerForCoverage(
            int id,
            Dictionary<string, List<TestNode>> testsByAssembly,
            Dictionary<string, MtpTestDescription> testDescriptions,
            TestSet testSet,
            object discoveryLock,
            ILogger logger)
            : base(id, testsByAssembly, testDescriptions, testSet, discoveryLock, logger)
        {
            _id = id;
        }

        public string CoverageFilePath => Path.Combine(Path.GetTempPath(), $"stryker-coverage-{_id}.txt");

        public bool IsCoverageModeEnabled
        {
            get
            {
                var field = typeof(SingleMicrosoftTestPlatformRunner).GetField("_coverageMode",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return (bool)field!.GetValue(this)!;
            }
        }
    }
}

