using Microsoft.Extensions.Logging.Abstractions;
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

    private TestingPlatformClient CreateClient()
        => new(_mtpClient.Object, _processHandle.Object, NullLogger.Instance);

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
        string? methodName = null)
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
