using System.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.Tests;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
[DoNotParallelize]
[TestCategory("MtpCancellationAcceptance")]
public sealed class MtpCancellationAcceptanceTests(TestContext context)
{
    [TestMethod]
    [DataRow("cooperative")]
    [DataRow("noncooperative")]
    public async Task RealHost_UserCancellation_ConfirmsExitBeforeStartingAnotherRun(string scenario)
    {
        var assembly = RequireFixture();
        var previousScenario = Environment.GetEnvironmentVariable("STRYKER_CANCELLATION_SCENARIO");
        Environment.SetEnvironmentVariable("STRYKER_CANCELLATION_SCENARIO", scenario);
        try
        {
            using var server = CreateServer(assembly);
            (await server.StartAsync(context.CancellationToken)).ShouldBeTrue();
            var tests = await server.DiscoverTestsAsync();
            var slow = tests.Single(test => test.DisplayName.Contains("E_SlowSelectedTest", StringComparison.Ordinal));
            var fast = tests.Single(test => test.DisplayName.Contains("A_KillingFailure", StringComparison.Ordinal));
            using var oldProcess = Process.GetProcessById(server.ProcessId!.Value);
            using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);
            var observedRunning = false;

            await Assert.ThrowsAsync<OperationCanceledException>(() => server.RunTestsAsync(
                [slow], TimeSpan.FromSeconds(15), updates =>
                {
                    if (updates.Any(update => update.Node.ExecutionState == TestNodeStates.InProgress))
                    {
                        observedRunning = true;
                        cancellation.Cancel();
                    }
                    return false;
                }, cancellation.Token));

            observedRunning.ShouldBeTrue();
            oldProcess.HasExited.ShouldBeTrue();
            server.IsAlive.ShouldBeFalse();
            (await server.StartAsync(context.CancellationToken)).ShouldBeTrue();
            server.ProcessId.ShouldNotBe(oldProcess.Id);
            var results = await server.RunTestsAsync([fast]);
            results.Single().Node.ExecutionState.ShouldBe(TestNodeStates.Passed);
        }
        finally
        {
            Environment.SetEnvironmentVariable("STRYKER_CANCELLATION_SCENARIO", previousScenario);
        }
    }

    [TestMethod]
    public async Task RealHost_Timeout_PreservesPartialExecutionAndDiscardsHost()
    {
        using var server = CreateServer(RequireFixture());
        (await server.StartAsync(context.CancellationToken)).ShouldBeTrue();
        var tests = await server.DiscoverTestsAsync();
        var slow = tests.Single(test => test.DisplayName.Contains("E_SlowSelectedTest", StringComparison.Ordinal));
        using var process = Process.GetProcessById(server.ProcessId!.Value);

        var (results, timedOut) = await server.RunTestsAsync([slow], TimeSpan.FromMilliseconds(200),
            cancellationToken: context.CancellationToken);

        timedOut.ShouldBeTrue();
        results.Where(update => TestNodeStates.IsFinished(update.Node.ExecutionState)).ShouldBeEmpty();
        process.HasExited.ShouldBeTrue();
        server.IsAlive.ShouldBeFalse();
    }

    [TestMethod]
    public async Task RealHost_RetryFailureThenPass_ReusesHostWithoutLeakingSupersededFailure()
    {
        using var server = CreateServer(RequireFixture());
        (await server.StartAsync(context.CancellationToken)).ShouldBeTrue();
        var tests = await server.DiscoverTestsAsync();
        var retried = tests.Single(test => test.DisplayName.Contains("C_RetryFailureThenPass", StringComparison.Ordinal));
        var processId = server.ProcessId;

        for (var run = 0; run < 2; run++)
        {
            var (results, timedOut) = await server.RunTestsAsync([retried], TimeSpan.FromSeconds(15),
                cancellationToken: context.CancellationToken);
            timedOut.ShouldBeFalse();
            results.Single().Node.ExecutionState.ShouldBe(TestNodeStates.Passed);
            results.Single().Node.RetryAttempt.ShouldBe(2);
            results.Single().Node.RetryIsSuperseded.ShouldBe(false);
            server.ProcessId.ShouldBe(processId);
            server.IsAlive.ShouldBeTrue();
        }
    }

    [TestMethod]
    public async Task RealHost_ZeroSelectedResults_IsRuntimeErrorNotSurvival()
    {
        var assembly = RequireFixture();
        var testsByAssembly = new Dictionary<string, List<TestNode>>();
        using var runner = new MicrosoftTestingPlatformRunner(0, testsByAssembly, new(), new TestSet(), new object(), NullLogger.Instance);
        (await runner.DiscoverTestsAsync(assembly)).ShouldBeTrue();
        testsByAssembly[assembly] = [new TestNode("uid-that-does-not-exist", "Missing", "action", TestNodeStates.Discovered)];
        var mutant = new Mock<IMutant>();
        mutant.SetupGet(value => value.AssessingTests).Returns(TestIdentifierList.EveryTest());

        var result = await runner.RunAllTestsAsync([assembly], 0, [mutant.Object], (_, _, _, _) => true,
            cancellationToken: context.CancellationToken);

        result.SessionHadRuntimeIssue.ShouldBeTrue();
        result.ExecutedTests.IsEmpty.ShouldBeTrue();
        result.ExecutedTests.IsEveryTest.ShouldBeFalse();
        result.FailingTests.IsEmpty.ShouldBeTrue();
    }

    [TestMethod]
    public async Task SourceClient_CancelledWait_DoesNotProveServerCleanupHasFinished()
    {
        var assembly = RequireFixture();
        var previousScenario = Environment.GetEnvironmentVariable("STRYKER_CANCELLATION_SCENARIO");
        var previousEvidence = Environment.GetEnvironmentVariable("STRYKER_CANCELLATION_EVIDENCE");
        var evidence = Path.Combine(previousEvidence ?? Path.GetTempPath(), $"source-cancel-proof-{Guid.NewGuid():N}");
        Directory.CreateDirectory(evidence);
        Environment.SetEnvironmentVariable("STRYKER_CANCELLATION_SCENARIO", "held-cleanup");
        Environment.SetEnvironmentVariable("STRYKER_CANCELLATION_EVIDENCE", evidence);
        var factory = new DefaultTestServerConnectionFactory();
        var (listener, port) = factory.CreateListener();
        using var ownedListener = listener;
        using var process = factory.StartProcess(assembly, port, new());
        try
        {
            using var connection = await listener.AcceptConnectionAsync(context.CancellationToken).WaitAsync(TimeSpan.FromSeconds(15));
            using var client = factory.CreateClient(connection, process.ProcessHandle, NullLogger.Instance);
            await client.InitializeAsync(context.CancellationToken);
            var discovered = new List<TestNode>();
            await client.DiscoverTestsAsync(updates =>
            {
                discovered.AddRange(updates.Select(update => update.Node));
                return Task.CompletedTask;
            }, context.CancellationToken);
            var slow = discovered.Single(test => test.DisplayName.Contains("E_SlowSelectedTest", StringComparison.Ordinal));
            var pid = process.ProcessHandle.Id;
            using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);
            var run = client.RunTestsAsync(_ => Task.CompletedTask, [slow], cancellation.Token);
            await WaitForMarkerAsync(Path.Combine(evidence, $"{pid}.slow-start"));

            cancellation.Cancel();
            await Assert.ThrowsAsync<OperationCanceledException>(() => run);
            await WaitForMarkerAsync(Path.Combine(evidence, $"{pid}.cleanup-start"));

            process.HasExited.ShouldBeFalse();
            File.Exists(Path.Combine(evidence, $"{pid}.cleanup-finished")).ShouldBeFalse();
            context.AddResultFile(Path.Combine(evidence, $"{pid}.events"));
        }
        finally
        {
            try
            {
                process.ProcessHandle.Kill();
                try
                {
                    await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(5));
                }
                catch (Exception) when (process.HasExited)
                {
                }
                process.HasExited.ShouldBeTrue();
            }
            finally
            {
                Environment.SetEnvironmentVariable("STRYKER_CANCELLATION_SCENARIO", previousScenario);
                Environment.SetEnvironmentVariable("STRYKER_CANCELLATION_EVIDENCE", previousEvidence);
            }
        }
    }

    [TestMethod]
    public async Task RealPool_PublicCallerCancellation_DiscardsHostBeforeNextMutant()
    {
        var assembly = RequireFixture();
        var options = new Mock<IStrykerOptions>();
        options.SetupGet(value => value.Concurrency).Returns(1);
        options.SetupGet(value => value.LogOptions).Returns(Mock.Of<ILogOptions>());
        using var pool = new MicrosoftTestPlatformRunnerPool(options.Object, NullLogger.Instance);
        (await pool.DiscoverTestsAsync(assembly, context.CancellationToken)).ShouldBeTrue();
        var runner = pool.Runners.Single();
        var slow = runner.GetDiscoveredTests(assembly)!.Single(test => test.DisplayName.Contains("E_SlowSelectedTest", StringComparison.Ordinal));
        using var oldProcess = Process.GetProcessById(runner._assemblyServers[assembly].ProcessId!.Value);
        var project = new Mock<IProjectAndTests>();
        project.Setup(value => value.GetTestAssemblies()).Returns([assembly]);
        var mutant = new Mock<IMutant>();
        mutant.SetupGet(value => value.Id).Returns(0);
        mutant.SetupGet(value => value.AssessingTests).Returns(new TestIdentifierList(slow.Uid));
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);
        cancellation.CancelAfter(TimeSpan.FromMilliseconds(300));

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            pool.TestMultipleMutantsAsync(project.Object, null, [mutant.Object], null, cancellation.Token));

        oldProcess.HasExited.ShouldBeTrue();
        pool.Runners.Single().ShouldBeSameAs(runner);
        runner._assemblyServers.ShouldBeEmpty();
        mutant.SetupGet(value => value.Id).Returns(1);
        var result = await pool.TestMultipleMutantsAsync(project.Object, null, [mutant.Object], null, context.CancellationToken);

        result.SessionTimedOut.ShouldBeFalse();
        result.SessionHadRuntimeIssue.ShouldBeFalse();
        result.FailingTests.IsEmpty.ShouldBeTrue();
        result.ExecutedTests.GetIdentifiers().ShouldBe([slow.Uid]);
        runner._assemblyServers[assembly].ProcessId.ShouldNotBe(oldProcess.Id);
    }

    private async Task WaitForMarkerAsync(string path)
    {
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);
        deadline.CancelAfter(TimeSpan.FromSeconds(10));
        while (!File.Exists(path))
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10), deadline.Token);
        }
    }

    private static AssemblyTestServer CreateServer(string assembly) =>
        new(assembly, new(), NullLogger.Instance, "acceptance");

    private static string RequireFixture()
    {
        var assembly = Environment.GetEnvironmentVariable("STRYKER_MTP_CANCELLATION_FIXTURE");
        if (assembly is null)
        {
            Assert.Inconclusive("Run the cancellation fixture validation script to enable real-host acceptance tests.");
        }
        File.Exists(assembly).ShouldBeTrue();
        return assembly;
    }
}
