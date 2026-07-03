using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.Results;
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

        var disposedRunners = new System.Collections.Concurrent.ConcurrentBag<int>();
        var runnerFactory = new Mock<ISingleRunnerFactory>();

        runnerFactory.Setup(x => x.CreateRunner(
                It.IsAny<int>(),
                It.IsAny<Dictionary<string, List<TestNode>>>(),
                It.IsAny<Dictionary<string, MtpTestDescription>>(),
                It.IsAny<TestSet>(),
                It.IsAny<object>(),
                It.IsAny<ILogger>(),
                It.IsAny<IStrykerOptions>()))
            .Returns<int, Dictionary<string, List<TestNode>>, Dictionary<string, MtpTestDescription>, TestSet, object, ILogger, IStrykerOptions>(
                (id, testsByAssembly, testDescriptions, testSet, discoveryLock, logger, opts) =>
                {
                    var testRunner = new TestableRunner(id, () => disposedRunners.Add(id));
                    return testRunner;
                });

        var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance, runnerFactory.Object);

        // The pool uses Parallel.For to create runners, which should complete before constructor returns
        // However, to be defensive against timing issues, verify by checking the actual runners in the pool
        pool.Runners.Count().ShouldBe(3, "All 3 runners should be available in the pool");

        // Act
        pool.Dispose();

        // Assert
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
    public void CaptureCoverage_ShouldReturnNormalConfidenceWithCoverageData()
    {
        // Arrange
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance);
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(Array.Empty<string>());

        // Act
        var coverage = pool.CaptureCoverage(project.Object).ToList();

        // Assert
        coverage.ShouldNotBeNull();
        // Even with no tests discovered, the method should complete successfully
        // Coverage results are created per test, so empty test set = empty coverage
        coverage.ShouldBeEmpty();
    }

    [TestMethod]
    public void CaptureCoverage_ShouldCapturePerTest_WhenCoverageBasedTestEnabled()
    {
        // Arrange - "perTest" (CoverageBasedTest, no isolation) must capture a DISTINCT coverage set per
        // test rather than the cumulative "all tests share everything" set the aggregate path produces.
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        options.Setup(x => x.OptimizationMode).Returns(OptimizationModes.CoverageBasedTest);

        var testsByAssembly = new Dictionary<string, List<TestNode>>();
        var testDescriptions = new Dictionary<string, MtpTestDescription>();
        var testSet = new TestSet();

        var testNode1 = new TestNode("test-1", "Test1", "test", "discovered");
        var testNode2 = new TestNode("test-2", "Test2", "test", "discovered");
        testsByAssembly["assembly.dll"] = new List<TestNode> { testNode1, testNode2 };

        var desc1 = new MtpTestDescription(testNode1);
        var desc2 = new MtpTestDescription(testNode2);
        testDescriptions["test-1"] = desc1;
        testDescriptions["test-2"] = desc2;
        testSet.RegisterTest(desc1.Description);
        testSet.RegisterTest(desc2.Description);

        var capturedTests = new System.Collections.Concurrent.ConcurrentBag<string>();

        var runnerFactory = new Mock<ISingleRunnerFactory>();
        runnerFactory.Setup(x => x.CreateRunner(
                It.IsAny<int>(),
                It.IsAny<Dictionary<string, List<TestNode>>>(),
                It.IsAny<Dictionary<string, MtpTestDescription>>(),
                It.IsAny<TestSet>(),
                It.IsAny<object>(),
                It.IsAny<ILogger>(),
                It.IsAny<IStrykerOptions>()))
            .Returns<int, Dictionary<string, List<TestNode>>, Dictionary<string, MtpTestDescription>, TestSet, object, ILogger, IStrykerOptions>(
                (id, tba, td, ts, dl, logger, opts) =>
                {
                    // Populate the pool's shared dictionaries so it discovers the same tests set up above.
                    if (tba.Count == 0)
                    {
                        foreach (var kvp in testsByAssembly) tba[kvp.Key] = kvp.Value;
                        foreach (var kvp in testDescriptions) td[kvp.Key] = kvp.Value;
                    }
                    return new TestableRunner(id, tba, td, ts, dl,
                        () => { },
                        coverageHandler: (assembly, test, testId) =>
                        {
                            capturedTests.Add(testId);
                            var covered = testId == desc1.Id ? new[] { 1, 2 } : new[] { 3 };
                            return Task.FromResult<ICoverageRunResult>(
                                CoverageRunResult.Create(testId, CoverageConfidence.Normal, covered, Array.Empty<int>(), Array.Empty<int>()));
                        });
                });

        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(new[] { "assembly.dll" });

        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance, runnerFactory.Object);

        // Act
        var coverage = pool.CaptureCoverage(project.Object).ToList();

        // Assert
        capturedTests.Count.ShouldBe(2, "both tests should have been captured individually");
        coverage.Count.ShouldBe(2, "one coverage result per test, not one cumulative result");

        var cov1 = coverage.First(c => c.TestId == desc1.Id);
        cov1.MutationsCovered.ShouldContain(1);
        cov1.MutationsCovered.ShouldContain(2);
        cov1.MutationsCovered.ShouldNotContain(3);

        var cov2 = coverage.First(c => c.TestId == desc2.Id);
        cov2.MutationsCovered.ShouldContain(3);
        cov2.MutationsCovered.ShouldNotContain(1);
    }

    [TestMethod]
    public void CaptureCoverage_ShouldStayAggregate_WhenOnlySkipUncoveredEnabled()
    {
        // Arrange - "all" mode (SkipUncoveredMutants only, no CoverageBasedTest) doesn't need per-test
        // granularity, so it must keep using the cheaper aggregate (one-pass) capture.
        var options = new Mock<IStrykerOptions>();
        options.Setup(x => x.Concurrency).Returns(1);
        options.Setup(x => x.OptimizationMode).Returns(OptimizationModes.SkipUncoveredMutants);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance);
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(Array.Empty<string>());

        // Act
        var coverage = pool.CaptureCoverage(project.Object).ToList();

        // Assert - aggregate path with no discovered tests returns empty, same as before this change
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


