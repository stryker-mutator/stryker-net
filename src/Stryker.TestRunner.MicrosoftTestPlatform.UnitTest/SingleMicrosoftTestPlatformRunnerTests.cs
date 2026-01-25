using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.TestRunner.Tests;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;

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

    [TestMethod]
    public void Constructor_ShouldCreateRunner()
    {
        // Act
        using var runner = new SingleMicrosoftTestPlatformRunner(
            0,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Assert
        runner.ShouldNotBeNull();
    }

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
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

        // Act - First disposal
        testableRunner.Dispose();

        // Assert
        testableRunner.DisposedFlagWasSet.ShouldBeTrue("_disposed flag should be set to true");
        testableRunner.DisposeLogicExecutedCount.ShouldBe(1, "Dispose logic should execute once on first call");
        File.Exists(mutantFilePath).ShouldBeFalse("Mutant file should be deleted after disposal");

        // Act - Second disposal (verify idempotency via _disposed flag check)
        testableRunner.Dispose();

        // Assert - disposal count should not increase (verifies early return from _disposed flag check)
        testableRunner.DisposeLogicExecutedCount.ShouldBe(1, "Dispose logic should only execute once due to _disposed flag check preventing re-execution");
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

        public override void Dispose()
        {
            // We need to detect if the disposal logic actually runs
            // The base Dispose checks _disposed flag first and returns early if already disposed
            // We'll use reflection to check the _disposed field before and after
            var disposedField = typeof(SingleMicrosoftTestPlatformRunner).GetField("_disposed", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            var wasDisposedBefore = (bool)disposedField!.GetValue(this)!;

            // Call base dispose
            base.Dispose();

            var wasDisposedAfter = (bool)disposedField!.GetValue(this)!;

            // If _disposed changed from false to true, the disposal logic executed
            if (!wasDisposedBefore && wasDisposedAfter)
            {
                _disposeLogicExecutedCount++;
                DisposedFlagWasSet = true;
            }
        }
    }
}

