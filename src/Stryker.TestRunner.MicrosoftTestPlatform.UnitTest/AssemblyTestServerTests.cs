using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class AssemblyTestServerTests
{
    private const string TestAssembly = "/test/path/assembly.dll";
    private const string TestRunnerId = "test-runner-1";
    private readonly Dictionary<string, string?> _envVars = new() { ["MY_VAR"] = "value" };

    private Mock<ITestServerConnectionFactory> _factoryMock = null!;
    private Mock<ITestServerListener> _listenerMock = null!;
    private Mock<ITestServerProcess> _processMock = null!;
    private Mock<ITestingPlatformClient> _clientMock = null!;
    private Mock<IProcessHandle> _processHandleMock = null!;

    [TestInitialize]
    public void Setup()
    {
        _factoryMock = new Mock<ITestServerConnectionFactory>();
        _listenerMock = new Mock<ITestServerListener>();
        _processMock = new Mock<ITestServerProcess>();
        _clientMock = new Mock<ITestingPlatformClient>();
        _processHandleMock = new Mock<IProcessHandle>();

        _processMock.SetupGet(p => p.ProcessHandle).Returns(_processHandleMock.Object);
        _processMock.SetupGet(p => p.HasExited).Returns(false);
    }

    private AssemblyTestServer CreateServer() =>
        new(TestAssembly, _envVars, NullLogger.Instance, TestRunnerId, connectionFactory: _factoryMock.Object);

    private void SetupSuccessfulConnection(int port = 12345)
    {
        var stream = new MemoryStream();
        var connection = Mock.Of<IDisposable>();

        _factoryMock.Setup(f => f.CreateListener()).Returns((_listenerMock.Object, port));
        _factoryMock.Setup(f => f.StartProcess(TestAssembly, port, _envVars)).Returns(_processMock.Object);
        _factoryMock.Setup(f => f.CreateClient(stream, _processHandleMock.Object, null)).Returns(_clientMock.Object);

        _listenerMock.Setup(l => l.AcceptConnectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((stream, connection));

        // WaitForExitAsync should not complete (process keeps running)
        _processMock.Setup(p => p.WaitForExitAsync()).Returns(new TaskCompletionSource().Task);

        _clientMock.Setup(c => c.InitializeAsync()).ReturnsAsync((InitializeResponse)null!);
    }

    [TestMethod]
    public void Constructor_ShouldSetIsInitializedToFalse()
    {
        using var server = CreateServer();

        server.IsInitialized.ShouldBeFalse();
    }

    [TestMethod]
    public async Task StartAsync_ShouldCreateListenerAndStartProcess()
    {
        SetupSuccessfulConnection(port: 9876);

        using var server = CreateServer();
        var result = await server.StartAsync();

        result.ShouldBeTrue();
        _factoryMock.Verify(f => f.CreateListener(), Times.Once);
        _factoryMock.Verify(f => f.StartProcess(TestAssembly, 9876, _envVars), Times.Once);
    }

    [TestMethod]
    public async Task StartAsync_ShouldPassCorrectPortFromListenerToProcess()
    {
        const int expectedPort = 55555;
        SetupSuccessfulConnection(port: expectedPort);

        using var server = CreateServer();
        await server.StartAsync();

        _factoryMock.Verify(f => f.StartProcess(TestAssembly, expectedPort, _envVars), Times.Once);
    }

    [TestMethod]
    public async Task StartAsync_ShouldPassEnvironmentVariablesToProcess()
    {
        SetupSuccessfulConnection();

        using var server = CreateServer();
        await server.StartAsync();

        _factoryMock.Verify(f => f.StartProcess(TestAssembly, It.IsAny<int>(), _envVars), Times.Once);
    }

    [TestMethod]
    public async Task StartAsync_ShouldCreateClientWithStreamAndProcessHandle()
    {
        var stream = new MemoryStream();
        var connection = Mock.Of<IDisposable>();
        const int port = 12345;

        _factoryMock.Setup(f => f.CreateListener()).Returns((_listenerMock.Object, port));
        _factoryMock.Setup(f => f.StartProcess(TestAssembly, port, _envVars)).Returns(_processMock.Object);
        _listenerMock.Setup(l => l.AcceptConnectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((stream, connection));
        _processMock.Setup(p => p.WaitForExitAsync()).Returns(new TaskCompletionSource().Task);
        _factoryMock.Setup(f => f.CreateClient(stream, _processHandleMock.Object, null)).Returns(_clientMock.Object);
        _clientMock.Setup(c => c.InitializeAsync()).ReturnsAsync((InitializeResponse)null!);

        using var server = CreateServer();
        await server.StartAsync();

        _factoryMock.Verify(f => f.CreateClient(stream, _processHandleMock.Object, null), Times.Once);
    }

    [TestMethod]
    public async Task StartAsync_ShouldInitializeTheClient()
    {
        SetupSuccessfulConnection();

        using var server = CreateServer();
        await server.StartAsync();

        _clientMock.Verify(c => c.InitializeAsync(), Times.Once);
    }

    [TestMethod]
    public async Task StartAsync_ShouldSetIsInitializedToTrue()
    {
        SetupSuccessfulConnection();

        using var server = CreateServer();
        await server.StartAsync();

        server.IsInitialized.ShouldBeTrue();
    }

    [TestMethod]
    public async Task StartAsync_ShouldReturnTrue_WhenAlreadyInitialized()
    {
        SetupSuccessfulConnection();
        using var server = CreateServer();
        await server.StartAsync();

        var secondResult = await server.StartAsync();

        secondResult.ShouldBeTrue();
        // Should not create a second listener
        _factoryMock.Verify(f => f.CreateListener(), Times.Once);
    }

    [TestMethod]
    public async Task StartAsync_ShouldReturnFalse_WhenProcessExitsPrematurely()
    {
        const int port = 12345;
        _factoryMock.Setup(f => f.CreateListener()).Returns((_listenerMock.Object, port));
        _factoryMock.Setup(f => f.StartProcess(TestAssembly, port, _envVars)).Returns(_processMock.Object);

        // Process exits immediately
        _processMock.Setup(p => p.WaitForExitAsync()).Returns(Task.CompletedTask);
        _processMock.SetupGet(p => p.HasExited).Returns(true);

        // Connection never completes
        _listenerMock.Setup(l => l.AcceptConnectionAsync(It.IsAny<CancellationToken>()))
            .Returns(new TaskCompletionSource<(Stream, IDisposable)>().Task);

        using var server = CreateServer();
        var result = await server.StartAsync();

        result.ShouldBeFalse();
        server.IsInitialized.ShouldBeFalse();
    }

    [TestMethod]
    public async Task StartAsync_ShouldReturnFalse_WhenExceptionIsThrown()
    {
        _factoryMock.Setup(f => f.CreateListener()).Throws(new InvalidOperationException("boom"));

        using var server = CreateServer();
        var result = await server.StartAsync();

        result.ShouldBeFalse();
        server.IsInitialized.ShouldBeFalse();
    }

    [TestMethod]
    public async Task StartAsync_ShouldCleanUpResources_WhenProcessExitsPrematurely()
    {
        const int port = 12345;
        _factoryMock.Setup(f => f.CreateListener()).Returns((_listenerMock.Object, port));
        _factoryMock.Setup(f => f.StartProcess(TestAssembly, port, _envVars)).Returns(_processMock.Object);
        _processMock.Setup(p => p.WaitForExitAsync()).Returns(Task.CompletedTask);
        _processMock.SetupGet(p => p.HasExited).Returns(true);
        _listenerMock.Setup(l => l.AcceptConnectionAsync(It.IsAny<CancellationToken>()))
            .Returns(new TaskCompletionSource<(Stream, IDisposable)>().Task);

        using var server = CreateServer();
        await server.StartAsync();

        _listenerMock.Verify(l => l.Stop(), Times.Once);
        _processMock.Verify(p => p.Dispose(), Times.Once);
    }

    [TestMethod]
    public async Task DiscoverTestsAsync_ShouldThrow_WhenNotInitialized()
    {
        using var server = CreateServer();

        await Should.ThrowAsync<InvalidOperationException>(server.DiscoverTestsAsync);
    }

    [TestMethod]
    public async Task DiscoverTestsAsync_ShouldCallClientDiscoverTests()
    {
        SetupSuccessfulConnection();

        var listener = new TestNodeUpdatesResponseListener(Guid.NewGuid(), _ => Task.CompletedTask);
        listener.Complete();

        _clientMock.Setup(c => c.DiscoverTestsAsync(It.IsAny<Guid>(), It.IsAny<Func<TestNodeUpdate[], Task>>(), true))
            .ReturnsAsync(listener);

        using var server = CreateServer();
        await server.StartAsync();
        var result = await server.DiscoverTestsAsync();

        result.ShouldNotBeNull();
        _clientMock.Verify(c => c.DiscoverTestsAsync(It.IsAny<Guid>(), It.IsAny<Func<TestNodeUpdate[], Task>>(), true), Times.Once);
    }

    [TestMethod]
    public async Task DiscoverTestsAsync_ShouldReturnOnlyDiscoveredNodes()
    {
        SetupSuccessfulConnection();

        var discoveredNode = new TestNode("uid-1", "Test1", "action", "discovered");
        var passedNode = new TestNode("uid-2", "Test2", "action", "passed");
        var updates = new[]
        {
            new TestNodeUpdate(discoveredNode, "parent"),
            new TestNodeUpdate(passedNode, "parent")
        };

        _clientMock.Setup(c => c.DiscoverTestsAsync(It.IsAny<Guid>(), It.IsAny<Func<TestNodeUpdate[], Task>>(), true))
            .Returns<Guid, Func<TestNodeUpdate[], Task>, bool>(async (id, callback, _) =>
            {
                await callback(updates);
                var listener = new TestNodeUpdatesResponseListener(id, _ => Task.CompletedTask);
                listener.Complete();
                return listener;
            });

        using var server = CreateServer();
        await server.StartAsync();
        var result = await server.DiscoverTestsAsync();

        result.Count.ShouldBe(1);
        result[0].Uid.ShouldBe("uid-1");
        result[0].DisplayName.ShouldBe("Test1");
    }

    [TestMethod]
    public async Task RunTestsAsync_ShouldThrow_WhenNotInitialized()
    {
        using var server = CreateServer();

        await Should.ThrowAsync<InvalidOperationException>(async () => await server.RunTestsAsync(null));
    }

    [TestMethod]
    public async Task RunTestsAsync_ShouldCallClientRunTests()
    {
        SetupSuccessfulConnection();

        var listener = new TestNodeUpdatesResponseListener(Guid.NewGuid(), _ => Task.CompletedTask);
        listener.Complete();

        _clientMock.Setup(c => c.RunTestsAsync(It.IsAny<Guid>(), It.IsAny<Func<TestNodeUpdate[], Task>>(), null))
            .ReturnsAsync(listener);

        using var server = CreateServer();
        await server.StartAsync();
        var result = await server.RunTestsAsync(null);

        result.ShouldNotBeNull();
        _clientMock.Verify(c => c.RunTestsAsync(It.IsAny<Guid>(), It.IsAny<Func<TestNodeUpdate[], Task>>(), null), Times.Once);
    }

    [TestMethod]
    public async Task RunTestsAsync_ShouldPassTestNodesToClient()
    {
        SetupSuccessfulConnection();

        var testNodes = new[] { new TestNode("uid-1", "Test1", "action", "discovered") };
        var listener = new TestNodeUpdatesResponseListener(Guid.NewGuid(), _ => Task.CompletedTask);
        listener.Complete();

        _clientMock.Setup(c => c.RunTestsAsync(It.IsAny<Guid>(), It.IsAny<Func<TestNodeUpdate[], Task>>(), testNodes))
            .ReturnsAsync(listener);

        using var server = CreateServer();
        await server.StartAsync();
        await server.RunTestsAsync(testNodes);

        _clientMock.Verify(c => c.RunTestsAsync(It.IsAny<Guid>(), It.IsAny<Func<TestNodeUpdate[], Task>>(), testNodes), Times.Once);
    }

    [TestMethod]
    public async Task RunTestsAsync_ShouldCollectTestResults()
    {
        SetupSuccessfulConnection();

        var passedNode = new TestNode("uid-1", "Test1", "action", "passed");
        var failedNode = new TestNode("uid-2", "Test2", "action", "failed");
        var updates = new[]
        {
            new TestNodeUpdate(passedNode, "parent"),
            new TestNodeUpdate(failedNode, "parent")
        };

        _clientMock.Setup(c => c.RunTestsAsync(It.IsAny<Guid>(), It.IsAny<Func<TestNodeUpdate[], Task>>(), null))
            .Returns<Guid, Func<TestNodeUpdate[], Task>, TestNode[]?>(async (id, callback, _) =>
            {
                await callback(updates);
                var listener = new TestNodeUpdatesResponseListener(id, _ => Task.CompletedTask);
                listener.Complete();
                return listener;
            });

        using var server = CreateServer();
        await server.StartAsync();
        var result = await server.RunTestsAsync(null);

        result.Count.ShouldBe(2);
    }

    [TestMethod]
    public async Task RunTestsAsync_WithTimeout_ShouldReturnTimedOutFalse_WhenCompletesInTime()
    {
        SetupSuccessfulConnection();

        var listener = new TestNodeUpdatesResponseListener(Guid.NewGuid(), _ => Task.CompletedTask);
        listener.Complete();

        _clientMock.Setup(c => c.RunTestsAsync(It.IsAny<Guid>(), It.IsAny<Func<TestNodeUpdate[], Task>>(), null))
            .ReturnsAsync(listener);

        using var server = CreateServer();
        await server.StartAsync();
        var (_, timedOut) = await server.RunTestsAsync(null, TimeSpan.FromSeconds(10));

        timedOut.ShouldBeFalse();
    }

    [TestMethod]
    public async Task RunTestsAsync_WithTimeout_ShouldReturnTimedOutTrue_WhenTimesOut()
    {
        SetupSuccessfulConnection();

        // Listener that never completes
        var listener = new TestNodeUpdatesResponseListener(Guid.NewGuid(), _ => Task.CompletedTask);

        _clientMock.Setup(c => c.RunTestsAsync(It.IsAny<Guid>(), It.IsAny<Func<TestNodeUpdate[], Task>>(), null))
            .ReturnsAsync(listener);

        using var server = CreateServer();
        await server.StartAsync();
        var (_, timedOut) = await server.RunTestsAsync(null, TimeSpan.FromMilliseconds(50));

        timedOut.ShouldBeTrue();
    }

    [TestMethod]
    public async Task StopAsync_ShouldDisposeResources()
    {
        SetupSuccessfulConnection();

        _clientMock.Setup(c => c.ExitAsync(true)).Returns(Task.CompletedTask);
        _clientMock.Setup(c => c.WaitServerProcessExitAsync()).ReturnsAsync(0);

        using var server = CreateServer();
        await server.StartAsync();
        await server.StopAsync();

        _clientMock.Verify(c => c.ExitAsync(true), Times.Once);
        _clientMock.Verify(c => c.WaitServerProcessExitAsync(), Times.Once);
        _listenerMock.Verify(l => l.Stop(), Times.Once);
        _listenerMock.Verify(l => l.Dispose(), Times.Once);
        _clientMock.Verify(c => c.Dispose(), Times.Once);
        _processMock.Verify(p => p.Dispose(), Times.Once);
        server.IsInitialized.ShouldBeFalse();
    }

    [TestMethod]
    public async Task StopAsync_ShouldNotThrow_WhenClientExitFails()
    {
        SetupSuccessfulConnection();

        _clientMock.Setup(c => c.ExitAsync(true)).ThrowsAsync(new InvalidOperationException("exit failed"));

        using var server = CreateServer();
        await server.StartAsync();

        await Should.NotThrowAsync(server.StopAsync);
    }

    [TestMethod]
    public async Task StopAsync_ShouldNotThrow_WhenNotInitialized()
    {
        using var server = CreateServer();

        await Should.NotThrowAsync(server.StopAsync);
    }

    [TestMethod]
    public async Task RestartAsync_ShouldStopAndStartAgain()
    {
        // First start succeeds
        var stream1 = new MemoryStream();
        var connection1 = Mock.Of<IDisposable>();
        const int port1 = 11111;

        _factoryMock.Setup(f => f.CreateListener()).Returns((_listenerMock.Object, port1));
        _factoryMock.Setup(f => f.StartProcess(TestAssembly, port1, _envVars)).Returns(_processMock.Object);
        _factoryMock.Setup(f => f.CreateClient(stream1, _processHandleMock.Object, null)).Returns(_clientMock.Object);
        _listenerMock.Setup(l => l.AcceptConnectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((stream1, connection1));
        _processMock.Setup(p => p.WaitForExitAsync()).Returns(new TaskCompletionSource().Task);
        _clientMock.Setup(c => c.InitializeAsync()).ReturnsAsync((InitializeResponse)null!);
        _clientMock.Setup(c => c.ExitAsync(true)).Returns(Task.CompletedTask);
        _clientMock.Setup(c => c.WaitServerProcessExitAsync()).ReturnsAsync(0);

        using var server = CreateServer();
        await server.StartAsync();
        server.IsInitialized.ShouldBeTrue();

        await server.RestartAsync();

        // Should have called Stop (ExitAsync) and then Start again
        _clientMock.Verify(c => c.ExitAsync(true), Times.Once);
        _factoryMock.Verify(f => f.CreateListener(), Times.Exactly(2));
    }

    [TestMethod]
    public void Dispose_ShouldCallStopAsync()
    {
        SetupSuccessfulConnection();

        _clientMock.Setup(c => c.ExitAsync(true)).Returns(Task.CompletedTask);
        _clientMock.Setup(c => c.WaitServerProcessExitAsync()).ReturnsAsync(0);

        var server = CreateServer();
        server.StartAsync().GetAwaiter().GetResult();

        server.Dispose();

        _clientMock.Verify(c => c.ExitAsync(true), Times.Once);
        _clientMock.Verify(c => c.Dispose(), Times.Once);
    }

    [TestMethod]
    public void Dispose_ShouldBeIdempotent()
    {
        SetupSuccessfulConnection();

        _clientMock.Setup(c => c.ExitAsync(true)).Returns(Task.CompletedTask);
        _clientMock.Setup(c => c.WaitServerProcessExitAsync()).ReturnsAsync(0);

        var server = CreateServer();
        server.StartAsync().GetAwaiter().GetResult();

        server.Dispose();
        server.Dispose();

        // ExitAsync only called once because StopAsync clears _client after first dispose
        _clientMock.Verify(c => c.ExitAsync(true), Times.Once);
    }
}

