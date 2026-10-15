using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Nerdbank.Streams;
using Shouldly;
using Spectre.Console;
using StreamJsonRpc;
using Stryker.CLI.Logging;
using Stryker.CLI.MutationServer;

namespace Stryker.CLI.UnitTest.MutationServer;

[TestClass]
public class MutationServerHostTests
{
    [TestMethod]
    public async Task ConfigureShouldBindObjectParametersAndReturnProtocolVersion()
    {
        var service = new MutationServerService(
            Mock.Of<IServiceProvider>(),
            Mock.Of<IConfigBuilder>(),
            Mock.Of<ILoggingInitializer>(),
            Mock.Of<IAnsiConsole>());
        var host = new MutationServerHost(service);
        var (serverStream, clientStream) = FullDuplexStream.CreatePair();
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var serverTask = host.RunConnectionAsync(serverStream, serverStream, cancellationTokenSource.Token);
        var formatter = new SystemTextJsonFormatter
        {
            JsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            }
        };
        using var client = new JsonRpc(new HeaderDelimitedMessageHandler(clientStream, clientStream, formatter));
        client.StartListening();

        var result = await client.InvokeWithParameterObjectAsync<ConfigureResult>(
            MutationServerProtocol.ConfigureMethod,
            new ConfigureParams());

        result.Version.ShouldBe(MutationServerProtocol.Version);

        clientStream.Dispose();
        await serverTask;
    }

    [TestMethod]
    public async Task SocketChannelShouldAcceptJsonRpcConnections()
    {
        var service = new MutationServerService(
            Mock.Of<IServiceProvider>(),
            Mock.Of<IConfigBuilder>(),
            Mock.Of<ILoggingInitializer>(),
            Mock.Of<IAnsiConsole>());
        var serverStarted = new TaskCompletionSource<int>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var host = new MutationServerHost(
            service,
            endpoint => serverStarted.TrySetResult(endpoint.Port));
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var serverTask = host.RunAsync(
            "socket",
            0,
            IPAddress.Loopback.ToString(),
            Array.Empty<string>(),
            cancellationTokenSource.Token);
        var port = await serverStarted.Task.WaitAsync(cancellationTokenSource.Token);
        using var tcpClient = new TcpClient();

        await ConnectAsync(tcpClient, port, cancellationTokenSource.Token);
        using var client = new JsonRpc(new HeaderDelimitedMessageHandler(
            tcpClient.GetStream(),
            tcpClient.GetStream(),
            CreateFormatter()));
        client.StartListening();

        var result = await client.InvokeWithParameterObjectAsync<ConfigureResult>(
            MutationServerProtocol.ConfigureMethod,
            new ConfigureParams());

        result.Version.ShouldBe(MutationServerProtocol.Version);

        client.Dispose();
        tcpClient.Dispose();
        cancellationTokenSource.Cancel();
        await serverTask;
    }

    [TestMethod]
    public async Task SocketChannelShouldReportPortBindingFailures()
    {
        var service = CreateService();
        var host = new MutationServerHost(service);
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;

        var exception = await Should.ThrowAsync<MutationServerException>(() => host.RunAsync(
            "socket",
            port,
            IPAddress.Loopback.ToString(),
            Array.Empty<string>(),
            CancellationToken.None));

        exception.Message.ShouldContain(port.ToString());
    }

    [TestMethod]
    public async Task InvalidFramesShouldEndTheConnectionWithACleanServerException()
    {
        var host = new MutationServerHost(CreateService());
        var (serverStream, clientStream) = FullDuplexStream.CreatePair();
        var serverTask = host.RunConnectionAsync(serverStream, serverStream, CancellationToken.None);

        await clientStream.WriteAsync(Encoding.ASCII.GetBytes("Content-Length: 0\r\n\r\n"));
        await clientStream.FlushAsync();

        var exception = await Should.ThrowAsync<MutationServerException>(
            () => serverTask.WaitAsync(TimeSpan.FromSeconds(1)));
        exception.Message.ShouldContain("invalid message");
    }

    [TestMethod]
    public async Task SocketChannelShouldRejectConcurrentClientsAndAllowReuse()
    {
        var serverStarted = new TaskCompletionSource<int>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var host = new MutationServerHost(
            CreateService(),
            endpoint => serverStarted.TrySetResult(endpoint.Port));
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var serverTask = host.RunAsync(
            "socket",
            0,
            IPAddress.Loopback.ToString(),
            Array.Empty<string>(),
            cancellationTokenSource.Token);
        var port = await serverStarted.Task.WaitAsync(cancellationTokenSource.Token);
        using var firstClient = new TcpClient();
        await firstClient.ConnectAsync(IPAddress.Loopback, port, cancellationTokenSource.Token);
        using var firstRpc = CreateClient(firstClient);

        (await firstRpc.InvokeWithParameterObjectAsync<ConfigureResult>(
            MutationServerProtocol.ConfigureMethod,
            new ConfigureParams())).Version.ShouldBe(MutationServerProtocol.Version);

        using var rejectedClient = new TcpClient();
        await rejectedClient.ConnectAsync(IPAddress.Loopback, port, cancellationTokenSource.Token);
        var buffer = new byte[1];
        (await rejectedClient.GetStream().ReadAsync(buffer, cancellationTokenSource.Token))
            .ShouldBe(0);
        (await firstRpc.InvokeWithParameterObjectAsync<ConfigureResult>(
            MutationServerProtocol.ConfigureMethod,
            new ConfigureParams())).Version.ShouldBe(MutationServerProtocol.Version);

        firstRpc.Dispose();
        firstClient.Dispose();
        SpinWait.SpinUntil(
                () => !host.HasActiveSocketClient,
                TimeSpan.FromSeconds(1))
            .ShouldBeTrue();
        using var nextClient = new TcpClient();
        await nextClient.ConnectAsync(IPAddress.Loopback, port, cancellationTokenSource.Token);
        using var nextRpc = CreateClient(nextClient);
        (await nextRpc.InvokeWithParameterObjectAsync<ConfigureResult>(
            MutationServerProtocol.ConfigureMethod,
            new ConfigureParams())).Version.ShouldBe(MutationServerProtocol.Version);

        nextRpc.Dispose();
        nextClient.Dispose();
        cancellationTokenSource.Cancel();
        await serverTask;
    }

    private static SystemTextJsonFormatter CreateFormatter()
        => new()
        {
            JsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            }
        };

    private static MutationServerService CreateService()
        => new(
            Mock.Of<IServiceProvider>(),
            Mock.Of<IConfigBuilder>(),
            Mock.Of<ILoggingInitializer>(),
            Mock.Of<IAnsiConsole>());

    private static JsonRpc CreateClient(TcpClient client)
    {
        var rpc = new JsonRpc(new HeaderDelimitedMessageHandler(
            client.GetStream(),
            client.GetStream(),
            CreateFormatter()));
        rpc.StartListening();
        return rpc;
    }

    private static async Task ConnectAsync(
        TcpClient client,
        int port,
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await client.ConnectAsync(IPAddress.Loopback, port, cancellationToken);
                return;
            }
            catch (SocketException)
            {
                await Task.Delay(20, cancellationToken);
            }
        }
    }
}
