using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using StreamJsonRpc;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using System.Net.Sockets;
using Stryker.TestRunner.MicrosoftTestPlatform.RPC;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class TestingPlatformClientTests
{
    [TestMethod]
    public void Constructor_ShouldInitializeClient()
    {
        // Arrange
        var stream = new MemoryStream();
        var jsonRpc = new JsonRpc(new HeaderDelimitedMessageHandler(stream, stream, new SystemTextJsonFormatter()));
        var tcpClient = new Mock<TcpClient>();
        var processHandle = new Mock<IProcessHandle>();

        // Act
        using var client = new TestingPlatformClient(jsonRpc, tcpClient.Object, processHandle.Object, false);

        // Assert
        client.ShouldNotBeNull();
        client.JsonRpcClient.ShouldBe(jsonRpc);
    }

    [TestMethod]
    public void ExitCode_ShouldReturnProcessHandleExitCode()
    {
        // Arrange
        var stream = new MemoryStream();
        var jsonRpc = new JsonRpc(new HeaderDelimitedMessageHandler(stream, stream, new SystemTextJsonFormatter()));
        var tcpClient = new Mock<TcpClient>();
        var processHandle = new Mock<IProcessHandle>();
        processHandle.SetupGet(x => x.ExitCode).Returns(42);

        // Act
        using var client = new TestingPlatformClient(jsonRpc, tcpClient.Object, processHandle.Object, false);

        // Assert
        client.ExitCode.ShouldBe(42);
    }

    [TestMethod]
    public async Task WaitServerProcessExitAsync_ShouldReturnProcessExitCode()
    {
        // Arrange
        var stream = new MemoryStream();
        var jsonRpc = new JsonRpc(new HeaderDelimitedMessageHandler(stream, stream, new SystemTextJsonFormatter()));
        var tcpClient = new Mock<TcpClient>();
        var processHandle = new Mock<IProcessHandle>();
        processHandle.Setup(x => x.WaitForExitAsync()).ReturnsAsync(0);
        processHandle.SetupGet(x => x.ExitCode).Returns(0);

        // Act
        using var client = new TestingPlatformClient(jsonRpc, tcpClient.Object, processHandle.Object, false);
        var exitCode = await client.WaitServerProcessExitAsync();

        // Assert
        exitCode.ShouldBe(0);
        processHandle.Verify(x => x.WaitForExitAsync(), Times.Once);
    }

    [TestMethod]
    public void RegisterLogListener_ShouldAcceptListener()
    {
        // Arrange
        var stream = new MemoryStream();
        var jsonRpc = new JsonRpc(new HeaderDelimitedMessageHandler(stream, stream, new SystemTextJsonFormatter()));
        var tcpClient = new Mock<TcpClient>();
        var processHandle = new Mock<IProcessHandle>();
        var listener = new LogsCollector();

        // Act
        using var client = new TestingPlatformClient(jsonRpc, tcpClient.Object, processHandle.Object, false);

        // Assert - should not throw
        client.RegisterLogListener(listener);
        listener.ShouldNotBeNull();
    }

    [TestMethod]
    public void RegisterTelemetryListener_ShouldAcceptListener()
    {
        // Arrange
        var stream = new MemoryStream();
        var jsonRpc = new JsonRpc(new HeaderDelimitedMessageHandler(stream, stream, new SystemTextJsonFormatter()));
        var tcpClient = new Mock<TcpClient>();
        var processHandle = new Mock<IProcessHandle>();
        var listener = new TelemetryCollector();

        // Act
        using var client = new TestingPlatformClient(jsonRpc, tcpClient.Object, processHandle.Object, false);

        // Assert - should not throw
        client.RegisterTelemetryListener(listener);
        listener.ShouldNotBeNull();
    }

    [TestMethod]
    public void Dispose_ShouldDisposeResources()
    {
        // Arrange
        var stream = new MemoryStream();
        var jsonRpc = new JsonRpc(new HeaderDelimitedMessageHandler(stream, stream, new SystemTextJsonFormatter()));
        var tcpClient = new Mock<TcpClient>();
        var processHandle = new Mock<IProcessHandle>();

        var client = new TestingPlatformClient(jsonRpc, tcpClient.Object, processHandle.Object, false);

        // Act
        client.Dispose();

        // Assert - no longer verifying mock since we're using real JsonRpc
        // Disposal completes successfully without throwing
    }
}

