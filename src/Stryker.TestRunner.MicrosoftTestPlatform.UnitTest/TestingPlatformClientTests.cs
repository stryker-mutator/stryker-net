using Moq;
using Nerdbank.Streams;
using Shouldly;
using StreamJsonRpc;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using System.Net.Sockets;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class TestingPlatformClientTests
{
    [TestMethod]
    public void Constructor_ShouldInitializeClient()
    {
        var stream = new MemoryStream();
        var jsonRpc = new JsonRpc(new HeaderDelimitedMessageHandler(stream, stream, new SystemTextJsonFormatter()));
        var tcpClient = new Mock<TcpClient>();
        var processHandle = new Mock<IProcessHandle>();

        using var client = new TestingPlatformClient(jsonRpc, tcpClient.Object, processHandle.Object, false);

        client.ShouldNotBeNull();
        client.JsonRpcClient.ShouldBe(jsonRpc);
    }

    [TestMethod]
    public void ExitCode_ShouldReturnProcessHandleExitCode()
    {
        var stream = new MemoryStream();
        var jsonRpc = new JsonRpc(new HeaderDelimitedMessageHandler(stream, stream, new SystemTextJsonFormatter()));
        var tcpClient = new Mock<TcpClient>();
        var processHandle = new Mock<IProcessHandle>();
        processHandle.SetupGet(x => x.ExitCode).Returns(42);

        using var client = new TestingPlatformClient(jsonRpc, tcpClient.Object, processHandle.Object, false);

        client.ExitCode.ShouldBe(42);
    }

    [TestMethod]
    public async Task WaitServerProcessExitAsync_ShouldReturnProcessExitCode()
    {
        var stream = new MemoryStream();
        var jsonRpc = new JsonRpc(new HeaderDelimitedMessageHandler(stream, stream, new SystemTextJsonFormatter()));
        var tcpClient = new Mock<TcpClient>();
        var processHandle = new Mock<IProcessHandle>();
        processHandle.Setup(x => x.WaitForExitAsync()).ReturnsAsync(0);
        processHandle.SetupGet(x => x.ExitCode).Returns(0);

        using var client = new TestingPlatformClient(jsonRpc, tcpClient.Object, processHandle.Object, false);
        var exitCode = await client.WaitServerProcessExitAsync();

        exitCode.ShouldBe(0);
        processHandle.Verify(x => x.WaitForExitAsync(), Times.Once);
    }

    [TestMethod]
    public void RegisterLogListener_ShouldAcceptListener()
    {
        var stream = new MemoryStream();
        var jsonRpc = new JsonRpc(new HeaderDelimitedMessageHandler(stream, stream, new SystemTextJsonFormatter()));
        var tcpClient = new Mock<TcpClient>();
        var processHandle = new Mock<IProcessHandle>();
        var listener = new LogsCollector();

        using var client = new TestingPlatformClient(jsonRpc, tcpClient.Object, processHandle.Object, false);

        client.RegisterLogListener(listener);
        listener.ShouldNotBeNull();
    }

    [TestMethod]
    public void RegisterTelemetryListener_ShouldAcceptListener()
    {
        var stream = new MemoryStream();
        var jsonRpc = new JsonRpc(new HeaderDelimitedMessageHandler(stream, stream, new SystemTextJsonFormatter()));
        var tcpClient = new Mock<TcpClient>();
        var processHandle = new Mock<IProcessHandle>();
        var listener = new TelemetryCollector();

        using var client = new TestingPlatformClient(jsonRpc, tcpClient.Object, processHandle.Object, false);

        client.RegisterTelemetryListener(listener);
        listener.ShouldNotBeNull();
    }

    [TestMethod]
    public void Dispose_ShouldDisposeResources()
    {
        var stream = new MemoryStream();
        var messageHandler = new HeaderDelimitedMessageHandler(stream, stream, new SystemTextJsonFormatter());
        var testableJsonRpc = new TestableJsonRpc(messageHandler);
        var testableTcpClient = new TestableTcpClient();

        var processHandle = new Mock<IProcessHandle>();

        var client = new TestingPlatformClient(testableJsonRpc, testableTcpClient, processHandle.Object, false);

        client.Dispose();

        testableJsonRpc.WasDisposed.ShouldBeTrue("JsonRpc.Dispose should be called when TestingPlatformClient is disposed");
        testableTcpClient.WasDisposed.ShouldBeTrue("TcpClient.Dispose should be called when TestingPlatformClient is disposed");
    }

    [TestMethod]
    public void Dispose_ShouldNotDisposeProcessHandle()
    {
        var stream = new MemoryStream();
        var messageHandler = new HeaderDelimitedMessageHandler(stream, stream, new SystemTextJsonFormatter());
        var jsonRpc = new TestableJsonRpc(messageHandler);
        var tcpClient = new TestableTcpClient();
        var processHandle = new Mock<IProcessHandle>();

        var client = new TestingPlatformClient(jsonRpc, tcpClient, processHandle.Object, false);

        client.Dispose();

        // ProcessHandle is not owned by TestingPlatformClient; it should not be disposed here
        processHandle.Verify(x => x.Kill(), Times.Never);
    }

    [TestMethod]
    public async Task InitializeAsync_ShouldInvokeInitializeRpcMethod()
    {
        using var connection = RpcTestConnection.Create();

        connection.ServerRpc.AddLocalRpcTarget(new FakeTestServer());
        connection.ServerRpc.StartListening();

        using var client = connection.CreateClient();

        var response = await client.InitializeAsync();

        response.ShouldNotBeNull();
        response.ServerInfo.Name.ShouldBe("fake-server");
        response.Capabilities.Testing.SupportsDiscovery.ShouldBeTrue();
    }

    [TestMethod]
    public async Task ExitAsync_Gracefully_ShouldSendExitNotification()
    {
        using var connection = RpcTestConnection.Create();

        var server = new FakeTestServer();
        connection.ServerRpc.AddLocalRpcTarget(server);
        connection.ServerRpc.StartListening();

        using var client = connection.CreateClient();

        await client.ExitAsync(gracefully: true);

        // NotifyWithParameterObjectAsync is fire-and-forget; allow processing time
        await Task.Delay(50);

        server.ExitCalled.ShouldBeTrue();
    }

    [TestMethod]
    public void ExitAsync_NotGracefully_ShouldDisposeTcpClient()
    {
        var stream = new MemoryStream();
        var jsonRpc = new JsonRpc(new HeaderDelimitedMessageHandler(stream, stream, new SystemTextJsonFormatter()));
        var testableTcpClient = new TestableTcpClient();
        var processHandle = new Mock<IProcessHandle>();

        using var client = new TestingPlatformClient(jsonRpc, testableTcpClient, processHandle.Object, false);

        // ExitAsync(gracefully: false) disposes TcpClient directly
        client.ExitAsync(gracefully: false).GetAwaiter().GetResult();

        testableTcpClient.WasDisposed.ShouldBeTrue();
    }

    [TestMethod]
    public async Task DiscoverTestsAsync_ShouldInvokeDiscoverTestsRpcMethod()
    {
        using var connection = RpcTestConnection.Create();

        var server = new FakeTestServer();
        connection.ServerRpc.AddLocalRpcTarget(server);
        connection.ServerRpc.StartListening();

        using var client = connection.CreateClient();

        var requestId = Guid.NewGuid();
        List<TestNodeUpdate> receivedUpdates = [];

        var listener = await client.DiscoverTestsAsync(requestId, updates =>
        {
            receivedUpdates.AddRange(updates);
            return Task.CompletedTask;
        });

        listener.ShouldNotBeNull();
        server.LastDiscoveryRunId.ShouldBe(requestId);
    }

    [TestMethod]
    public async Task DiscoverTestsAsync_ShouldReceiveTestUpdatesFromServer()
    {
        using var connection = RpcTestConnection.Create();

        var server = new FakeTestServer();
        connection.ServerRpc.AddLocalRpcTarget(server);
        connection.ServerRpc.StartListening();

        using var client = connection.CreateClient();

        var requestId = Guid.NewGuid();
        List<TestNodeUpdate> receivedUpdates = [];

        var listener = await client.DiscoverTestsAsync(requestId, updates =>
        {
            receivedUpdates.AddRange(updates);
            return Task.CompletedTask;
        });

        // Server sends test updates via the client's TargetHandler callback
        var testNode = new TestNode("uid-1", "TestMethod1", "action", "discovered");
        var update = new TestNodeUpdate(testNode, "parent");
        await connection.ServerRpc.InvokeWithParameterObjectAsync(
            "testing/testUpdates/tests",
            new { runId = requestId, changes = new[] { update } });

        // Server signals completion by sending null changes
        await connection.ServerRpc.InvokeWithParameterObjectAsync(
            "testing/testUpdates/tests",
            new { runId = requestId, changes = (TestNodeUpdate[]?)null });

        await listener.WaitCompletionAsync();

        receivedUpdates.Count.ShouldBe(1);
        receivedUpdates[0].Node.Uid.ShouldBe("uid-1");
        receivedUpdates[0].Node.DisplayName.ShouldBe("TestMethod1");
        receivedUpdates[0].Node.ExecutionState.ShouldBe("discovered");
    }

    [TestMethod]
    public async Task RunTestsAsync_ShouldInvokeRunTestsRpcMethod()
    {
        using var connection = RpcTestConnection.Create();

        var server = new FakeTestServer();
        connection.ServerRpc.AddLocalRpcTarget(server);
        connection.ServerRpc.StartListening();

        using var client = connection.CreateClient();

        var requestId = Guid.NewGuid();
        List<TestNodeUpdate> receivedUpdates = [];

        var listener = await client.RunTestsAsync(requestId, updates =>
        {
            receivedUpdates.AddRange(updates);
            return Task.CompletedTask;
        });

        listener.ShouldNotBeNull();
        server.LastRunTestsRunId.ShouldBe(requestId);
    }

    [TestMethod]
    public async Task RunTestsAsync_ShouldPassTestNodesToServer()
    {
        using var connection = RpcTestConnection.Create();

        var server = new FakeTestServer();
        connection.ServerRpc.AddLocalRpcTarget(server);
        connection.ServerRpc.StartListening();

        using var client = connection.CreateClient();

        var requestId = Guid.NewGuid();
        var testNodes = new[] { new TestNode("uid-1", "Test1", "action", "discovered") };

        var listener = await client.RunTestsAsync(requestId, _ => Task.CompletedTask, testNodes);

        listener.ShouldNotBeNull();
        server.LastRunTestCases.ShouldNotBeNull();
        server.LastRunTestCases!.Length.ShouldBe(1);
        server.LastRunTestCases[0].Uid.ShouldBe("uid-1");
    }

    [TestMethod]
    public async Task RunTestsAsync_ShouldReceiveTestResultsFromServer()
    {
        using var connection = RpcTestConnection.Create();

        var server = new FakeTestServer();
        connection.ServerRpc.AddLocalRpcTarget(server);
        connection.ServerRpc.StartListening();

        using var client = connection.CreateClient();

        var requestId = Guid.NewGuid();
        List<TestNodeUpdate> receivedUpdates = [];

        var listener = await client.RunTestsAsync(requestId, updates =>
        {
            receivedUpdates.AddRange(updates);
            return Task.CompletedTask;
        });

        // Server sends test results
        var passedNode = new TestNode("uid-1", "Test1", "action", "passed");
        var failedNode = new TestNode("uid-2", "Test2", "action", "failed");
        await connection.ServerRpc.InvokeWithParameterObjectAsync(
            "testing/testUpdates/tests",
            new { runId = requestId, changes = new[] { new TestNodeUpdate(passedNode, "p"), new TestNodeUpdate(failedNode, "p") } });

        // Signal completion
        await connection.ServerRpc.InvokeWithParameterObjectAsync(
            "testing/testUpdates/tests",
            new { runId = requestId, changes = (TestNodeUpdate[]?)null });

        await listener.WaitCompletionAsync();

        receivedUpdates.Count.ShouldBe(2);
        receivedUpdates.ShouldContain(u => u.Node.ExecutionState == "passed");
        receivedUpdates.ShouldContain(u => u.Node.ExecutionState == "failed");
    }

    [TestMethod]
    public async Task RunTestsAsync_ShouldPassNullTestCases_WhenNoFilterProvided()
    {
        using var connection = RpcTestConnection.Create();

        var server = new FakeTestServer();
        connection.ServerRpc.AddLocalRpcTarget(server);
        connection.ServerRpc.StartListening();

        using var client = connection.CreateClient();

        var requestId = Guid.NewGuid();

        await client.RunTestsAsync(requestId, _ => Task.CompletedTask, testNodes: null);

        server.LastRunTestCases.ShouldBeNull();
    }

    [TestMethod]
    public async Task TestsUpdate_ShouldCompleteListener_WhenNullChangesReceived()
    {
        using var connection = RpcTestConnection.Create();

        var server = new FakeTestServer();
        connection.ServerRpc.AddLocalRpcTarget(server);
        connection.ServerRpc.StartListening();

        using var client = connection.CreateClient();

        var requestId = Guid.NewGuid();
        var callbackInvoked = false;

        var listener = await client.DiscoverTestsAsync(requestId, _ =>
        {
            callbackInvoked = true;
            return Task.CompletedTask;
        });

        // Send null changes to signal completion
        await connection.ServerRpc.InvokeWithParameterObjectAsync(
            "testing/testUpdates/tests",
            new { runId = requestId, changes = (TestNodeUpdate[]?)null });

        var completed = await listener.WaitCompletionAsync(TimeSpan.FromSeconds(5));

        completed.ShouldBeTrue();
        callbackInvoked.ShouldBeFalse("Callback should not be invoked for null changes");
    }

    [TestMethod]
    public async Task TestsUpdate_ShouldIgnoreUpdatesForUnknownRunId()
    {
        using var connection = RpcTestConnection.Create();

        var server = new FakeTestServer();
        connection.ServerRpc.AddLocalRpcTarget(server);
        connection.ServerRpc.StartListening();

        using var client = connection.CreateClient();

        var requestId = Guid.NewGuid();
        var unknownRunId = Guid.NewGuid();
        var callbackInvoked = false;

        await client.DiscoverTestsAsync(requestId, _ =>
        {
            callbackInvoked = true;
            return Task.CompletedTask;
        });

        // Send update for an unknown runId
        var testNode = new TestNode("uid-1", "Test1", "action", "discovered");
        await connection.ServerRpc.InvokeWithParameterObjectAsync(
            "testing/testUpdates/tests",
            new { runId = unknownRunId, changes = new[] { new TestNodeUpdate(testNode, "p") } });

        // Give it a moment to process
        await Task.Delay(100);

        callbackInvoked.ShouldBeFalse("Callback should not be invoked for unknown runId");
    }

    [TestMethod]
    public async Task TestsUpdate_ShouldDeliverMultipleBatchesOfUpdates()
    {
        using var connection = RpcTestConnection.Create();

        var server = new FakeTestServer();
        connection.ServerRpc.AddLocalRpcTarget(server);
        connection.ServerRpc.StartListening();

        using var client = connection.CreateClient();

        var requestId = Guid.NewGuid();
        List<TestNodeUpdate> receivedUpdates = [];

        var listener = await client.RunTestsAsync(requestId, updates =>
        {
            receivedUpdates.AddRange(updates);
            return Task.CompletedTask;
        });

        // Send batch 1
        await connection.ServerRpc.InvokeWithParameterObjectAsync(
            "testing/testUpdates/tests",
            new { runId = requestId, changes = new[] { new TestNodeUpdate(new TestNode("uid-1", "Test1", "action", "passed"), "p") } });

        // Send batch 2
        await connection.ServerRpc.InvokeWithParameterObjectAsync(
            "testing/testUpdates/tests",
            new { runId = requestId, changes = new[] { new TestNodeUpdate(new TestNode("uid-2", "Test2", "action", "failed"), "p") } });

        // Signal completion
        await connection.ServerRpc.InvokeWithParameterObjectAsync(
            "testing/testUpdates/tests",
            new { runId = requestId, changes = (TestNodeUpdate[]?)null });

        await listener.WaitCompletionAsync();

        receivedUpdates.Count.ShouldBe(2);
        receivedUpdates[0].Node.Uid.ShouldBe("uid-1");
        receivedUpdates[1].Node.Uid.ShouldBe("uid-2");
    }

    [TestMethod]
    public async Task TestsUpdate_ShouldRemoveListenerAfterCompletion()
    {
        using var connection = RpcTestConnection.Create();

        var server = new FakeTestServer();
        connection.ServerRpc.AddLocalRpcTarget(server);
        connection.ServerRpc.StartListening();

        using var client = connection.CreateClient();

        var requestId = Guid.NewGuid();
        var callCount = 0;

        var listener = await client.DiscoverTestsAsync(requestId, _ =>
        {
            Interlocked.Increment(ref callCount);
            return Task.CompletedTask;
        });

        // Signal completion
        await connection.ServerRpc.InvokeWithParameterObjectAsync(
            "testing/testUpdates/tests",
            new { runId = requestId, changes = (TestNodeUpdate[]?)null });

        await listener.WaitCompletionAsync();

        // Send another update with the same runId after completion
        var testNode = new TestNode("uid-1", "Test1", "action", "discovered");
        await connection.ServerRpc.InvokeWithParameterObjectAsync(
            "testing/testUpdates/tests",
            new { runId = requestId, changes = new[] { new TestNodeUpdate(testNode, "p") } });

        await Task.Delay(100);

        callCount.ShouldBe(0, "Listener should have been removed after completion, no further callbacks");
    }

    [TestMethod]
    public async Task LogCallback_ShouldDeliverLogsToRegisteredListeners()
    {
        using var connection = RpcTestConnection.Create();

        var server = new FakeTestServer();
        connection.ServerRpc.AddLocalRpcTarget(server);
        connection.ServerRpc.StartListening();

        using var client = connection.CreateClient();
        var logCollector = new LogsCollector();
        client.RegisterLogListener(logCollector);

        await connection.ServerRpc.InvokeAsync("client/log", "Warning", "something went wrong");

        await Task.Delay(100);

        logCollector.Count.ShouldBe(1);
        logCollector.First().Message.ShouldBe("something went wrong");
    }

    [TestMethod]
    public async Task TelemetryCallback_ShouldDeliverTelemetryToRegisteredListeners()
    {
        using var connection = RpcTestConnection.Create();

        var server = new FakeTestServer();
        connection.ServerRpc.AddLocalRpcTarget(server);
        connection.ServerRpc.StartListening();

        using var client = connection.CreateClient();
        var telemetryCollector = new TelemetryCollector();
        client.RegisterTelemetryListener(telemetryCollector);

        var payload = new TelemetryPayload("test.event", new Dictionary<string, object> { ["key"] = "value" });
        await connection.ServerRpc.InvokeWithParameterObjectAsync("telemetry/update", payload);

        await Task.Delay(100);

        telemetryCollector.Count.ShouldBe(1);
        telemetryCollector.First().EventName.ShouldBe("test.event");
    }

    /// <summary>
    /// Creates a bidirectional JSON-RPC connection using duplex pipes, with a "server" side and a "client" side.
    /// </summary>
    private sealed class RpcTestConnection : IDisposable
    {
        public JsonRpc ServerRpc { get; }
        private readonly Stream _clientStream;
        private readonly TcpClient _dummyTcpClient = new();
        private readonly Mock<IProcessHandle> _processHandle = new();

        private RpcTestConnection(JsonRpc serverRpc, Stream clientStream)
        {
            ServerRpc = serverRpc;
            _clientStream = clientStream;
        }

        public static RpcTestConnection Create()
        {
            var (serverStream, clientStream) = FullDuplexStream.CreatePair();

            var serverHandler = new HeaderDelimitedMessageHandler(
                serverStream, serverStream, new SystemTextJsonFormatter());

            var serverRpc = new JsonRpc(serverHandler);

            return new RpcTestConnection(serverRpc, clientStream);
        }

        public TestingPlatformClient CreateClient()
        {
            var clientHandler = new HeaderDelimitedMessageHandler(
                _clientStream, _clientStream, new SystemTextJsonFormatter());

            var clientRpc = new JsonRpc(clientHandler);
            return new TestingPlatformClient(clientRpc, _dummyTcpClient, _processHandle.Object, false);
        }

        public void Dispose()
        {
            ServerRpc.Dispose();
            _dummyTcpClient.Dispose();
        }
    }

    /// <summary>
    /// Simulates a Microsoft Testing Platform server that handles JSON-RPC requests.
    /// </summary>
    private sealed class FakeTestServer
    {
        public bool ExitCalled { get; private set; }
        public Guid LastDiscoveryRunId { get; private set; }
        public Guid LastRunTestsRunId { get; private set; }
        public TestNode[]? LastRunTestCases { get; private set; }

        [JsonRpcMethod("initialize", UseSingleObjectParameterDeserialization = true)]
        public InitializeResponse Initialize(InitializeRequest request)
        {
            return new InitializeResponse(
                new ServerInfo("fake-server", "1.0.0"),
                new ServerCapabilities(new ServerTestingCapabilities(
                    SupportsDiscovery: true,
                    MultiRequestSupport: false,
                    VSTestProvider: false)));
        }

        [JsonRpcMethod("exit", UseSingleObjectParameterDeserialization = true)]
        public void Exit(object _)
        {
            ExitCalled = true;
        }

        [JsonRpcMethod("testing/discoverTests", UseSingleObjectParameterDeserialization = true)]
        public void DiscoverTests(DiscoveryRequest request)
        {
            LastDiscoveryRunId = request.RunId;
        }

        [JsonRpcMethod("testing/runTests", UseSingleObjectParameterDeserialization = true)]
        public void RunTests(RunTestsRequest request)
        {
            LastRunTestsRunId = request.RunId;
            LastRunTestCases = request.TestCases;
        }
    }

    private class TestableJsonRpc : JsonRpc
    {
        public bool WasDisposed { get; private set; }

        public TestableJsonRpc(IJsonRpcMessageHandler messageHandler) : base(messageHandler)
        {
        }

        protected override void Dispose(bool disposing)
        {
            WasDisposed = true;
            base.Dispose(disposing);
        }
    }

    private class TestableTcpClient : TcpClient
    {
        public bool WasDisposed { get; private set; }

        protected override void Dispose(bool disposing)
        {
            WasDisposed = true;
            base.Dispose(disposing);
        }
    }
}

