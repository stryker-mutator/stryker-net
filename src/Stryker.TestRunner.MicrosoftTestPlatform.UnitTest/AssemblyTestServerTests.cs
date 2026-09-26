using System.Net.Sockets;
using Microsoft.Extensions.Logging;
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
    private readonly Dictionary<string, string?> _environmentVariables = new() { ["MY_VAR"] = "value" };

    private Mock<ITestServerConnectionFactory> _factory = null!;
    private Mock<ITestServerListener> _listener = null!;
    private Mock<ITestServerProcess> _process = null!;
    private Mock<ITestingPlatformClient> _client = null!;
    private Mock<IProcessHandle> _processHandle = null!;

    [TestInitialize]
    public void Setup()
    {
        _factory = new Mock<ITestServerConnectionFactory>();
        _listener = new Mock<ITestServerListener>();
        _process = new Mock<ITestServerProcess>();
        _client = new Mock<ITestingPlatformClient>();
        _processHandle = new Mock<IProcessHandle>();

        _process.SetupGet(process => process.ProcessHandle).Returns(_processHandle.Object);
        _process.SetupGet(process => process.HasExited).Returns(false);
        _process.Setup(process => process.WaitForExitAsync()).Returns(new TaskCompletionSource().Task);
        _listener.Setup(listener => listener.AcceptConnectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TcpClient());
        _factory.Setup(factory => factory.CreateListener()).Returns((_listener.Object, 12345));
        _factory.Setup(factory => factory.StartProcess(TestAssembly, 12345, _environmentVariables))
            .Returns(_process.Object);
        _factory.Setup(factory => factory.CreateClient(It.IsAny<TcpClient>(), _processHandle.Object, It.IsAny<ILogger>()))
            .Returns(_client.Object);
        _client.Setup(client => client.InitializeAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _client.Setup(client => client.ExitAsync(true)).Returns(Task.CompletedTask);
        _client.Setup(client => client.WaitServerProcessExitAsync()).ReturnsAsync(0);
    }

    private AssemblyTestServer CreateServer()
        => new(TestAssembly, _environmentVariables, NullLogger.Instance, TestRunnerId, connectionFactory: _factory.Object);

    [TestMethod]
    public void Constructor_StartsUninitialized()
    {
        using var server = CreateServer();

        server.IsInitialized.ShouldBeFalse();
        server.IsAlive.ShouldBeFalse();
    }

    [TestMethod]
    public async Task StartAsync_StartsProcessAndInitializesClient()
    {
        using var server = CreateServer();

        var started = await server.StartAsync();

        started.ShouldBeTrue();
        server.IsInitialized.ShouldBeTrue();
        server.IsAlive.ShouldBeTrue();
        _factory.Verify(factory => factory.CreateListener(), Times.Once);
        _factory.Verify(factory => factory.StartProcess(TestAssembly, 12345, _environmentVariables), Times.Once);
        _factory.Verify(factory => factory.CreateClient(It.IsAny<TcpClient>(), _processHandle.Object, It.IsAny<ILogger>()), Times.Once);
        _client.Verify(client => client.InitializeAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task StartAsync_WhenAlreadyInitialized_DoesNotStartAnotherProcess()
    {
        using var server = CreateServer();
        await server.StartAsync();

        var started = await server.StartAsync();

        started.ShouldBeTrue();
        _factory.Verify(factory => factory.StartProcess(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Dictionary<string, string?>>()), Times.Once);
    }

    [TestMethod]
    public async Task StartAsync_WhenProcessExitsBeforeConnecting_ReturnsFalseAndCleansUp()
    {
        _process.Setup(process => process.WaitForExitAsync()).Returns(Task.CompletedTask);
        _process.SetupGet(process => process.HasExited).Returns(true);
        _listener.Setup(listener => listener.AcceptConnectionAsync(It.IsAny<CancellationToken>()))
            .Returns(new TaskCompletionSource<TcpClient>().Task);

        using var server = CreateServer();
        var started = await server.StartAsync();

        started.ShouldBeFalse();
        server.IsInitialized.ShouldBeFalse();
        _listener.Verify(listener => listener.Stop(), Times.Once);
        _process.Verify(process => process.Dispose(), Times.Once);
    }

    [TestMethod]
    public async Task StartAsync_WhenFactoryThrows_ReturnsFalse()
    {
        _factory.Setup(factory => factory.CreateListener()).Throws(new InvalidOperationException("boom"));

        using var server = CreateServer();
        var started = await server.StartAsync();

        started.ShouldBeFalse();
        server.IsInitialized.ShouldBeFalse();
    }

    [TestMethod]
    public async Task StartAsync_WhenClientCreationThrows_DisposesAcceptedConnection()
    {
        using var tcpClient = new TcpClient();
        _listener.Setup(listener => listener.AcceptConnectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tcpClient);
        _factory.Setup(factory => factory.CreateClient(tcpClient, _processHandle.Object, It.IsAny<ILogger>()))
            .Throws(new InvalidOperationException("boom"));

        using var server = CreateServer();
        var started = await server.StartAsync();

        started.ShouldBeFalse();
        tcpClient.Client.ShouldBeNull();
    }

    [TestMethod]
    public async Task IsAlive_WhenProcessExitsAfterStart_ReturnsFalse()
    {
        using var server = CreateServer();
        await server.StartAsync();
        _process.SetupGet(process => process.HasExited).Returns(true);

        server.IsAlive.ShouldBeFalse();
    }

    [TestMethod]
    public async Task DiscoverTestsAsync_WhenNotInitialized_Throws()
    {
        using var server = CreateServer();

        await Should.ThrowAsync<InvalidOperationException>(server.DiscoverTestsAsync);
    }

    [TestMethod]
    public async Task DiscoverTestsAsync_ReturnsOnlyDiscoveredNodes()
    {
        var discovered = new TestNode("uid-1", "Test 1", "action", TestNodeStates.Discovered);
        var passed = new TestNode("uid-2", "Test 2", "action", TestNodeStates.Passed);
        _client.Setup(client => client.DiscoverTestsAsync(
                It.IsAny<Func<TestNodeUpdate[], Task>>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<TestNodeUpdate[], Task>, CancellationToken>(async (callback, _) =>
                await callback([
                    new TestNodeUpdate(discovered, "parent"),
                    new TestNodeUpdate(passed, "parent")
                ]));

        using var server = CreateServer();
        await server.StartAsync();
        var tests = await server.DiscoverTestsAsync();

        tests.ShouldBe([discovered]);
    }

    [TestMethod]
    public async Task RunTestsAsync_WhenNotInitialized_Throws()
    {
        using var server = CreateServer();

        await Should.ThrowAsync<InvalidOperationException>(async () => await server.RunTestsAsync(null));
    }

    [TestMethod]
    public async Task RunTestsAsync_PassesSelectionAndCollectsResults()
    {
        var selection = new[] { new TestNode("uid-1", "Test 1", "action", TestNodeStates.Discovered) };
        var result = new TestNodeUpdate(
            new TestNode("uid-1", "Test 1", "action", TestNodeStates.Passed),
            "parent");
        _client.Setup(client => client.RunTestsAsync(
                It.IsAny<Func<TestNodeUpdate[], Task>>(),
                selection,
                It.IsAny<CancellationToken>()))
            .Returns<Func<TestNodeUpdate[], Task>, TestNode[]?, CancellationToken>(async (callback, _, _) =>
                await callback([result]));

        using var server = CreateServer();
        await server.StartAsync();
        var results = await server.RunTestsAsync(selection);

        results.ShouldBe([result]);
        _client.Verify(client => client.RunTestsAsync(
            It.IsAny<Func<TestNodeUpdate[], Task>>(),
            selection,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task RunTestsAsync_WithTimeout_CompletesWithoutTimeout()
    {
        _client.Setup(client => client.RunTestsAsync(
                It.IsAny<Func<TestNodeUpdate[], Task>>(),
                null,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        using var server = CreateServer();
        await server.StartAsync();
        var (_, timedOut) = await server.RunTestsAsync(null, TimeSpan.FromSeconds(1));

        timedOut.ShouldBeFalse();
    }

    [TestMethod]
    public async Task RunTestsAsync_WithTimeout_CancelsRequestAndKeepsPartialResults()
    {
        var partialResult = new TestNodeUpdate(
            new TestNode("uid-1", "Test 1", "action", TestNodeStates.Passed),
            "parent");
        _client.Setup(client => client.RunTestsAsync(
                It.IsAny<Func<TestNodeUpdate[], Task>>(),
                null,
                It.IsAny<CancellationToken>()))
            .Returns<Func<TestNodeUpdate[], Task>, TestNode[]?, CancellationToken>(async (callback, _, cancellationToken) =>
            {
                await callback([partialResult]);
                await Task.Delay(Timeout.Infinite, cancellationToken);
            });

        using var server = CreateServer();
        await server.StartAsync();
        var (results, timedOut) = await server.RunTestsAsync(null, TimeSpan.FromMilliseconds(25));

        timedOut.ShouldBeTrue();
        results.ShouldBe([partialResult]);
        _client.Verify(client => client.RunTestsAsync(
            It.IsAny<Func<TestNodeUpdate[], Task>>(),
            null,
            It.Is<CancellationToken>(token => token.CanBeCanceled)), Times.Once);
    }

    [TestMethod]
    public async Task RunTestsAsync_WhenHostCrashes_ThrowsTestHostCrashed()
    {
        _client.Setup(client => client.RunTestsAsync(
                It.IsAny<Func<TestNodeUpdate[], Task>>(),
                null,
                It.IsAny<CancellationToken>()))
            .Callback(() => _process.SetupGet(process => process.HasExited).Returns(true))
            .ThrowsAsync(new IOException("connection closed"));

        using var server = CreateServer();
        await server.StartAsync();

        await Should.ThrowAsync<TestHostCrashedException>(async () => await server.RunTestsAsync(null));
    }

    [TestMethod]
    public async Task StopAsync_GracefullyStopsAndDisposesResources()
    {
        using var server = CreateServer();
        await server.StartAsync();

        await server.StopAsync();

        _client.Verify(client => client.ExitAsync(true), Times.Once);
        _client.Verify(client => client.WaitServerProcessExitAsync(), Times.Once);
        _client.Verify(client => client.Dispose(), Times.Once);
        _listener.Verify(listener => listener.Stop(), Times.Once);
        _listener.Verify(listener => listener.Dispose(), Times.Once);
        _process.Verify(process => process.Dispose(), Times.Once);
        server.IsInitialized.ShouldBeFalse();
    }

    [TestMethod]
    public async Task StopAsync_ForcefullyKillsProcessWithoutSendingExit()
    {
        using var server = CreateServer();
        await server.StartAsync();

        await server.StopAsync(force: true);

        _processHandle.Verify(process => process.Kill(), Times.Once);
        _client.Verify(client => client.ExitAsync(It.IsAny<bool>()), Times.Never);
    }

    [TestMethod]
    public async Task StopAsync_WhenExitFails_StillDisposesResources()
    {
        _client.Setup(client => client.ExitAsync(true)).ThrowsAsync(new InvalidOperationException("exit failed"));

        using var server = CreateServer();
        await server.StartAsync();

        await Should.NotThrowAsync(server.StopAsync());
        _client.Verify(client => client.Dispose(), Times.Once);
        _process.Verify(process => process.Dispose(), Times.Once);
    }

    [TestMethod]
    public async Task RestartAsync_StartsASecondServer()
    {
        using var server = CreateServer();
        await server.StartAsync();

        await server.RestartAsync();

        _factory.Verify(factory => factory.CreateListener(), Times.Exactly(2));
        _client.Verify(client => client.InitializeAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [TestMethod]
    public async Task Dispose_IsIdempotent()
    {
        var server = CreateServer();
        await server.StartAsync();

        server.Dispose();
        server.Dispose();

        _process.Verify(process => process.Dispose(), Times.Once);
    }
}
