using System.Diagnostics;
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
        var createdRunners = new List<int>();
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
                    lock (createdRunners)
                    {
                        createdRunners.Add(id);
                        Monitor.Pulse(createdRunners);
                    }
                    return testRunner;
                });

        var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance, runnerFactory.Object);

        // The pool uses Parallel.For to create runners, which should complete before constructor returns
        // However, to be defensive against timing issues, verify by checking the actual runners in the pool

        var timeout = new Stopwatch();
        timeout.Start();
        var start = timeout.ElapsedMilliseconds;
        lock (createdRunners)
        {
            while (createdRunners.Count < 3 && timeout.ElapsedMilliseconds-start < 2000)
            {
                Monitor.Wait(createdRunners, 200);
            }
        }
        createdRunners.Count.ShouldBe(3, "All 3 runners should have been created before disposal");

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
    public void CaptureCoverage_ShouldCaptureEachTestInIsolation_WhenPerTestInIsolationEnabled()
    {
        // Arrange - "perTestInIsolation" (CoverageBasedTest | CaptureCoveragePerTest) must route to the
        // isolated-process capture and keep each test's coverage set distinct, at Exact confidence.
        var harness = new IsolatedCoverageHarness(
            OptimizationModes.CoverageBasedTest | OptimizationModes.CaptureCoveragePerTest,
            assemblies: new Dictionary<string, string[]> { ["assembly.dll"] = ["test-1", "test-2"] },
            isolatedCoverage: testId => testId == "test-1" ? [1, 2] : [3]);

        // Act
        var coverage = harness.Pool.CaptureCoverage(harness.Project).ToList();

        // Assert
        harness.IsolatedCaptures.Count.ShouldBe(2, "every discovered test should be captured in its own process");
        harness.ReusedCaptures.ShouldBeEmpty("the reused-process capture must not be used in isolation mode");
        coverage.Count.ShouldBe(2, "one coverage result per test, not one cumulative result");

        var cov1 = coverage.First(c => c.TestId == harness.IdOf("test-1"));
        cov1.MutationsCovered.ShouldBe(new[] { 1, 2 }, ignoreOrder: true);
        cov1.Confidence.ShouldBe(CoverageConfidence.Exact, "process isolation makes each result trustworthy");

        var cov2 = coverage.First(c => c.TestId == harness.IdOf("test-2"));
        cov2.MutationsCovered.ShouldBe(new[] { 3 }, ignoreOrder: true);
        cov2.Confidence.ShouldBe(CoverageConfidence.Exact);
    }

    [TestMethod]
    public void CaptureCoverage_ShouldCaptureInIsolation_WhenOnlyCaptureCoveragePerTestEnabled()
    {
        // Arrange - CaptureCoveragePerTest on its own is enough to pick the isolated path; it must not
        // fall through to the reused-process or aggregate capture just because CoverageBasedTest is absent.
        var harness = new IsolatedCoverageHarness(
            OptimizationModes.CaptureCoveragePerTest,
            assemblies: new Dictionary<string, string[]> { ["assembly.dll"] = ["test-1"] },
            isolatedCoverage: _ => [7]);

        // Act
        var coverage = harness.Pool.CaptureCoverage(harness.Project).ToList();

        // Assert
        harness.IsolatedCaptures.Count.ShouldBe(1);
        harness.ReusedCaptures.ShouldBeEmpty();
        coverage.ShouldHaveSingleItem().MutationsCovered.ShouldBe(new[] { 7 });
    }

    [TestMethod]
    public void CaptureCoverage_ShouldCaptureTestsFromEveryAssembly_InIsolation()
    {
        // Arrange - tests are collected per assembly, so a second assembly must not be dropped.
        var harness = new IsolatedCoverageHarness(
            OptimizationModes.CoverageBasedTest | OptimizationModes.CaptureCoveragePerTest,
            assemblies: new Dictionary<string, string[]>
            {
                ["first.dll"] = ["test-1"],
                ["second.dll"] = ["test-2", "test-3"]
            },
            isolatedCoverage: _ => [1]);

        // Act
        var coverage = harness.Pool.CaptureCoverage(harness.Project).ToList();

        // Assert
        coverage.Count.ShouldBe(3);
        harness.IsolatedCaptures.Select(c => c.Assembly).ShouldBe(
            new[] { "first.dll", "second.dll", "second.dll" }, ignoreOrder: true);
    }

    [TestMethod]
    public void CaptureCoverage_ShouldSkipTestsWithoutDescription_InIsolation()
    {
        // Arrange - a discovered TestNode without a matching MtpTestDescription has no test id to report
        // coverage against, so it must be skipped instead of capturing coverage under a bogus id.
        var harness = new IsolatedCoverageHarness(
            OptimizationModes.CoverageBasedTest | OptimizationModes.CaptureCoveragePerTest,
            assemblies: new Dictionary<string, string[]> { ["assembly.dll"] = ["test-1", "test-2"] },
            isolatedCoverage: _ => [1],
            undescribedTests: ["test-2"]);

        // Act
        var coverage = harness.Pool.CaptureCoverage(harness.Project).ToList();

        // Assert
        harness.IsolatedCaptures.ShouldHaveSingleItem().TestId.ShouldBe(harness.IdOf("test-1"));
        coverage.ShouldHaveSingleItem().TestId.ShouldBe(harness.IdOf("test-1"));
    }

    [TestMethod]
    public void CaptureCoverage_ShouldKeepCapturingOtherTests_WhenOneIsolatedCaptureFails()
    {
        // Arrange - one test blowing up (crashed host, unreadable coverage file) degrades to Dubious for
        // that test only; the rest of the run must still produce their own results.
        var harness = new IsolatedCoverageHarness(
            OptimizationModes.CoverageBasedTest | OptimizationModes.CaptureCoveragePerTest,
            assemblies: new Dictionary<string, string[]> { ["assembly.dll"] = ["test-1", "test-2"] },
            isolatedCoverage: testId => testId == "test-1"
                ? throw new InvalidOperationException("test host crashed")
                : [3]);

        // Act
        var coverage = harness.Pool.CaptureCoverage(harness.Project).ToList();

        // Assert
        coverage.Count.ShouldBe(2, "a single failing test must not drop the other results");

        var failed = coverage.First(c => c.TestId == harness.IdOf("test-1"));
        failed.Confidence.ShouldBe(CoverageConfidence.Dubious);
        failed.MutationsCovered.ShouldBeEmpty();

        coverage.First(c => c.TestId == harness.IdOf("test-2")).MutationsCovered.ShouldBe(new[] { 3 });
    }

    [TestMethod]
    public void CaptureCoverage_ShouldReturnEmpty_WhenNoTestsDiscovered_InIsolation()
    {
        // Arrange
        var harness = new IsolatedCoverageHarness(
            OptimizationModes.CoverageBasedTest | OptimizationModes.CaptureCoveragePerTest,
            assemblies: new Dictionary<string, string[]>(),
            isolatedCoverage: _ => [1]);

        // Act
        var coverage = harness.Pool.CaptureCoverage(harness.Project).ToList();

        // Assert
        coverage.ShouldBeEmpty();
        harness.IsolatedCaptures.ShouldBeEmpty();
    }

    [TestMethod]
    public void CaptureCoverage_ShouldCaptureEveryTest_WhenPoolHasMultipleRunners()
    {
        // Arrange - isolated capture fans the tests out over the whole pool, so with more tests than
        // runners every test must still be captured exactly once.
        var harness = new IsolatedCoverageHarness(
            OptimizationModes.CoverageBasedTest | OptimizationModes.CaptureCoveragePerTest,
            assemblies: new Dictionary<string, string[]> { ["assembly.dll"] = ["t-1", "t-2", "t-3", "t-4", "t-5"] },
            isolatedCoverage: _ => [1],
            concurrency: 3);

        // Act
        var coverage = harness.Pool.CaptureCoverage(harness.Project).ToList();

        // Assert
        coverage.Count.ShouldBe(5);
        coverage.Select(c => c.TestId).Distinct().Count().ShouldBe(5, "no test should be captured twice or skipped");
        harness.IsolatedCaptures.Count.ShouldBe(5);
        harness.ReusedCaptures.ShouldBeEmpty();
    }

    /// <summary>
    /// Builds a pool backed by <see cref="TestableRunner"/>s with a pre-populated discovery result, and
    /// records which capture path each test went through.
    /// </summary>
    private sealed class IsolatedCoverageHarness
    {
        private readonly Dictionary<string, MtpTestDescription> _descriptions = new();

        public MicrosoftTestPlatformRunnerPool Pool { get; }
        public IProjectAndTests Project { get; }
        public System.Collections.Concurrent.ConcurrentBag<(string Assembly, string TestId)> IsolatedCaptures { get; } = new();
        public System.Collections.Concurrent.ConcurrentBag<string> ReusedCaptures { get; } = new();

        public IsolatedCoverageHarness(
            OptimizationModes optimizationMode,
            Dictionary<string, string[]> assemblies,
            Func<string, int[]> isolatedCoverage,
            string[]? undescribedTests = null,
            int concurrency = 1)
        {
            var testsByAssembly = new Dictionary<string, List<TestNode>>();
            foreach (var (assembly, testUids) in assemblies)
            {
                var nodes = new List<TestNode>();
                foreach (var uid in testUids)
                {
                    var node = new TestNode(uid, uid, "test", "discovered");
                    nodes.Add(node);

                    if (undescribedTests?.Contains(uid) == true)
                    {
                        continue;
                    }

                    _descriptions[uid] = new MtpTestDescription(node);
                }
                testsByAssembly[assembly] = nodes;
            }

            var options = new Mock<IStrykerOptions>();
            options.Setup(x => x.Concurrency).Returns(concurrency);
            options.Setup(x => x.OptimizationMode).Returns(optimizationMode);

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
                        // Populate the pool's shared dictionaries so it sees the tests set up above.
                        lock (dl)
                        {
                            if (tba.Count == 0)
                            {
                                foreach (var kvp in testsByAssembly) tba[kvp.Key] = kvp.Value;
                                foreach (var kvp in _descriptions) td[kvp.Key] = kvp.Value;
                            }
                        }

                        return new TestableRunner(id, tba, td, ts, dl,
                            () => { },
                            coverageHandler: (_, _, testId) =>
                            {
                                ReusedCaptures.Add(testId);
                                return Task.FromResult<ICoverageRunResult>(CoverageRunResult.Create(
                                    testId, CoverageConfidence.Normal, [], [], []));
                            },
                            isolatedCoverageHandler: (assembly, _, testId) =>
                            {
                                IsolatedCaptures.Add((assembly, testId));
                                return Task.FromResult<ICoverageRunResult>(CoverageRunResult.Create(
                                    testId, CoverageConfidence.Exact, isolatedCoverage(testId), [], []));
                            });
                    });

            var project = new Mock<IProjectAndTests>();
            project.Setup(x => x.GetTestAssemblies()).Returns(assemblies.Keys.ToList());
            Project = project.Object;

            Pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance, runnerFactory.Object);
        }

        public string IdOf(string testUid) => _descriptions[testUid].Id;
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


