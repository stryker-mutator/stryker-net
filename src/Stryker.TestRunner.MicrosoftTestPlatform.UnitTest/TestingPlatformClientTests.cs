using Microsoft.Extensions.Logging.Abstractions;
using Moq;
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
        // Arrange
        var jsonRpc = new Mock<JsonRpc>(MockBehavior.Loose, new object[] { new Mock<IJsonRpcMessageHandler>().Object });
        var tcpClient = new Mock<TcpClient>();
        var processHandle = new Mock<IProcessHandle>();

        // Act
        using var client = new TestingPlatformClient(jsonRpc.Object, tcpClient.Object, processHandle.Object, false);

        // Assert
        client.ShouldNotBeNull();
        client.JsonRpcClient.ShouldBe(jsonRpc.Object);
    }

    [TestMethod]
    public void ExitCode_ShouldReturnProcessHandleExitCode()
    {
        // Arrange
        var jsonRpc = new Mock<JsonRpc>(MockBehavior.Loose, new object[] { new Mock<IJsonRpcMessageHandler>().Object });
        var tcpClient = new Mock<TcpClient>();
        var processHandle = new Mock<IProcessHandle>();
        processHandle.SetupGet(x => x.ExitCode).Returns(42);

        // Act
        using var client = new TestingPlatformClient(jsonRpc.Object, tcpClient.Object, processHandle.Object, false);

        // Assert
        client.ExitCode.ShouldBe(42);
    }

    [TestMethod]
    public async Task WaitServerProcessExitAsync_ShouldReturnProcessExitCode()
    {
        // Arrange
        var jsonRpc = new Mock<JsonRpc>(MockBehavior.Loose, new object[] { new Mock<IJsonRpcMessageHandler>().Object });
        var tcpClient = new Mock<TcpClient>();
        var processHandle = new Mock<IProcessHandle>();
        processHandle.Setup(x => x.WaitForExitAsync()).ReturnsAsync(0);
        processHandle.SetupGet(x => x.ExitCode).Returns(0);

        // Act
        using var client = new TestingPlatformClient(jsonRpc.Object, tcpClient.Object, processHandle.Object, false);
        var exitCode = await client.WaitServerProcessExitAsync();

        // Assert
        exitCode.ShouldBe(0);
        processHandle.Verify(x => x.WaitForExitAsync(), Times.Once);
    }

    [TestMethod]
    public void RegisterLogListener_ShouldAcceptListener()
    {
        // Arrange
        var jsonRpc = new Mock<JsonRpc>(MockBehavior.Loose, new object[] { new Mock<IJsonRpcMessageHandler>().Object });
        var tcpClient = new Mock<TcpClient>();
        var processHandle = new Mock<IProcessHandle>();
        var listener = new LogsCollector();

        // Act
        using var client = new TestingPlatformClient(jsonRpc.Object, tcpClient.Object, processHandle.Object, false);

        // Assert - should not throw
        client.RegisterLogListener(listener);
        listener.ShouldNotBeNull();
    }

    [TestMethod]
    public void RegisterTelemetryListener_ShouldAcceptListener()
    {
        // Arrange
        var jsonRpc = new Mock<JsonRpc>(MockBehavior.Loose, new object[] { new Mock<IJsonRpcMessageHandler>().Object });
        var tcpClient = new Mock<TcpClient>();
        var processHandle = new Mock<IProcessHandle>();
        var listener = new TelemetryCollector();

        // Act
        using var client = new TestingPlatformClient(jsonRpc.Object, tcpClient.Object, processHandle.Object, false);

        // Assert - should not throw
        client.RegisterTelemetryListener(listener);
        listener.ShouldNotBeNull();
    }

    [TestMethod]
    public void Dispose_ShouldDisposeResources()
    {
        // Arrange
        var jsonRpc = new Mock<JsonRpc>(MockBehavior.Loose, new object[] { new Mock<IJsonRpcMessageHandler>().Object });
        var tcpClient = new Mock<TcpClient>();
        var processHandle = new Mock<IProcessHandle>();

        var client = new TestingPlatformClient(jsonRpc.Object, tcpClient.Object, processHandle.Object, false);

        // Act
        client.Dispose();

        // Assert
        jsonRpc.Verify(x => x.Dispose(), Times.Once);
        tcpClient.Verify(x => x.Dispose(), Times.Once);
    }
}

