using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

/// <summary>
/// Regression coverage for MTP false verdicts caused by testing static mutants on a reused
/// test-host process. Static state initializes once per process, under whichever mutant is active
/// when the type first loads: a static mutant baked in that way keeps failing tests in every later
/// session (unrelated mutants reported Killed), while a static mutant tested later on a warm host
/// can never activate at all (reported Survived). The fix runs each mutant flagged IsStaticValue
/// or MustBeTestedInIsolation alone in a dedicated server: reset before the run so the mutation is
/// active during static initialization, control file cleared and reset after so neither the
/// baked-in state nor a stale mutant id can leak forward.
/// </summary>
[TestClass]
public class SingleMicrosoftTestPlatformRunnerIsolationTests
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

    private static IMutant CreateMutant(int id, bool isStaticValue = false, bool mustBeTestedInIsolation = false)
    {
        var mutant = new Mock<IMutant>();
        mutant.Setup(x => x.Id).Returns(id);
        mutant.Setup(x => x.IsStaticValue).Returns(isStaticValue);
        mutant.Setup(x => x.MustBeTestedInIsolation).Returns(mustBeTestedInIsolation);
        return mutant.Object;
    }

    private static IProjectAndTests CreateProject(string assembly)
    {
        var project = new Mock<IProjectAndTests>();
        project.Setup(x => x.GetTestAssemblies()).Returns(new List<string> { assembly });
        return project.Object;
    }

    private SessionTrackingRunner CreateRunner() =>
        new(_testsByAssembly, _testDescriptions, _testSet, _discoveryLock);

    [TestMethod, Timeout(1000)]
    public async Task TestMultipleMutantsAsync_StaticMutant_RunsActivatedOnDedicatedServer()
    {
        using var runner = CreateRunner();

        await runner.TestMultipleMutantsAsync(
            CreateProject("/test.dll"), null, [CreateMutant(7, isStaticValue: true)], null);

        // Reset before the run: the fresh host initializes static state with the mutation active.
        // Reset after the run: the state it baked in cannot bleed into the next session.
        runner.Events.ShouldBe(["reset", "run:/test.dll", "reset"]);
        runner.ActiveMutantIds.ShouldBe([7]);
    }

    [TestMethod, Timeout(1000)]
    public async Task TestMultipleMutantsAsync_MutantMarkedForIsolationByCoverage_RunsOnDedicatedServer()
    {
        using var runner = CreateRunner();

        await runner.TestMultipleMutantsAsync(
            CreateProject("/test.dll"), null, [CreateMutant(7, mustBeTestedInIsolation: true)], null);

        runner.Events.ShouldBe(["reset", "run:/test.dll", "reset"]);
        runner.ActiveMutantIds.ShouldBe([7]);
    }

    [TestMethod, Timeout(1000)]
    public async Task TestMultipleMutantsAsync_IsolatedSession_ClearsMutantFileAfterRun()
    {
        using var runner = CreateRunner();

        await runner.TestMultipleMutantsAsync(
            CreateProject("/test.dll"), null, [CreateMutant(7, isStaticValue: true)], null);

        // A server started by any later code path (e.g. discovery) must not observe the
        // static mutant id at type-load.
        runner.ReadMutantFile().ShouldBe(-1);
    }

    [TestMethod, Timeout(1000)]
    public async Task TestMultipleMutantsAsync_RegularMutant_ReusesServer()
    {
        using var runner = CreateRunner();

        await runner.TestMultipleMutantsAsync(
            CreateProject("/test.dll"), null, [CreateMutant(5)], null);

        // Process reuse is the point of the MTP runner; non-static mutants must not pay for a restart.
        runner.Events.ShouldBe(["run:/test.dll"]);
        runner.ActiveMutantIds.ShouldBe([5]);
    }

    [TestMethod, Timeout(1000)]
    public async Task TestMultipleMutantsAsync_BatchContainingStaticMutant_RunsEachMutantAloneAndActivated()
    {
        using var runner = CreateRunner();

        var result = await runner.TestMultipleMutantsAsync(
            CreateProject("/test.dll"), null, [CreateMutant(1), CreateMutant(2, isStaticValue: true)], null);

        // The control file activates a single id, so a batched session would run with no
        // mutation active (-1). Isolated batches are split: every mutant runs alone with
        // its own mutation live.
        runner.Events.ShouldBe(["reset", "run:/test.dll", "reset", "reset", "run:/test.dll", "reset"]);
        runner.ActiveMutantIds.ShouldBe([1, 2]);
        result.FailingTests.GetIdentifiers().ShouldBe(["failing-test"]);
    }

    [TestMethod, Timeout(1000)]
    public async Task TestMultipleMutantsAsync_RegularBatch_CurrentlyRunsUnmutated()
    {
        using var runner = CreateRunner();

        await runner.TestMultipleMutantsAsync(
            CreateProject("/test.dll"), null, [CreateMutant(1), CreateMutant(2)], null);

        // Canary for a pre-existing limitation, not an endorsement of it: the single-id control
        // channel cannot activate more than one mutant, so a batch of regular mutants runs with
        // no mutation active (-1). Unreachable today because MTP's aggregate coverage gives every
        // mutant overlapping assessing tests, so the executor never batches. The moment per-test
        // coverage lands (#3516/#3689) batching becomes real, and this behavior would report
        // every batched mutant as falsely Survived (original code runs, all tests pass). If this
        // test starts failing because multi-mutant activation was implemented, delete it; if
        // batching becomes reachable while it still passes, batches must be split or activation
        // extended first.
        runner.Events.ShouldBe(["run:/test.dll"]);
        runner.ActiveMutantIds.ShouldBe([-1]);
    }

    [TestMethod, Timeout(1000)]
    public async Task TestMultipleMutantsAsync_StaticMutant_ResetsServerEvenWhenRunCrashes()
    {
        using var runner = CreateRunner();
        runner.CrashOnRuns.Add(1);

        await runner.TestMultipleMutantsAsync(
            CreateProject("/test.dll"), null, [CreateMutant(1, isStaticValue: true)], null);

        // Even when the session fails, the trailing cleanup must run so the next
        // session cannot observe state from the isolated one.
        runner.Events.ShouldBe(["reset", "run:/test.dll", "reset"]);
        runner.ReadMutantFile().ShouldBe(-1);
    }

    [TestMethod, Timeout(1000)]
    public async Task TestMultipleMutantsAsync_BatchWithCrashedIsolatedMutant_ReturnsInconclusiveFlaggedResult()
    {
        using var runner = CreateRunner();
        runner.CrashOnRuns.Add(2);

        var updates = new List<(int[] MutantIds, string[] FailedTests)>();
        bool Update(IReadOnlyList<IMutant> testedMutants, ITestIdentifiers failed, ITestIdentifiers ran, ITestIdentifiers timedOut)
        {
            updates.Add((testedMutants.Select(m => m.Id).ToArray(), failed.GetIdentifiers().ToArray()));
            return true;
        }

        var result = await runner.TestMultipleMutantsAsync(
            CreateProject("/test.dll"), null,
            [CreateMutant(1, isStaticValue: true), CreateMutant(2, isStaticValue: true)], Update);

        // Attribution stays per-session: each mutant's update carries only its own outcome.
        updates.Count.ShouldBe(2);
        updates[0].MutantIds.ShouldBe([1]);
        updates[0].FailedTests.ShouldBe(["failing-test"]);
        updates[1].MutantIds.ShouldBe([2]);
        updates[1].FailedTests.ShouldBeEmpty();

        // The merged batch result must signal the crash but stay inconclusive (empty lists):
        // the executor re-analyzes a flagged multi-mutant batch with the flags dropped, so a
        // union of the siblings' results would kill or survive the crashed mutant with evidence
        // that is not its own. Empty lists leave it Pending for the single-mutant retry path.
        result.SessionHadRuntimeIssue.ShouldBeTrue();
        result.SessionTimedOut.ShouldBeFalse();
        result.FailingTests.GetIdentifiers().ShouldBeEmpty();
        result.ExecutedTests.IsEveryTest.ShouldBeFalse();
        result.ExecutedTests.GetIdentifiers().ShouldBeEmpty();
        result.TimedOutTests.GetIdentifiers().ShouldBeEmpty();
        result.ResultMessage.ShouldContain("crash");
    }

    [TestMethod, Timeout(1000)]
    public async Task TestMultipleMutantsAsync_BatchWithTimedOutIsolatedMutant_ReturnsInconclusiveFlaggedResult()
    {
        using var runner = CreateRunner();
        runner.TimeoutOnRuns.Add(2);

        var updates = new List<(int[] MutantIds, string[] FailedTests, string[] TimedOutTests)>();
        bool Update(IReadOnlyList<IMutant> testedMutants, ITestIdentifiers failed, ITestIdentifiers ran, ITestIdentifiers timedOut)
        {
            updates.Add((testedMutants.Select(m => m.Id).ToArray(),
                failed.GetIdentifiers().ToArray(),
                timedOut.GetIdentifiers().ToArray()));
            return true;
        }

        var result = await runner.TestMultipleMutantsAsync(
            CreateProject("/test.dll"), null,
            [CreateMutant(1, isStaticValue: true), CreateMutant(2, isStaticValue: true)], Update);

        updates.Count.ShouldBe(2);
        updates[0].MutantIds.ShouldBe([1]);
        updates[0].FailedTests.ShouldBe(["failing-test"]);
        updates[0].TimedOutTests.ShouldBeEmpty();
        updates[1].MutantIds.ShouldBe([2]);
        updates[1].FailedTests.ShouldBeEmpty();
        updates[1].TimedOutTests.ShouldBe(["uid-t1"]);

        // Timeout twin of the crash case above - same inconclusive-but-flagged contract:
        // keep the session flag, empty the lists, so the executor's single-mutant retry
        // path classifies the timed-out mutant instead of its siblings' results.
        result.SessionTimedOut.ShouldBeTrue();
        result.SessionHadRuntimeIssue.ShouldBeFalse();
        result.FailingTests.GetIdentifiers().ShouldBeEmpty();
        result.ExecutedTests.IsEveryTest.ShouldBeFalse();
        result.ExecutedTests.GetIdentifiers().ShouldBeEmpty();
        result.TimedOutTests.GetIdentifiers().ShouldBeEmpty();
    }

    [TestMethod, Timeout(1000)]
    public async Task TestMultipleMutantsAsync_StaticMutant_StillReportsTestResults()
    {
        using var runner = CreateRunner();

        var result = await runner.TestMultipleMutantsAsync(
            CreateProject("/test.dll"), null, [CreateMutant(1, isStaticValue: true)], null);

        result.ShouldNotBeNull();
        result.FailingTests.GetIdentifiers().ShouldBe(["failing-test"]);
    }

    [TestMethod, Timeout(1000)]
    public async Task InitialTestAsync_DoesNotUseIsolation()
    {
        using var runner = CreateRunner();

        await runner.InitialTestAsync(CreateProject("/test.dll"));

        runner.Events.ShouldBe(["run:/test.dll"]);
        runner.ActiveMutantIds.ShouldBe([-1]);
    }

    /// <summary>
    /// Records the order of server resets and assembly runs, and the mutant id the control file
    /// holds while each run executes (what a real test host would activate), instead of starting
    /// real test servers. Uses a runner id no other test class shares so the control file cannot
    /// be touched by concurrent tests.
    /// </summary>
    private sealed class SessionTrackingRunner : SingleMicrosoftTestPlatformRunner
    {
        private const int RunnerId = 970;

        public List<string> Events { get; } = [];
        public List<int> ActiveMutantIds { get; } = [];
        public HashSet<int> CrashOnRuns { get; } = [];
        public HashSet<int> TimeoutOnRuns { get; } = [];

        public SessionTrackingRunner(
            Dictionary<string, List<TestNode>> testsByAssembly,
            Dictionary<string, MtpTestDescription> testDescriptions,
            TestSet testSet,
            object discoveryLock)
            : base(RunnerId, testsByAssembly, testDescriptions, testSet, discoveryLock, NullLogger.Instance)
        {
        }

        public int ReadMutantFile()
        {
            var path = Path.Combine(Path.GetTempPath(), $"stryker-mutant-{RunnerId}.txt");
            return BitConverter.ToInt32(File.ReadAllBytes(path), 0);
        }

        public override async Task ResetServerAsync()
        {
            Events.Add("reset");
            await base.ResetServerAsync();
        }

        internal override Task<(TestRunResult? Result, bool TimedOut, List<TestNode>? DiscoveredTests)> RunAssemblyTestsAsync(
            string assembly, ITimeoutValueCalculator? timeoutCalc)
        {
            Events.Add($"run:{assembly}");
            ActiveMutantIds.Add(ReadMutantFile());
            var runNumber = ActiveMutantIds.Count;

            if (CrashOnRuns.Contains(runNumber))
            {
                return Task.FromResult<(TestRunResult?, bool, List<TestNode>?)>(
                    (new TestRunResult(false, "simulated test host crash"), false, null));
            }

            if (TimeoutOnRuns.Contains(runNumber))
            {
                // The runner only registers a timeout when the assembly has discovered tests,
                // so report one alongside the timed-out flag.
                var timedOutResult = new TestRunResult(
                    Array.Empty<IFrameworkTestDescription>(),
                    TestIdentifierList.EveryTest(),
                    TestIdentifierList.NoTest(),
                    TestIdentifierList.NoTest(),
                    string.Empty,
                    Enumerable.Empty<string>(),
                    TimeSpan.Zero);
                return Task.FromResult<(TestRunResult?, bool, List<TestNode>?)>(
                    (timedOutResult, true, [new TestNode("uid-t1", "T1", "test", "discovered")]));
            }

            var result = new TestRunResult(
                Array.Empty<IFrameworkTestDescription>(),
                TestIdentifierList.EveryTest(),
                new TestIdentifierList("failing-test"),
                TestIdentifierList.NoTest(),
                string.Empty,
                Enumerable.Empty<string>(),
                TimeSpan.Zero);
            return Task.FromResult<(TestRunResult?, bool, List<TestNode>?)>((result, false, null));
        }
    }
}
