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
        var messageHandler = new HeaderDelimitedMessageHandler(stream, stream, new SystemTextJsonFormatter());
        var testableJsonRpc = new TestableJsonRpc(messageHandler);
        var testableTcpClient = new TestableTcpClient();
        
        var processHandle = new Mock<IProcessHandle>();

        var client = new TestingPlatformClient(testableJsonRpc, testableTcpClient, processHandle.Object, false);

        // Act
        client.Dispose();

        // Assert - Verify that both JsonRpc and TcpClient were disposed
        testableJsonRpc.WasDisposed.ShouldBeTrue("JsonRpc.Dispose should be called when TestingPlatformClient is disposed");
        testableTcpClient.WasDisposed.ShouldBeTrue("TcpClient.Dispose should be called when TestingPlatformClient is disposed");
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

