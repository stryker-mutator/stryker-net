using Microsoft.Extensions.Logging.Abstractions;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
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
public sealed class MtpBailPolicyTests
{
    [TestMethod]
    public async Task RunAllTestsAsync_ExplicitFinalFailure_BailsDuringAssemblyAndSkipsLaterAssemblies()
    {
        using var runner = new PolicyRunner(
            [Node("failed", TestNodeStates.Failed, false), Node("unfinished", TestNodeStates.InProgress)],
            [Node("later", TestNodeStates.Passed)]);
        var mutant = Mutant();
        var callbacks = 0;

        var result = await runner.RunAllTestsAsync(["first", "second"], 1, [mutant], (_, failures, _, _) =>
        {
            callbacks++;
            return failures.IsEmpty;
        });

        runner.StreamBailed.ShouldBeTrue();
        runner.VisitedAssemblies.ShouldBe(["first"]);
        result.SessionTimedOut.ShouldBeFalse();
        result.ExecutedTests.GetIdentifiers().ShouldBe(["failed"]);
        result.FailingTests.GetIdentifiers().ShouldBe(["failed"]);
        callbacks.ShouldBeGreaterThan(0);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow(true)]
    public async Task RunAllTestsAsync_UnannotatedOrSupersededFailure_DoesNotBailDuringAssembly(bool? superseded)
    {
        using var runner = new PolicyRunner(
            [Node("retried", TestNodeStates.Failed, superseded), Node("retried", TestNodeStates.Passed, false)],
            [Node("later", TestNodeStates.Passed)]);
        var callbackFailures = new List<string>();

        var result = await runner.RunAllTestsAsync(["first", "second"], 1, [Mutant()], (_, failures, _, _) =>
        {
            callbackFailures.AddRange(failures.GetIdentifiers());
            return true;
        });

        runner.StreamBailed.ShouldBeFalse();
        runner.VisitedAssemblies.ShouldBe(["first", "second"]);
        result.FailingTests.IsEmpty.ShouldBeTrue();
        callbackFailures.ShouldBeEmpty();
    }

    [TestMethod]
    public async Task RunAllTestsAsync_BaselineFailure_UsesConsumerPolicyAndContinuesUntilNewFailure()
    {
        using var runner = new PolicyRunner(
            [Node("baseline", TestNodeStates.Failed, false), Node("new-failure", TestNodeStates.Failed, false)],
            [Node("later", TestNodeStates.Passed)]);
        var policyCalls = new List<string[]>();

        var result = await runner.RunAllTestsAsync(["first", "second"], 1, [Mutant()], (_, failures, _, _) =>
        {
            var newFailures = failures.GetIdentifiers().Where(id => id != "baseline").ToArray();
            policyCalls.Add(newFailures);
            return newFailures.Length == 0;
        });

        policyCalls[0].ShouldBeEmpty();
        policyCalls[1].ShouldBe(["new-failure"]);
        runner.StreamBailed.ShouldBeTrue();
        result.FailingTests.GetIdentifiers().ShouldContain("new-failure");
    }

    [TestMethod]
    public async Task RunAllTestsAsync_DisableBail_RunsEveryAssemblyEvenWhenPolicyReturnsFalse()
    {
        using var runner = new PolicyRunner(
            [Node("failure", TestNodeStates.Failed, false)],
            [Node("later", TestNodeStates.Passed)], disableBail: true);

        var result = await runner.RunAllTestsAsync(["first", "second"], 1, [Mutant()], (_, _, _, _) => false);

        runner.StreamBailed.ShouldBeFalse();
        runner.VisitedAssemblies.ShouldBe(["first", "second"]);
        result.ExecutedTests.IsEveryTest.ShouldBeTrue();
    }

    [TestMethod]
    public async Task RunAllTestsAsync_GlobalEveryTest_RequiresAllAssembliesAndExactIdentities()
    {
        using var runner = new PolicyRunner(
            [Node("first-test", TestNodeStates.Passed), Node("first-test", TestNodeStates.Passed)],
            [Node("second-test", TestNodeStates.Passed)]);
        var globalSentinels = new List<bool>();

        var result = await runner.RunAllTestsAsync(["first", "second"], 1, [Mutant()], (_, _, executed, _) =>
        {
            globalSentinels.Add(executed.IsEveryTest);
            return true;
        });

        globalSentinels.ShouldBe([false, false, true]);
        result.ExecutedTests.IsEveryTest.ShouldBeTrue();
    }

    [TestMethod]
    public async Task RunAllTestsAsync_IncompleteRun_NeverExpandsDuplicateUpdatesToEveryTest()
    {
        using var runner = new PolicyRunner(
            [Node("first-test", TestNodeStates.Passed), Node("first-test", TestNodeStates.Passed)],
            [Node("unfinished", TestNodeStates.InProgress)]);

        var result = await runner.RunAllTestsAsync(["first", "second"], 1, [Mutant()], (_, _, _, _) => true);

        result.ExecutedTests.IsEveryTest.ShouldBeFalse();
        result.ExecutedTests.GetIdentifiers().ShouldBe(["first-test"]);
    }

    [TestMethod]
    public async Task RunAllTestsAsync_BaselineFailureAndRuntimeIssue_DoesNotReturnKillEvidence()
    {
        using var runner = new PolicyRunner([Node("baseline", TestNodeStates.Failed)], []);
        runner.Descriptions["baseline"].InitiallyFailed = true;
        runner.RuntimeIssue = true;

        var result = await runner.RunAllTestsAsync(["first"], 1, [Mutant()], (_, _, _, _) => true);

        result.SessionHadRuntimeIssue.ShouldBeTrue();
        result.ExecutedTests.GetIdentifiers().ShouldBe(["baseline"]);
        result.FailingTests.IsEmpty.ShouldBeTrue();
    }

    [TestMethod]
    public async Task RunAllTestsAsync_ConcurrentCalls_CannotSwitchMutantBeforePreviousRunReturns()
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var runner = new PolicyRunner([Node("test", TestNodeStates.Passed)], []);
        runner.BeforeTerminal = async () =>
        {
            entered.TrySetResult();
            await release.Task;
        };
        var first = runner.RunAllTestsAsync(["first"], 1, null, null);
        await entered.Task;
        var second = runner.RunAllTestsAsync(["first"], 2, null, null);

        second.IsCompleted.ShouldBeFalse();
        BitConverter.ToInt32(File.ReadAllBytes(runner.MutantFilePath)).ShouldBe(1);
        release.SetResult();
        await Task.WhenAll(first, second);
        BitConverter.ToInt32(File.ReadAllBytes(runner.MutantFilePath)).ShouldBe(2);
    }

    [TestMethod]
    public async Task RunAllTestsAsync_UnverifiedHostExit_AbortsBeforeAnyFurtherMutantWrite()
    {
        var factory = new Mock<ITestServerConnectionFactory>();
        var listener = new Mock<ITestServerListener>();
        var process = new Mock<ITestServerProcess>();
        var handle = new Mock<IProcessHandle>();
        var client = new Mock<ITestingPlatformClient>();
        var exited = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var hasExited = false;
        process.SetupGet(value => value.HasExited).Returns(() => hasExited);
        process.SetupGet(value => value.ProcessHandle).Returns(handle.Object);
        process.Setup(value => value.WaitForExitAsync()).Returns(exited.Task);
        listener.Setup(value => value.AcceptConnectionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new TcpClient());
        factory.Setup(value => value.CreateListener()).Returns((listener.Object, 123));
        factory.Setup(value => value.StartProcess(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Dictionary<string, string?>>())).Returns(process.Object);
        factory.Setup(value => value.CreateClient(It.IsAny<TcpClient>(), It.IsAny<IProcessHandle>(), It.IsAny<ILogger>())).Returns(client.Object);
        client.Setup(value => value.InitializeAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        using var server = new AssemblyTestServer("first", new(), NullLogger.Instance, "stuck",
            connectionFactory: factory.Object, shutdownTimeout: TimeSpan.FromMilliseconds(20));
        using var runner = new PolicyRunner([Node("test", TestNodeStates.Passed)], []);
        (await server.StartAsync()).ShouldBeTrue();
        try
        {
            await Assert.ThrowsExactlyAsync<TestHostTerminationException>(() => server.StopAsync(force: true));
            runner._assemblyServers["first"] = server;
            await Assert.ThrowsExactlyAsync<TestHostTerminationException>(() => runner.RunAllTestsAsync(["first"], 1, null, null));
            await Assert.ThrowsExactlyAsync<TestHostTerminationException>(() => runner.RunAllTestsAsync(["first"], 2, null, null));
            BitConverter.ToInt32(File.ReadAllBytes(runner.MutantFilePath)).ShouldBe(-1);
            client.Verify(value => value.Dispose(), Times.Once);
            process.Verify(value => value.Dispose(), Times.Never);
        }
        finally
        {
            hasExited = true;
            exited.SetResult();
        }
    }

    [TestMethod]
    public async Task ResetServerAsync_WaitsForActiveRunAndSerializesConcurrentResets()
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var runner = new PolicyRunner([Node("test", TestNodeStates.Passed)], []);
        runner.BeforeTerminal = async () =>
        {
            entered.SetResult();
            await release.Task;
        };
        var run = runner.RunAllTestsAsync(["first"], 1, null, null);
        await entered.Task;
        var firstReset = runner.ResetServerAsync();
        var secondReset = runner.ResetServerAsync();

        firstReset.IsCompleted.ShouldBeFalse();
        secondReset.IsCompleted.ShouldBeFalse();
        release.SetResult();
        await Task.WhenAll(run, firstReset, secondReset);
        BitConverter.ToInt32(File.ReadAllBytes(runner.MutantFilePath)).ShouldBe(1);
    }

    [TestMethod]
    public async Task Pool_CancellingQueuedCaller_DoesNotCancelOrSwitchTheActiveMutant()
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var runner = new PolicyRunner([Node("test", TestNodeStates.Passed)], []);
        runner.BeforeTerminal = async () =>
        {
            entered.SetResult();
            await release.Task;
        };
        var options = new Mock<IStrykerOptions>();
        options.SetupGet(value => value.Concurrency).Returns(1);
        var factory = new Mock<ISingleRunnerFactory>();
        factory.Setup(value => value.CreateRunner(It.IsAny<int>(), It.IsAny<Dictionary<string, List<TestNode>>>(),
            It.IsAny<Dictionary<string, MtpTestDescription>>(), It.IsAny<TestSet>(), It.IsAny<object>(),
            It.IsAny<ILogger>(), It.IsAny<IStrykerOptions?>())).Returns(runner);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance, factory.Object);
        var project = new Mock<IProjectAndTests>();
        project.Setup(value => value.GetTestAssemblies()).Returns(["first"]);
        var firstMutant = new Mock<IMutant>();
        firstMutant.SetupGet(value => value.Id).Returns(1);
        firstMutant.SetupGet(value => value.AssessingTests).Returns(TestIdentifierList.EveryTest());
        var first = pool.TestMultipleMutantsAsync(project.Object, null, [firstMutant.Object], null);
        await entered.Task;
        using var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
        try
        {
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                pool.TestMultipleMutantsAsync(project.Object, null, [Mutant()], null, cancellation.Token));
            first.IsCompleted.ShouldBeFalse();
            BitConverter.ToInt32(File.ReadAllBytes(runner.MutantFilePath)).ShouldBe(1);
        }
        finally
        {
            release.TrySetResult();
            await first;
        }
    }

    [TestMethod]
    public async Task Pool_ResetIncludesLeasedRunnerAndWaitsForItsActiveRun()
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var resetStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var logger = new Mock<ILogger>();
        logger.Setup(value => value.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception?>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()))
            .Callback(new InvocationAction(invocation =>
            {
                if (invocation.Arguments[2]?.ToString()?.Contains("Resetting test servers to reload assemblies", StringComparison.Ordinal) == true)
                {
                    resetStarted.TrySetResult();
                }
            }));
        var runner = new PolicyRunner([Node("test", TestNodeStates.Passed)], [], logger: logger.Object);
        runner.BeforeTerminal = async () =>
        {
            entered.SetResult();
            await release.Task;
        };
        var options = new Mock<IStrykerOptions>();
        options.SetupGet(value => value.Concurrency).Returns(1);
        var factory = new Mock<ISingleRunnerFactory>();
        factory.Setup(value => value.CreateRunner(It.IsAny<int>(), It.IsAny<Dictionary<string, List<TestNode>>>(),
            It.IsAny<Dictionary<string, MtpTestDescription>>(), It.IsAny<TestSet>(), It.IsAny<object>(),
            It.IsAny<ILogger>(), It.IsAny<IStrykerOptions?>())).Returns(runner);
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance, factory.Object);
        var project = new Mock<IProjectAndTests>();
        project.Setup(value => value.GetTestAssemblies()).Returns(["first"]);
        var run = pool.TestMultipleMutantsAsync(project.Object, null, [Mutant()], null);
        await entered.Task;
        pool.Runners.ShouldBeEmpty();
        var reset = Task.Run(pool.ResetTestProcesses);
        try
        {
            (await Task.WhenAny(resetStarted.Task, reset)).ShouldBeSameAs(resetStarted.Task);
            reset.IsCompleted.ShouldBeFalse();
        }
        finally
        {
            release.TrySetResult();
            await Task.WhenAll(run, reset);
        }
        pool.Runners.Single().ShouldBeSameAs(runner);
    }

    private static IMutant Mutant()
    {
        var mutant = new Mock<IMutant>();
        mutant.SetupGet(value => value.AssessingTests).Returns(TestIdentifierList.EveryTest());
        return mutant.Object;
    }

    private static TestNodeUpdate Node(string id, string state, bool? superseded = null) =>
        new(new TestNode(id, id, "action", state, RetryIsSuperseded: superseded), "root");

    private sealed class PolicyRunner : MicrosoftTestingPlatformRunner
    {
        private readonly Dictionary<string, TestNodeUpdate[]> _batches;
        public Dictionary<string, MtpTestDescription> Descriptions { get; }
        public List<string> VisitedAssemblies { get; } = [];
        public bool StreamBailed { get; private set; }
        public bool RuntimeIssue { get; set; }
        public Func<Task>? BeforeTerminal { get; set; }

        public PolicyRunner(TestNodeUpdate[] first, TestNodeUpdate[] second, bool disableBail = false, ILogger? logger = null)
            : this(new Dictionary<string, TestNodeUpdate[]> { ["first"] = first, ["second"] = second },
                new Dictionary<string, MtpTestDescription>(), disableBail, logger)
        {
        }

        private PolicyRunner(Dictionary<string, TestNodeUpdate[]> batches,
            Dictionary<string, MtpTestDescription> descriptions, bool disableBail, ILogger? logger)
            : base(0, Discover(batches), descriptions, new TestSet(), new object(), logger ?? NullLogger.Instance, Options(disableBail))
        {
            _batches = batches;
            Descriptions = descriptions;
            foreach (var update in batches.Values.SelectMany(updates => updates))
            {
                descriptions.TryAdd(update.Node.Uid, new MtpTestDescription(update.Node));
            }
        }

        internal override async Task<(TestRunResult? Result, bool TimedOut, List<TestNode>? DiscoveredTests)> RunAssemblyTestsAsync(
            string assembly, ITimeoutValueCalculator? timeoutCalc, IReadOnlyList<IMutant>? mutants = null,
            Func<TestNode, bool>? testUidFilter = null, Func<IReadOnlyCollection<TestNodeUpdate>, bool>? shouldBail = null,
            CancellationToken cancellationToken = default)
        {
            VisitedAssemblies.Add(assembly);
            var received = new List<TestNodeUpdate>();
            foreach (var update in _batches[assembly])
            {
                received.Add(update);
                if (shouldBail?.Invoke(received) == true)
                {
                    StreamBailed = true;
                    break;
                }
            }
            if (BeforeTerminal is not null)
            {
                await BeforeTerminal();
            }
            var result = BuildTestRunResult(received, 0, TimeSpan.Zero);
            if (RuntimeIssue)
            {
                result = TestRunResult.RuntimeError(result.TestDescriptions, result.ExecutedTests, result.FailingTests,
                    result.TimedOutTests, "partial runtime issue", result.Messages, result.Duration);
            }
            return (result, false, GetDiscoveredTests(assembly));
        }

        private static Dictionary<string, List<TestNode>> Discover(Dictionary<string, TestNodeUpdate[]> batches) =>
            batches.ToDictionary(pair => pair.Key,
                pair => pair.Value.Select(update => update.Node with { ExecutionState = TestNodeStates.Discovered })
                    .DistinctBy(node => node.Uid).ToList());

        private static IStrykerOptions Options(bool disableBail)
        {
            var options = new Mock<IStrykerOptions>();
            options.SetupGet(value => value.OptimizationMode).Returns(disableBail ? OptimizationModes.DisableBail : OptimizationModes.None);
            return options.Object;
        }
    }
}
