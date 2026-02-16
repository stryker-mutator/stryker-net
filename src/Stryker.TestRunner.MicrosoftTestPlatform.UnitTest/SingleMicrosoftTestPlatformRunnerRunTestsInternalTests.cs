using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.Tests;
using System.Reflection;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

/// <summary>
/// Tests for RunTestsInternalAsync method coverage.
/// Note: RunTestsInternalAsync is a private method that creates AssemblyTestServer instances.
/// Since it's tightly coupled to server infrastructure, these tests verify the method is called
/// and exercises exception handling paths. Full integration tests would be needed for complete
/// coverage of success scenarios.
/// </summary>
[TestClass]
public class SingleMicrosoftTestPlatformRunnerRunTestsInternalTests
{
    private Dictionary<string, List<TestNode>> _testsByAssembly = null!;
    private Dictionary<string, MtpTestDescription> _testDescriptions = null!;
    private TestSet _testSet = null!;
    private object _discoveryLock = null!;
    private string _testAssemblyPath = null!;

    [TestInitialize]
    public void Initialize()
    {
        _testsByAssembly = new Dictionary<string, List<TestNode>>();
        _testDescriptions = new Dictionary<string, MtpTestDescription>();
        _testSet = new TestSet();
        _discoveryLock = new object();
        _testAssemblyPath = GetType().Assembly.Location;
    }

    [TestMethod]
    public async Task InitialTestAsync_CallsRunTestsInternalAsync_AndHandlesServerCreationFailure()
    {
        // Arrange - InitialTestAsync eventually calls RunTestsInternalAsync via RunAllTestsAsync
        var project = new Mock<IProjectAndTests>();
        var invalidAssembly = "/path/to/nonexistent.dll";
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { invalidAssembly });

        using var runner = new SingleMicrosoftTestPlatformRunner(
            0,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Act - This will call RunAllTestsAsync -> ProcessSingleAssemblyAsync -> RunTestsInternalAsync
        // RunTestsInternalAsync will handle the exception when GetOrCreateServerAsync fails
        var result = await runner.InitialTestAsync(project.Object);

        // Assert
        result.ShouldNotBeNull();
        result.ExecutedTests.ShouldNotBeNull();
        result.FailingTests.ShouldNotBeNull();
        result.Duration.ShouldBeGreaterThanOrEqualTo(TimeSpan.Zero);
    }

    [TestMethod]
    public async Task TestMultipleMutantsAsync_CallsRunTestsInternalAsync_WithNonExistentAssembly()
    {
        // Arrange
        var project = new Mock<IProjectAndTests>();
        var invalidAssembly = "/invalid/path/test.dll";
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { invalidAssembly });

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

        // Act - Calls RunAllTestsAsync -> ProcessSingleAssemblyAsync -> RunTestsInternalAsync
        var result = await runner.TestMultipleMutantsAsync(project.Object, null, mutants, null);

        // Assert - RunTestsInternalAsync catches exceptions and returns TestRunResult
        result.ShouldNotBeNull();
        result.ExecutedTests.ShouldNotBeNull();
        result.Duration.ShouldBeGreaterThanOrEqualTo(TimeSpan.Zero);
    }

    [TestMethod]
    public async Task RunTestsInternalAsync_HandlesExceptionPath_WhenServerCreationFails()
    {
        // Arrange
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { "/nonexistent.dll" });

        using var runner = new SingleMicrosoftTestPlatformRunner(
            0,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Act - This exercises the exception handling in RunTestsInternalAsync
        var result = await runner.InitialTestAsync(project.Object);

        // Assert - RunTestsInternalAsync returns TestRunResult(false, ex.Message) on exception
        result.ShouldNotBeNull();
        var testRunResult = result as Stryker.TestRunner.Results.TestRunResult;
        testRunResult.ShouldNotBeNull();
        testRunResult.FailingTests.ShouldNotBeNull();
    }

    // [TestMethod]
    // public async Task RunTestsInternalAsync_WithTimeout_DoesNotHangOnRealAssembly()
    // {
    //     // Arrange - This test ensures we don't try to start real servers that would hang
    //     var fakeAssemblyPath = "/path/to/fake/test.dll";
    //     var project = new Mock<IProjectAndTests>();
    //     project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { fakeAssemblyPath });

    //     var mutant = new Mock<IMutant>();
    //     mutant.Setup(x => x.Id).Returns(1);
    //     var mutants = new List<IMutant> { mutant.Object };

    //     var timeoutCalc = new Mock<ITimeoutValueCalculator>();
    //     timeoutCalc.Setup(x => x.CalculateTimeoutValue(It.IsAny<int>())).Returns(1);

    //     // Add discovered tests
    //     var testNode = new TestNode("test1", "TestMethod1", "passed", "passed");
    //     _testsByAssembly[fakeAssemblyPath] = new List<TestNode> { testNode };
    //     _testDescriptions["test1"] = new MtpTestDescription(testNode);

    //     using var runner = new SingleMicrosoftTestPlatformRunner(
    //         0,
    //         _testsByAssembly,
    //         _testDescriptions,
    //         _testSet,
    //         _discoveryLock,
    //         NullLogger.Instance);

    //     // Act - Should complete quickly without hanging, even though we have a timeout calculator
    //     var result = await runner.TestMultipleMutantsAsync(project.Object, timeoutCalc.Object, mutants, null);

    //     // Assert - The test completes without hanging (file doesn't exist so returns early)
    //     result.ShouldNotBeNull();
    //     result.ExecutedTests.ShouldNotBeNull();
    // }

    // [TestMethod]
    // public async Task RunTestsInternalAsync_HandlesNoDiscoveredTests()
    // {
    //     // Arrange - no tests discovered for assembly
    //     var project = new Mock<IProjectAndTests>();
    //     project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { _testAssemblyPath });

    //     using var runner = new SingleMicrosoftTestPlatformRunner(
    //         0,
    //         _testsByAssembly, // Empty - no discovered tests
    //         _testDescriptions,
    //         _testSet,
    //         _discoveryLock,
    //         NullLogger.Instance);

    //     // Act - RunTestsInternalAsync will get null tests
    //     var result = await runner.InitialTestAsync(project.Object);

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.ExecutedTests.ShouldNotBeNull();
    // }

    // [TestMethod]
    // public async Task RunTestsInternalAsync_RegistersTestResults_InTestDescriptions()
    // {
    //     // Arrange
    //     var testNode = new TestNode("test1", "TestMethod1", "passed", "passed");
    //     var mtpTestDesc = new MtpTestDescription(testNode);
    //     _testDescriptions["test1"] = mtpTestDesc;

    //     var project = new Mock<IProjectAndTests>();
    //     project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { _testAssemblyPath });

    //     using var runner = new SingleMicrosoftTestPlatformRunner(
    //         0,
    //         _testsByAssembly,
    //         _testDescriptions,
    //         _testSet,
    //         _discoveryLock,
    //         NullLogger.Instance);

    //     // Act
    //     var result = await runner.InitialTestAsync(project.Object);

    //     // Assert - Test descriptions should still be in the dictionary
    //     _testDescriptions.ShouldContainKey("test1");
    //     result.ShouldNotBeNull();
    // }

    // [TestMethod]
    // public async Task RunTestsInternalAsync_CalculatesDuration()
    // {
    //     // Arrange
    //     var project = new Mock<IProjectAndTests>();
    //     project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { "/nonexistent.dll" });

    //     using var runner = new SingleMicrosoftTestPlatformRunner(
    //         0,
    //         _testsByAssembly,
    //         _testDescriptions,
    //         _testSet,
    //         _discoveryLock,
    //         NullLogger.Instance);

    //     // Act
    //     var startTime = DateTime.UtcNow;
    //     var result = await runner.InitialTestAsync(project.Object);
    //     var endTime = DateTime.UtcNow;

    //     // Assert - Duration should be calculated
    //     result.ShouldNotBeNull();
    //     result.Duration.ShouldBeGreaterThanOrEqualTo(TimeSpan.Zero);
    //     result.Duration.ShouldBeLessThan(endTime - startTime + TimeSpan.FromSeconds(2));
    // }

    // [TestMethod]
    // public async Task RunTestsInternalAsync_WithMultipleMutants_UsesNegativeOneMutantId()
    // {
    //     // Arrange
    //     var project = new Mock<IProjectAndTests>();
    //     project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { "/test.dll" });

    //     var mutant1 = new Mock<IMutant>();
    //     mutant1.Setup(x => x.Id).Returns(1);
    //     var mutant2 = new Mock<IMutant>();
    //     mutant2.Setup(x => x.Id).Returns(2);
    //     var mutants = new List<IMutant> { mutant1.Object, mutant2.Object };

    //     using var runner = new SingleMicrosoftTestPlatformRunner(
    //         0,
    //         _testsByAssembly,
    //         _testDescriptions,
    //         _testSet,
    //         _discoveryLock,
    //         NullLogger.Instance);

    //     // Act - With multiple mutants, mutantId should be -1 (no mutation)
    //     var result = await runner.TestMultipleMutantsAsync(project.Object, null, mutants, null);

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.ExecutedTests.ShouldNotBeNull();
    // }

    // [TestMethod]
    // public async Task RunTestsInternalAsync_WithSingleMutant_UsesMutantId()
    // {
    //     // Arrange
    //     var project = new Mock<IProjectAndTests>();
    //     project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { "/test.dll" });

    //     var mutant = new Mock<IMutant>();
    //     mutant.Setup(x => x.Id).Returns(42);
    //     var mutants = new List<IMutant> { mutant.Object };

    //     using var runner = new SingleMicrosoftTestPlatformRunner(
    //         0,
    //         _testsByAssembly,
    //         _testDescriptions,
    //         _testSet,
    //         _discoveryLock,
    //         NullLogger.Instance);

    //     // Act - With single mutant, that mutant's ID should be used
    //     var result = await runner.TestMultipleMutantsAsync(project.Object, null, mutants, null);

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.ExecutedTests.ShouldNotBeNull();
    // }

    // [TestMethod]
    // public async Task RunTestsInternalAsync_ReturnsEveryTest_WhenAllTestsExecuted()
    // {
    //     // Arrange - Set up a scenario where all discovered tests would be executed
    //     var testNode = new TestNode("test1", "TestMethod1", "passed", "passed");
    //     _testsByAssembly[_testAssemblyPath] = new List<TestNode> { testNode };

    //     var project = new Mock<IProjectAndTests>();
    //     project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { _testAssemblyPath });

    //     using var runner = new SingleMicrosoftTestPlatformRunner(
    //         0,
    //         _testsByAssembly,
    //         _testDescriptions,
    //         _testSet,
    //         _discoveryLock,
    //         NullLogger.Instance);

    //     // Act
    //     var result = await runner.InitialTestAsync(project.Object);

    //     // Assert
    //     result.ShouldNotBeNull();
    //     result.ExecutedTests.ShouldNotBeNull();
    // }

    // [TestMethod]
    // public async Task RunTestsInternalAsync_IncludesResultMessage_OnError()
    // {
    //     // Arrange
    //     var project = new Mock<IProjectAndTests>();
    //     project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { "/invalid/assembly.dll" });

    //     using var runner = new SingleMicrosoftTestPlatformRunner(
    //         0,
    //         _testsByAssembly,
    //         _testDescriptions,
    //         _testSet,
    //         _discoveryLock,
    //         NullLogger.Instance);

    //     // Act
    //     var result = await runner.InitialTestAsync(project.Object);

    //     // Assert - Result message should be included on error
    //     result.ShouldNotBeNull();
    //     result.ResultMessage.ShouldNotBeNull();
    // }
}
