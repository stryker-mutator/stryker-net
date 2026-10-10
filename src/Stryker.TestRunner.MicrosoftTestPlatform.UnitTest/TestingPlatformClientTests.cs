using Microsoft.Extensions.Logging;
using Microsoft.Testing.Platform.ServerMode.Client;
using Moq;
using Shouldly;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class TestingPlatformClientTests
{
    private readonly Mock<IMtpServerClient> _mtpClient = new();
    private readonly Mock<IProcessHandle> _processHandle = new();
    private readonly Mock<ILogger> _logger = new();

    private TestingPlatformClient CreateClient()
        => new(_mtpClient.Object, _processHandle.Object, _logger.Object);

    [TestMethod]
    public async Task InitializeAsync_ForwardsToSourceClient()
    {
        _mtpClient.Setup(client => client.InitializeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateCapabilities());

        using var client = CreateClient();
        await client.InitializeAsync();

        _mtpClient.Verify(sourceClient => sourceClient.InitializeAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task DiscoverTestsAsync_MapsSourceClientUpdates()
    {
        var sourceUpdate = CreateUpdate(
            uid: "test-1",
            displayName: "My test",
            executionState: TestNodeStates.Discovered,
            filePath: "Tests.cs",
            lineStart: 42,
            lineEnd: 44,
            typeName: "Tests",
            methodName: "MyTest");

        _mtpClient.Setup(client => client.DiscoverTestsAsync(It.IsAny<CancellationToken>()))
            .Callback(() => RaiseUpdates(sourceUpdate))
            .Returns(Task.CompletedTask);

        TestNodeUpdate[]? updates = null;
        using var client = CreateClient();
        await client.DiscoverTestsAsync(received =>
        {
            updates = received;
            return Task.CompletedTask;
        });

        updates.ShouldNotBeNull();
        updates.Length.ShouldBe(1);
        updates[0].ParentUid.ShouldBe("parent");
        updates[0].Node.ShouldBe(new TestNode(
            "test-1",
            "My test",
            "action",
            TestNodeStates.Discovered,
            "Tests.cs",
            42,
            44,
            "Tests",
            "MyTest"));
    }

    [TestMethod]
    public async Task DiscoverTestsAsync_AwaitsUpdateCallback()
    {
        var callbackCompletion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _mtpClient.Setup(client => client.DiscoverTestsAsync(It.IsAny<CancellationToken>()))
            .Callback(() => RaiseUpdates(CreateUpdate()))
            .Returns(Task.CompletedTask);

        using var client = CreateClient();
        var discovery = client.DiscoverTestsAsync(_ => callbackCompletion.Task);

        discovery.IsCompleted.ShouldBeFalse();
        callbackCompletion.SetResult();
        await discovery;
    }

    [TestMethod]
    public async Task RunTestsAsync_WithoutSelection_RunsAllTests()
    {
        _mtpClient.Setup(client => client.RunTestsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MtpRunResult([]));

        using var client = CreateClient();
        await client.RunTestsAsync(_ => Task.CompletedTask);

        _mtpClient.Verify(sourceClient => sourceClient.RunTestsAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mtpClient.Verify(
            sourceClient => sourceClient.RunTestsAsync(It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task RunTestsAsync_WithSelection_SendsOnlyTestUids()
    {
        var tests = new[]
        {
            new TestNode("test-1", "Test 1", "action", TestNodeStates.Discovered),
            new TestNode("test-2", "Test 2", "action", TestNodeStates.Discovered)
        };
        _mtpClient.Setup(client => client.RunTestsAsync(It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MtpRunResult([]));

        using var client = CreateClient();
        await client.RunTestsAsync(_ => Task.CompletedTask, tests);

        _mtpClient.Verify(
            sourceClient => sourceClient.RunTestsAsync(
                It.Is<IReadOnlyCollection<string>>(uids => uids.SequenceEqual(new[] { "test-1", "test-2" })),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    public async Task RunTestsAsync_ForwardsCancellation()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        _mtpClient.Setup(client => client.RunTestsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MtpRunResult([]));

        using var client = CreateClient();
        await client.RunTestsAsync(_ => Task.CompletedTask, cancellationToken: cancellationTokenSource.Token);

        _mtpClient.Verify(
            sourceClient => sourceClient.RunTestsAsync(cancellationTokenSource.Token),
            Times.Once);
    }

    [TestMethod, Timeout(5000)]
    public async Task DiscoverTestsAsync_WaitsForASlowAsyncCallback_BeforeReturning()
    {
        _mtpClient.Setup(client => client.DiscoverTestsAsync(It.IsAny<CancellationToken>()))
            .Callback(() => RaiseUpdates(CreateUpdate(executionState: TestNodeStates.Discovered)))
            .Returns(Task.CompletedTask);

        var callbackFinished = false;
        using var client = CreateClient();

        await client.DiscoverTestsAsync(async _ =>
        {
            await Task.Delay(50);
            callbackFinished = true;
        });

        callbackFinished.ShouldBeTrue();
    }
    [TestMethod]
    public async Task RunTestsAsync_MapsErrorDetailsOfAFailedTest()
    {
        _mtpClient.Setup(client => client.RunTestsAsync(It.IsAny<CancellationToken>()))
            .Callback(() => RaiseUpdates(CreateUpdate(
                executionState: TestNodeStates.Error,
                errorMessage: "System.TypeInitializationException : boom",
                errorStackTrace: "   at X..cctor()")))
            .ReturnsAsync(new MtpRunResult([]));

        TestNodeUpdate[]? updates = null;
        using var client = CreateClient();
        await client.RunTestsAsync(received =>
        {
            updates = received;
            return Task.CompletedTask;
        });

        updates.ShouldNotBeNull();
        updates[0].Node.ErrorMessage.ShouldBe("System.TypeInitializationException : boom");
        updates[0].Node.ErrorStackTrace.ShouldBe("   at X..cctor()");
    }

    [TestMethod]
    public async Task RunTestsAsync_LeavesErrorDetailsNull_WhenTheServerSendsNone()
    {
        _mtpClient.Setup(client => client.RunTestsAsync(It.IsAny<CancellationToken>()))
            .Callback(() => RaiseUpdates(CreateUpdate(executionState: TestNodeStates.Passed)))
            .ReturnsAsync(new MtpRunResult([]));

        TestNodeUpdate[]? updates = null;
        using var client = CreateClient();
        await client.RunTestsAsync(received =>
        {
            updates = received;
            return Task.CompletedTask;
        });

        updates.ShouldNotBeNull();
        updates[0].Node.ErrorMessage.ShouldBeNull();
        updates[0].Node.ErrorStackTrace.ShouldBeNull();
    }

    [TestMethod, Timeout(2000)]
    public async Task RunTestsAsync_ReturnsPromptly_WhenNoUpdateIsReported()
    {
        _mtpClient.Setup(client => client.RunTestsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MtpRunResult([]));

        using var client = CreateClient();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await client.RunTestsAsync(_ => Task.CompletedTask);

        stopwatch.Elapsed.ShouldBeLessThan(TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public async Task RunTestsAsync_PropagatesAFailingCallback()
    {
        _mtpClient.Setup(client => client.RunTestsAsync(It.IsAny<CancellationToken>()))
            .Callback(() => RaiseUpdates(CreateUpdate()))
            .ReturnsAsync(new MtpRunResult([]));

        using var client = CreateClient();

        await Should.ThrowAsync<InvalidOperationException>(
            () => client.RunTestsAsync(_ => Task.FromException(new InvalidOperationException("callback failed"))));
    }

    [TestMethod, Timeout(5000)]
    public async Task AwaitCallbacksAsync_ThrowsAtTheCap_WhenAQueuedCallbackNeverCompletes()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var callbacks = new List<Task> { new TaskCompletionSource().Task };

        await Should.ThrowAsync<TimeoutException>(
            () => TestingPlatformClient.AwaitCallbacksAsync(callbacks, new object(), maxWaitMs: 100));

        stopwatch.Elapsed.ShouldBeGreaterThanOrEqualTo(TimeSpan.FromMilliseconds(90));
        stopwatch.Elapsed.ShouldBeLessThan(TimeSpan.FromSeconds(3));
    }

    [TestMethod, Timeout(5000)]
    public async Task AwaitCallbacksAsync_WarnsWithThePendingCount_WhenTheCapIsReached()
    {
        var logger = new CapturingLogger();
        var callbacks = new List<Task> { new TaskCompletionSource().Task, Task.CompletedTask, new TaskCompletionSource().Task };

        await Should.ThrowAsync<TimeoutException>(
            () => TestingPlatformClient.AwaitCallbacksAsync(callbacks, new object(), maxWaitMs: 50, logger: logger));

        var warning = logger.Entries.ShouldHaveSingleItem();
        warning.Level.ShouldBe(Microsoft.Extensions.Logging.LogLevel.Warning);
        warning.Message.ShouldContain("2 test update callback(s)");
    }

    [TestMethod, Timeout(5000)]
    public async Task AwaitCallbacksAsync_DoesNotWarn_WhenEveryCallbackCompletes()
    {
        var logger = new CapturingLogger();

        await TestingPlatformClient.AwaitCallbacksAsync([Task.CompletedTask], new object(), logger: logger);

        logger.Entries.ShouldBeEmpty();
    }
    [TestMethod, Timeout(5000)]
    public async Task AwaitCallbacksAsync_Throws_WhenAQueuedCallbackFails()
    {
        var callbacks = new List<Task> { Task.FromException(new InvalidOperationException("callback failed")) };

        await Should.ThrowAsync<InvalidOperationException>(
            () => TestingPlatformClient.AwaitCallbacksAsync(callbacks, new object()));
    }

    [TestMethod, Timeout(5000)]
    public async Task AwaitCallbacksAsync_Throws_WhenOneOfSeveralCallbacksFails()
    {
        var callbacks = new List<Task>
        {
            Task.Run(async () => await Task.Delay(30)),
            Task.FromException(new InvalidOperationException("callback failed")),
        };

        await Should.ThrowAsync<InvalidOperationException>(
            () => TestingPlatformClient.AwaitCallbacksAsync(callbacks, new object()));
    }

    [TestMethod, Timeout(5000)]
    public async Task AwaitCallbacksAsync_AwaitsEveryQueuedCallback()
    {
        var completed = 0;
        var callbacks = new List<Task>
        {
            Task.Run(async () => { await Task.Delay(30); Interlocked.Increment(ref completed); }),
            Task.Run(async () => { await Task.Delay(60); Interlocked.Increment(ref completed); }),
        };

        await TestingPlatformClient.AwaitCallbacksAsync(callbacks, new object());

        completed.ShouldBe(2);
    }

    [TestMethod]
    public void AwaitCallbacksAsync_CompletesSynchronously_WhenNothingWasQueued()
    {
        var wait = TestingPlatformClient.AwaitCallbacksAsync([], new object());

        wait.IsCompletedSuccessfully.ShouldBeTrue("no settle delay is needed once the request has completed");
    }

    [TestMethod]
    public async Task AwaitCallbacksAsync_ClearsTheQueue_SoACallbackIsAwaitedOnce()
    {
        var callbacks = new List<Task> { Task.CompletedTask };

        await TestingPlatformClient.AwaitCallbacksAsync(callbacks, new object());

        callbacks.ShouldBeEmpty();
    }

    [TestMethod, Timeout(5000)]
    public async Task RunTestsAsync_WaitsForASlowAsyncCallback_BeforeReturning()
    {
        _mtpClient.Setup(client => client.RunTestsAsync(It.IsAny<CancellationToken>()))
            .Callback(() => RaiseUpdates(CreateUpdate(executionState: TestNodeStates.Failed)))
            .ReturnsAsync(new MtpRunResult([]));

        var callbackFinished = false;
        using var client = CreateClient();

        await client.RunTestsAsync(async _ =>
        {
            await Task.Delay(50);
            callbackFinished = true;
        });

        callbackFinished.ShouldBeTrue();
    }

    [TestMethod]
    public async Task RunTestsAsync_SerializesRequests()
    {
        var firstRequestStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseFirstRequest = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var invocationCount = 0;

        _mtpClient.Setup(client => client.RunTestsAsync(It.IsAny<CancellationToken>()))
            .Returns(async () =>
            {
                if (Interlocked.Increment(ref invocationCount) == 1)
                {
                    firstRequestStarted.SetResult();
                    await releaseFirstRequest.Task;
                }

                return new MtpRunResult([]);
            });

        using var client = CreateClient();
        var firstRequest = client.RunTestsAsync(_ => Task.CompletedTask);
        await firstRequestStarted.Task;
        var secondRequest = client.RunTestsAsync(_ => Task.CompletedTask);

        _mtpClient.Verify(sourceClient => sourceClient.RunTestsAsync(It.IsAny<CancellationToken>()), Times.Once);

        releaseFirstRequest.SetResult();
        await Task.WhenAll(firstRequest, secondRequest);

        _mtpClient.Verify(sourceClient => sourceClient.RunTestsAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [TestMethod]
    public async Task ExitAsync_Gracefully_SendsExit()
    {
        _mtpClient.Setup(client => client.ExitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        using var client = CreateClient();
        await client.ExitAsync();

        _mtpClient.Verify(sourceClient => sourceClient.ExitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task ExitAsync_Forcefully_DisposesConnection()
    {
        var client = CreateClient();

        await client.ExitAsync(gracefully: false);

        _mtpClient.Verify(sourceClient => sourceClient.Dispose(), Times.Once);
    }

    [TestMethod]
    public async Task WaitServerProcessExitAsync_ReturnsProcessExitCode()
    {
        _processHandle.Setup(process => process.WaitForExitAsync()).ReturnsAsync(42);
        _processHandle.SetupGet(process => process.ExitCode).Returns(42);

        using var client = CreateClient();
        var exitCode = await client.WaitServerProcessExitAsync();

        exitCode.ShouldBe(42);
    }

    [TestMethod]
    public void LogReceived_LogsServerMessageAtDebugWithOriginalLevel()
    {
        using var client = CreateClient();

        _mtpClient.Raise(
            sourceClient => sourceClient.LogReceived += null,
            new MtpLogEventArgs("Warning", "Unhandled task exception"));

        _logger.Verify(
            logger => logger.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state!.ToString() == "MTP server Warning: Unhandled task exception"),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once);
        _logger.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void Dispose_DisposesSourceClientButNotProcessHandle()
    {
        var client = CreateClient();

        client.Dispose();

        _mtpClient.Verify(sourceClient => sourceClient.Dispose(), Times.Once);
    }

    private void RaiseUpdates(params MtpTestNodeUpdate[] updates)
        => _mtpClient.Raise(
            client => client.TestNodesUpdated += null,
            new MtpTestNodeUpdateEventArgs(Guid.NewGuid(), updates));

    private static MtpServerCapabilities CreateCapabilities()
        => new(
            serverProcessId: 1,
            serverName: "test-server",
            serverVersion: "1.0.0",
            supportsDiscovery: true,
            multiRequestSupport: false,
            vstestProviderSupport: false,
            supportsAttachments: false,
            multiConnectionProvider: false);

    private static MtpTestNodeUpdate CreateUpdate(
        string uid = "test-1",
        string displayName = "Test 1",
        string executionState = TestNodeStates.Passed,
        string? filePath = null,
        int? lineStart = null,
        int? lineEnd = null,
        string? typeName = null,
        string? methodName = null,
        string? errorMessage = null,
        string? errorStackTrace = null)
    {
        var node = new Dictionary<string, object?>
        {
            ["uid"] = uid,
            ["display-name"] = displayName,
            ["node-type"] = "action",
            ["execution-state"] = executionState
        };

        AddIfNotNull(node, "location.file", filePath);
        AddIfNotNull(node, "location.line-start", lineStart);
        AddIfNotNull(node, "location.line-end", lineEnd);
        AddIfNotNull(node, "location.type", typeName);
        AddIfNotNull(node, "location.method", methodName);
        AddIfNotNull(node, "error.message", errorMessage);
        AddIfNotNull(node, "error.stacktrace", errorStackTrace);

        return new MtpTestNodeUpdate(node, "parent");
    }

    private static void AddIfNotNull(IDictionary<string, object?> values, string key, object? value)
    {
        if (value is not null)
        {
            values[key] = value;
        }
    }
}
