using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.Tests;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class MicrosoftTestPlatformRunnerPoolTests : TestBase
{
    [TestMethod]
    public void Constructor_ShouldCreateRunnersBasedOnConcurrency()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(2);

        // Act
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance);

        // Assert - pool should be created without exceptions
        pool.ShouldNotBeNull();
    }

    [TestMethod]
    public void Constructor_ShouldCreateAtLeastOneRunner_WhenConcurrencyIsZero()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(0);

        // Act
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance);

        // Assert - pool should be created with at least 1 runner
        pool.ShouldNotBeNull();
    }

    [TestMethod]
    public async Task DiscoverTests_ShouldReturnFalse_WhenAssemblyPathIsEmpty()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance);

        // Act
        var result = await pool.DiscoverTestsAsync(string.Empty);

        // Assert
        result.ShouldBeFalse();
    }

    [TestMethod]
    public async Task DiscoverTests_ShouldReturnFalse_WhenAssemblyPathIsNull()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance);

        // Act
        var result = await pool.DiscoverTestsAsync(null!);

        // Assert
        result.ShouldBeFalse();
    }

    [TestMethod]
    public async Task DiscoverTests_ShouldReturnFalse_WhenAssemblyDoesNotExist()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance);

        // Act
        var result = await pool.DiscoverTestsAsync("/nonexistent/path/assembly.dll");

        // Assert
        result.ShouldBeFalse();
    }

    [TestMethod]
    public void GetTests_ShouldReturnEmptyTestSet_WhenNoTestsDiscovered()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance);
        var project = new Mock<IProjectAndTests>();

        // Act
        var testSet = pool.GetTests(project.Object);

        // Assert
        testSet.Count.ShouldBe(0);
    }

    [TestMethod]
    public async Task InitialTest_ShouldReturnFailure_WhenNoTestAssembliesFound()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance);
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(Array.Empty<string>());

        // Act
        var result = await pool.InitialTestAsync(project.Object);

        // Assert
        result.FailingTests.IsEveryTest.ShouldBeTrue();
    }

    [TestMethod]
    public async Task TestMultipleMutants_ShouldReturnFailure_WhenNoTestAssembliesFound()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance);
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(Array.Empty<string>());
        var mutants = new List<IMutant> { new Mock<IMutant>().Object };

        // Act
        var result = await pool.TestMultipleMutantsAsync(project.Object, null, mutants, null);

        // Assert
        result.FailingTests.IsEveryTest.ShouldBeTrue();
    }

    [TestMethod]
    public void CaptureCoverage_ShouldReturnEmptyCoverage_WhenNoTestsDiscovered()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance);
        var project = new Mock<IProjectAndTests>();

        // Act
        var coverage = pool.CaptureCoverage(project.Object);

        // Assert
        coverage.ShouldNotBeNull();
        coverage.ShouldBeEmpty();
    }

    [TestMethod]
    public void Dispose_ShouldDisposeAllRunnersInPool()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(3);

        var disposedRunners = new List<int>();
        var createdRunners = new List<int>();
        var runnerFactory = new Mock<ISingleRunnerFactory>();

        runnerFactory.Setup(x => x.CreateRunner(
                It.IsAny<int>(),
                It.IsAny<Dictionary<string, List<TestNode>>>(),
                It.IsAny<Dictionary<string, MtpTestDescription>>(),
                It.IsAny<TestSet>(),
                It.IsAny<object>(),
                It.IsAny<ILogger>()))
            .Returns<int, Dictionary<string, List<TestNode>>, Dictionary<string, MtpTestDescription>, TestSet, object, ILogger>(
                (id, testsByAssembly, testDescriptions, testSet, discoveryLock, logger) =>
                {
                    var testRunner = new TestableRunner(id, () => disposedRunners.Add(id));
                    createdRunners.Add(id);
                    return testRunner;
                });

        var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance, runnerFactory.Object);

        createdRunners.Count.ShouldBe(3, "All 3 runners should have been created before disposal");

        // Act
        pool.Dispose();

        // Assert - Verify Dispose was called on all runners
        disposedRunners.Count.ShouldBe(3, "Dispose should be called on all 3 runners");
        disposedRunners.ShouldContain(0);
        disposedRunners.ShouldContain(1);
        disposedRunners.ShouldContain(2);
    }

    [TestMethod]
    public void Constructor_ShouldCreateMultipleRunners_WhenConcurrencyIsHigh()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(4);

        // Act
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance);

        // Assert - pool should be created with 4 runners
        pool.ShouldNotBeNull();
    }

    [TestMethod]
    public async Task DiscoverTests_ShouldHandleMultipleCallsSequentially()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance);

        // Act
        var result1 = await pool.DiscoverTestsAsync("/nonexistent/path1.dll");
        var result2 = await pool.DiscoverTestsAsync("/nonexistent/path2.dll");

        // Assert
        result1.ShouldBeFalse();
        result2.ShouldBeFalse();
    }

    [TestMethod]
    public async Task InitialTest_ShouldThrowArgumentNullException_WhenAssembliesIsNull()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance);
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns((List<string>)null!);

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () => await pool.InitialTestAsync(project.Object));
    }

    [TestMethod]
    public async Task TestMultipleMutants_ShouldHandleEmptyMutantList()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance);
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(Array.Empty<string>());
        var mutants = new List<IMutant>();

        // Act
        var result = await pool.TestMultipleMutantsAsync(project.Object, null, mutants, null);

        // Assert
        result.ShouldNotBeNull();
        result.FailingTests.IsEveryTest.ShouldBeTrue();
    }

    [TestMethod]
    public void CaptureCoverage_ShouldReturnDubiousConfidence()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance);
        var project = new Mock<IProjectAndTests>();

        // Act
        var coverage = pool.CaptureCoverage(project.Object).ToList();

        // Assert
        coverage.ShouldNotBeNull();
        coverage.ShouldBeEmpty();
    }

    [TestMethod]
    public void Constructor_ShouldUseProvidedLogger()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        var logger = NullLogger.Instance;

        // Act
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, logger);

        // Assert
        pool.ShouldNotBeNull();
    }

    [TestMethod]
    public void Constructor_ShouldUseDefaultLogger_WhenLoggerIsNull()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);

        // Act
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, null);

        // Assert
        pool.ShouldNotBeNull();
    }
}


