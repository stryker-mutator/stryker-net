using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

/// <summary>
/// Host lifecycle around a mutant session: warm-up on a fresh host, recycling of a poisoned host and
/// the mutant id bookkeeping (stryker-mutator/stryker-net#3832).
/// </summary>
[TestClass]
public class MicrosoftTestingPlatformRunnerWarmUpTests
{
    private const string Assembly = "warm-up.dll";

    private static AssemblyTestServer CreateServer() =>
        new(Assembly, new Dictionary<string, string?>(), NullLogger.Instance, "MtpRunner-0");

    private static TestNodeUpdate Update(string state, string? message = null, string? stackTrace = null) =>
        new(new TestNode("uid", "Test", "action", state, ErrorMessage: message, ErrorStackTrace: stackTrace), "root");

    [TestMethod]
    public async Task WarmUpServerAsync_RunsTestsWithNoMutantActive_ThenRestoresTheMutantId()
    {
        using var runner = new WarmUpRunner(timedOut: false) { ActiveMutantId = 42 };
        using var server = CreateServer();

        var ready = await runner.WarmUpServerAsync(server, Assembly, null);

        ready.ShouldBeTrue();
        server.IsWarmedUp.ShouldBeTrue();
        runner.WarmUpCalls.ShouldBe(1);
        runner.IdSeenDuringWarmUp.ShouldBe(-1);
        ReadMutantId(runner).ShouldBe(42);
    }

    [TestMethod]
    public async Task WarmUpServerAsync_SkipsTheRun_WhenNoMutantIsActive()
    {
        using var runner = new WarmUpRunner(timedOut: false) { ActiveMutantId = -1 };
        using var server = CreateServer();

        var ready = await runner.WarmUpServerAsync(server, Assembly, null);

        ready.ShouldBeTrue();
        server.IsWarmedUp.ShouldBeTrue();
        runner.WarmUpCalls.ShouldBe(0);
    }

    [TestMethod]
    public async Task WarmUpServerAsync_ReportsFailureAndLeavesTheServerCold_WhenTheWarmUpTimesOut()
    {
        using var runner = new WarmUpRunner(timedOut: true) { ActiveMutantId = 7 };
        using var server = CreateServer();

        var ready = await runner.WarmUpServerAsync(server, Assembly, null);

        ready.ShouldBeFalse();
        server.IsWarmedUp.ShouldBeFalse();
        ReadMutantId(runner).ShouldBe(7);
    }

    [TestMethod]
    public async Task WarmUpServerAsync_DiscardsTheHost_WhenTheWarmUpReportsATypeInitializerFailure()
    {
        using var runner = new WarmUpRunner(timedOut: false, warmUpUpdates: [Update(TestNodeStates.Error, "System.TypeInitializationException : boom")]) { ActiveMutantId = 42 };
        using var server = CreateServer();

        var ready = await runner.WarmUpServerAsync(server, Assembly, null);

        // The host initialized a poisoned type with no mutant active: the first mutant would be
        // falsely killed on it, so it must not be marked warmed.
        ready.ShouldBeFalse();
        server.IsWarmedUp.ShouldBeFalse();
        ReadMutantId(runner).ShouldBe(42);
    }

    [TestMethod]
    public async Task WarmUpServerAsync_MarksTheHostWarm_WhenTheWarmUpReportsOnlyPlainFailures()
    {
        // Continuing after initial test failures is a supported choice; only a failed type
        // initializer poisons the host.
        using var runner = new WarmUpRunner(timedOut: false, warmUpUpdates: [Update(TestNodeStates.Failed, "Assert.Equal() Failure", "at Tests.Foo()")]) { ActiveMutantId = 42 };
        using var server = CreateServer();

        var ready = await runner.WarmUpServerAsync(server, Assembly, null);

        ready.ShouldBeTrue();
        server.IsWarmedUp.ShouldBeTrue();
    }

    [TestMethod]
    public async Task WarmUpServerAsync_RestoresTheMutantId_WhenTheWarmUpThrows()
    {
        using var runner = new WarmUpRunner(timedOut: false, failure: new InvalidOperationException("host crashed")) { ActiveMutantId = 13 };
        using var server = CreateServer();

        await Should.ThrowAsync<InvalidOperationException>(() => runner.WarmUpServerAsync(server, Assembly, null));

        server.IsWarmedUp.ShouldBeFalse();
        ReadMutantId(runner).ShouldBe(13);
    }

    [TestMethod]
    public async Task WarmUpServerAsync_FailsWithoutRunningOrMarkingTheHostWarm_WhenTheControlFileCannotBeWritten()
    {
        using var runner = new WarmUpRunner(timedOut: false) { ActiveMutantId = 9 };
        using var server = CreateServer();
        CloseControlFile(runner);

        await Should.ThrowAsync<InvalidOperationException>(() => runner.WarmUpServerAsync(server, Assembly, null));

        runner.WarmUpCalls.ShouldBe(0);
        server.IsWarmedUp.ShouldBeFalse();
    }

    [TestMethod, Timeout(2000)]
    public async Task RunAllTestsAsync_FailsTheSession_WhenTheMutantIdCannotBePublished()
    {
        using var runner = new SessionRunner();
        CloseControlFile(runner);

        var result = await runner.RunAllTestsAsync([Assembly], mutantId: 4, mutants: null, update: null);

        result.ResultMessage.ShouldContain("control file");
        runner.Sessions.ShouldBe(0);
    }

    // Disposing the cached stream makes every later write fail, like a control file that went away.
    private static void CloseControlFile(MicrosoftTestingPlatformRunner runner)
    {
        var field = typeof(MicrosoftTestingPlatformRunner).GetField("_mutantFileStream", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        ((FileStream)field!.GetValue(runner)!).Dispose();
    }

    [TestMethod]
    public void OpenMutantControlFile_ReleasesTheStream_WhenResizingFails()
    {
        var path = Path.Combine(Path.GetTempPath(), $"stryker-control-{Guid.NewGuid():N}.txt");
        try
        {
            Should.Throw<IOException>(() => MicrosoftTestingPlatformRunner.OpenMutantControlFile(path, _ => throw new IOException("disk full")));

            // A leaked handle would keep the file locked for exclusive access.
            using var exclusive = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [TestMethod]
    public void OpenMutantControlFile_SizesAnEmptyFile_AndKeepsTheStreamOpen()
    {
        var path = Path.Combine(Path.GetTempPath(), $"stryker-control-{Guid.NewGuid():N}.txt");
        try
        {
            using var stream = MicrosoftTestingPlatformRunner.OpenMutantControlFile(path, s => s.SetLength(sizeof(int)));

            stream.Length.ShouldBe(sizeof(int));
            stream.CanWrite.ShouldBeTrue();
        }
        finally
        {
            File.Delete(path);
        }
    }

    [TestMethod]
    public void OpenMutantControlFile_DoesNotResizeAFileThatIsAlreadyLongEnough()
    {
        var path = Path.Combine(Path.GetTempPath(), $"stryker-control-{Guid.NewGuid():N}.txt");
        try
        {
            File.WriteAllBytes(path, [1, 2, 3, 4]);
            var resized = false;

            using var stream = MicrosoftTestingPlatformRunner.OpenMutantControlFile(path, _ => resized = true);

            resized.ShouldBeFalse();
            stream.Length.ShouldBe(4);
        }
        finally
        {
            File.Delete(path);
        }
    }
    [TestMethod]
    public async Task WarmUpServerAsync_RunsEveryDiscoveredTest()
    {
        using var runner = new WarmUpRunner(timedOut: false) { ActiveMutantId = 3 };
        using var server = CreateServer();
        var tests = new List<TestNode>
        {
            new("t1", "t1", "action", TestNodeStates.Discovered),
            new("t2", "t2", "action", TestNodeStates.Discovered),
        };

        await runner.WarmUpServerAsync(server, Assembly, tests);

        runner.TestsSeen.ShouldBe(["t1", "t2"]);
    }

    [TestMethod]
    public async Task WarmUpServerAsync_TwiceInARow_LeavesTheMutantIdAlone()
    {
        using var runner = new WarmUpRunner(timedOut: false) { ActiveMutantId = 21 };
        using var first = CreateServer();
        using var second = CreateServer();

        await runner.WarmUpServerAsync(first, Assembly, null);
        await runner.WarmUpServerAsync(second, Assembly, null);

        runner.WarmUpCalls.ShouldBe(2);
        ReadMutantId(runner).ShouldBe(21);
    }

    [TestMethod]
    public async Task WarmUpServerAsync_PassesTheGivenTimeoutToTheRun()
    {
        using var runner = new WarmUpRunner(timedOut: false) { ActiveMutantId = 5 };
        using var server = CreateServer();

        await runner.WarmUpServerAsync(server, Assembly, null, TimeSpan.FromMinutes(45));

        runner.TimeoutSeen.ShouldBe(TimeSpan.FromMinutes(45));
    }

    [TestMethod]
    public void CalculateWarmUpTimeout_ScalesWithTheFullSuite_NotAFixedCap()
    {
        var tests = new List<TestNode> { new("t1", "t1", "action", TestNodeStates.Discovered) };
        var descriptions = new Dictionary<string, MtpTestDescription>();
        var description = new MtpTestDescription(tests[0]);
        description.RegisterInitialTestResult(new MtpTestResult(TimeSpan.FromMinutes(10)));
        descriptions["t1"] = description;
        using var runner = new MicrosoftTestingPlatformRunner(0, new Dictionary<string, List<TestNode>>(), descriptions, new TestSet(), new object(), NullLogger.Instance);
        var calculator = new Mock<ITimeoutValueCalculator>();
        calculator.Setup(c => c.CalculateTimeoutValue(It.IsAny<int>())).Returns<int>(estimate => estimate * 2);

        var timeout = runner.CalculateWarmUpTimeout(tests, calculator.Object, Assembly);

        timeout.ShouldBe(TimeSpan.FromMinutes(20));
    }

    [TestMethod]
    public void CalculateWarmUpTimeout_HasNoCap_WhenThereIsNoCalculatorOrNoDiscoveredTests()
    {
        using var runner = new WarmUpRunner(timedOut: false);
        var calculator = new Mock<ITimeoutValueCalculator>();

        runner.CalculateWarmUpTimeout([], null, Assembly).ShouldBeNull();
        runner.CalculateWarmUpTimeout(null, calculator.Object, Assembly).ShouldBeNull();
    }

    [TestMethod]
    [DataRow(false, false, false)]
    [DataRow(true, false, true)]
    [DataRow(false, true, true)]
    [DataRow(true, true, true)]
    public void RequiresFreshHost_IsTrueWhenAnyMutantSitsInStaticCode(bool first, bool second, bool expected)
    {
        var mutants = new[] { StaticMutant(first), StaticMutant(second) };

        MicrosoftTestingPlatformRunner.RequiresFreshHost(mutants).ShouldBe(expected);
    }

    [TestMethod]
    public void RequiresFreshHost_IsFalseWithoutMutants()
    {
        MicrosoftTestingPlatformRunner.RequiresFreshHost(null).ShouldBeFalse();
        MicrosoftTestingPlatformRunner.RequiresFreshHost([]).ShouldBeFalse();
    }

    [TestMethod]
    [DataRow(OptimizationModes.None, false)]
    [DataRow(OptimizationModes.SkipUncoveredMutants, true)]
    [DataRow(OptimizationModes.CoverageBasedTest, true)]
    [DataRow(OptimizationModes.CoverageBasedTest | OptimizationModes.CaptureCoveragePerTest, true)]
    public void WarmUpEnabled_FollowsCoverageModes(OptimizationModes mode, bool expected)
    {
        var options = new Mock<IStrykerOptions>();
        options.SetupGet(o => o.OptimizationMode).Returns(mode);
        using var runner = new WarmUpRunner(timedOut: false, options: options.Object);

        runner.WarmUpEnabled.ShouldBe(expected);
    }

    [TestMethod]
    public void WarmUpEnabled_IsTrueWhenOptionsAreUnknown()
    {
        using var runner = new WarmUpRunner(timedOut: false);

        runner.WarmUpEnabled.ShouldBeTrue();
    }

    [TestMethod, Timeout(30000)]
    public async Task RunAssemblyTestsInternalAsync_RetriesTheWarmUp_ThenReturnsTheCrashSentinel_WhenTheWarmUpNeverSucceeds()
    {
        // The warm-up gate must give up after two attempts and surface the crash sentinel, so the
        // affected mutants are classified RuntimeError instead of being judged on an unwarmed host.
        var testAssembly = typeof(MicrosoftTestingPlatformRunnerWarmUpTests).Assembly.Location;
        using var runner = new WarmUpRunner(timedOut: true) { ActiveMutantId = 7 };

        var (result, timedOut) = await runner.RunAssemblyTestsInternalAsync(testAssembly, testUidFilter: null);

        timedOut.ShouldBeFalse();
        runner.WarmUpCalls.ShouldBe(2, "both attempts must have tried to warm the host up");
        result.FailingTests.IsEveryTest.ShouldBeTrue("the run must return the crash sentinel, not a judged result");
    }

    private static IMutant StaticMutant(bool isStatic)
    {
        var mutant = new Mock<IMutant>();
        mutant.SetupGet(m => m.IsStaticValue).Returns(isStatic);
        return mutant.Object;
    }

    [TestMethod]
    public async Task RecycleIfHostPoisonedAsync_RecyclesOnATypeInitializerFailure()
    {
        using var runner = new WarmUpRunner(timedOut: false);

        var recycled = await runner.RecycleIfHostPoisonedAsync(Assembly,
            [Update(TestNodeStates.Passed), Update(TestNodeStates.Error, "System.TypeInitializationException : boom")]);

        recycled.ShouldBeTrue();
    }

    [TestMethod]
    public async Task RecycleIfHostPoisonedAsync_KeepsTheHostOnAPlainAssertionFailure()
    {
        using var runner = new WarmUpRunner(timedOut: false);

        var recycled = await runner.RecycleIfHostPoisonedAsync(Assembly,
            [Update(TestNodeStates.Failed, "Assert.Equal() Failure", "at Tests.Foo()")]);

        recycled.ShouldBeFalse();
    }

    [TestMethod]
    public async Task RecycleIfHostPoisonedAsync_KeepsTheHostWhenThereAreNoResults()
    {
        using var runner = new WarmUpRunner(timedOut: false);

        var recycled = await runner.RecycleIfHostPoisonedAsync(Assembly, []);

        recycled.ShouldBeFalse();
    }

    [TestMethod, Timeout(2000)]
    public async Task RunAllTestsAsync_ExposesTheMutantIdDuringTheSession_AndClearsItAfterwards()
    {
        using var runner = new SessionRunner();

        await runner.RunAllTestsAsync([Assembly], mutantId: 17, mutants: null, update: null);

        runner.IdSeenDuringSession.ShouldBe(17);
        runner.ActiveMutantId.ShouldBe(-1);
    }

    [TestMethod, Timeout(2000)]
    public async Task RunAllTestsAsync_ReleasesTheSession_WhenASessionThrows()
    {
        using var runner = new SessionRunner { FailFirstSession = true };

        var failed = await runner.RunAllTestsAsync([Assembly], mutantId: 1, mutants: null, update: null);
        var next = await runner.RunAllTestsAsync([Assembly], mutantId: 2, mutants: null, update: null);

        failed.ShouldNotBeNull();
        next.ShouldNotBeNull();
        runner.Sessions.ShouldBe(2);
        runner.ActiveMutantId.ShouldBe(-1);
    }

    private static int ReadMutantId(MicrosoftTestingPlatformRunner runner)
    {
        using var stream = new FileStream(runner.MutantFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var bytes = new byte[sizeof(int)];
        stream.ReadExactly(bytes);
        return BitConverter.ToInt32(bytes);
    }

    private sealed class WarmUpRunner : MicrosoftTestingPlatformRunner
    {
        private readonly bool _timedOut;
        private readonly Exception? _failure;
        private readonly TestNodeUpdate[]? _warmUpUpdates;

        public WarmUpRunner(bool timedOut, Exception? failure = null, IStrykerOptions? options = null, TestNodeUpdate[]? warmUpUpdates = null)
            : base(0, new Dictionary<string, List<TestNode>>(), new Dictionary<string, MtpTestDescription>(), new TestSet(), new object(), NullLogger.Instance, options)
        {
            _timedOut = timedOut;
            _failure = failure;
            _warmUpUpdates = warmUpUpdates;
        }

        public int WarmUpCalls { get; private set; }

        public int IdSeenDuringWarmUp { get; private set; }

        public string[] TestsSeen { get; private set; } = [];

        public TimeSpan? TimeoutSeen { get; private set; }

        internal override Task<(List<TestNodeUpdate> Updates, bool TimedOut)> RunWarmUpTestsAsync(AssemblyTestServer server, TestNode[]? tests, TimeSpan? timeout)
        {
            WarmUpCalls++;
            TimeoutSeen = timeout;
            IdSeenDuringWarmUp = ReadMutantId(this);
            TestsSeen = tests?.Select(test => test.Uid).ToArray() ?? [];

            if (_failure is not null)
            {
                return Task.FromException<(List<TestNodeUpdate> Updates, bool TimedOut)>(_failure);
            }

            List<TestNodeUpdate> updates = _warmUpUpdates is null ? [] : [.. _warmUpUpdates];
            return Task.FromResult((updates, _timedOut));
        }
    }

    private sealed class SessionRunner : MicrosoftTestingPlatformRunner
    {
        public SessionRunner()
            : base(0, new Dictionary<string, List<TestNode>>(), new Dictionary<string, MtpTestDescription>(), new TestSet(), new object(), NullLogger.Instance)
        {
        }

        public bool FailFirstSession { get; init; }

        public int Sessions { get; private set; }

        public int IdSeenDuringSession { get; private set; }

        internal override Task<(TestRunResult? Result, bool TimedOut, List<TestNode>? DiscoveredTests)> RunAssemblyTestsAsync(
            string assembly,
            ITimeoutValueCalculator? timeoutCalc,
            IReadOnlyList<IMutant>? mutants = null,
            Func<TestNode, bool>? testUidFilter = null)
        {
            Sessions++;
            IdSeenDuringSession = ActiveMutantId;

            if (FailFirstSession && Sessions == 1)
            {
                throw new InvalidOperationException("session failed");
            }

            return Task.FromResult<(TestRunResult?, bool, List<TestNode>?)>((null, false, null));
        }
    }
}
