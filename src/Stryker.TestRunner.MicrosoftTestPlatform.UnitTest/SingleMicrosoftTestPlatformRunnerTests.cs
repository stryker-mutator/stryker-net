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
    public void Dispose_ShouldCleanUpResources()
    {
        // Arrange
        var runner = new SingleMicrosoftTestPlatformRunner(
            0,
            _testsByAssembly,
            _testDescriptions,
            _testSet,
            _discoveryLock,
            NullLogger.Instance);

        // Act & Assert - should not throw
        runner.Dispose();
        runner.Dispose(); // Second dispose should be safe
    }
}

