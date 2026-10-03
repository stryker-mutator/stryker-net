using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using StreamJsonRpc;

namespace Stryker.CLI.MutationServer;

/// <summary>
/// Runs a Mutation Server Protocol endpoint.
/// </summary>
public interface IMutationServer
{
    /// <summary>
    /// Runs the server until its transport closes or cancellation is requested.
    /// </summary>
    Task RunAsync(
        string channel,
        int? port,
        string address,
        IReadOnlyCollection<string> serverArguments,
        CancellationToken cancellationToken);
}

internal sealed class MutationServerHost : IMutationServer
{
    private readonly MutationServerService _service;
    private readonly Action<IPEndPoint> _listenerStarted;
    private readonly SemaphoreSlim _socketConnection = new(1, 1);
    private IReadOnlyCollection<string> _serverArguments = [];

    public MutationServerHost(
        MutationServerService service,
        Action<IPEndPoint> listenerStarted = null)
    {
        _service = service;
        _listenerStarted = listenerStarted;
    }

    public async Task RunAsync(
        string channel,
        int? port,
        string address,
        IReadOnlyCollection<string> serverArguments,
        CancellationToken cancellationToken)
    {
        _serverArguments = serverArguments;

        if (string.Equals(channel, "stdio", StringComparison.OrdinalIgnoreCase))
        {
            _service.SetServerArguments(serverArguments);
            await RunConnectionAsync(
                Console.OpenStandardInput(),
                Console.OpenStandardOutput(),
                cancellationToken).ConfigureAwait(false);
            return;
        }

        if (!string.Equals(channel, "socket", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException($"Unsupported mutation server channel '{channel}'.");
        }

        await RunSocketServerAsync(
            address ?? "localhost",
            port ?? throw new ArgumentException("The --port option is required for the socket channel."),
            cancellationToken).ConfigureAwait(false);
    }

    private async Task RunSocketServerAsync(string address, int port, CancellationToken cancellationToken)
    {
        var ipAddress = await ResolveAddressAsync(address, cancellationToken).ConfigureAwait(false);
        var listener = new TcpListener(ipAddress, port);
        try
        {
            listener.Start();
        }
        catch (SocketException exception)
        {
            throw new MutationServerException(
                $"Unable to start the mutation server on {address}:{port}.",
                exception);
        }

        var boundPort = ((IPEndPoint)listener.LocalEndpoint).Port;
        _listenerStarted?.Invoke((IPEndPoint)listener.LocalEndpoint);
        await Console.Error.WriteLineAsync($"Stryker server listening on {address}:{boundPort}").ConfigureAwait(false);

        var connections = new List<Task>();
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync(cancellationToken).ConfigureAwait(false);
                connections.RemoveAll(connection => connection.IsCompleted);
                connections.Add(RunClientAsync(client, cancellationToken));
            }

        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        finally
        {
            listener.Stop();
            await Task.WhenAll(connections).ConfigureAwait(false);
        }
    }

    internal bool HasActiveSocketClient => _socketConnection.CurrentCount == 0;

    private async Task RunClientAsync(TcpClient client, CancellationToken cancellationToken)
    {
        if (!await _socketConnection.WaitAsync(0, cancellationToken).ConfigureAwait(false))
        {
            client.Dispose();
            await Console.Error.WriteLineAsync(
                "The mutation server supports one active socket client at a time.").ConfigureAwait(false);
            return;
        }

        try
        {
            using (client)
            {
                _service.SetServerArguments(_serverArguments);
                var stream = client.GetStream();
                await RunConnectionAsync(stream, stream, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (MutationServerException exception)
        {
            await Console.Error.WriteLineAsync(exception.Message).ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is IOException or SocketException)
        {
            await Console.Error.WriteLineAsync(
                $"Mutation server client connection failed: {exception.Message}").ConfigureAwait(false);
        }
        finally
        {
            _socketConnection.Release();
        }
    }

    internal async Task RunConnectionAsync(
        Stream receivingStream,
        Stream sendingStream,
        CancellationToken cancellationToken)
    {
        var formatter = new SystemTextJsonFormatter
        {
            JsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            }
        };
        using var rpc = new JsonRpc(new HeaderDelimitedMessageHandler(sendingStream, receivingStream, formatter));
        using var connectionCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var target = new MutationServerRpcTarget(_service, rpc, connectionCancellation.Token);
        rpc.AddLocalRpcTarget(
            target,
            new JsonRpcTargetOptions
            {
                UseSingleObjectParameterDeserialization = true
            });
        rpc.StartListening();

        Exception completionException = null;
        try
        {
            await rpc.Completion.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            completionException = exception;
        }

        await connectionCancellation.CancelAsync().ConfigureAwait(false);
        await _service.WaitForIdleAsync().ConfigureAwait(false);
        if (completionException is not null)
        {
            throw new MutationServerException(
                "The mutation server connection ended because it received an invalid message.",
                completionException);
        }
    }

    private static async Task<IPAddress> ResolveAddressAsync(string address, CancellationToken cancellationToken)
    {
        if (IPAddress.TryParse(address, out var ipAddress))
        {
            return ipAddress;
        }

        var addresses = await Dns.GetHostAddressesAsync(address, cancellationToken).ConfigureAwait(false);
        return addresses.FirstOrDefault(candidate => candidate.AddressFamily == AddressFamily.InterNetwork)
               ?? addresses.First();
    }

    private sealed class MutationServerRpcTarget
    {
        private readonly MutationServerService _service;
        private readonly JsonRpc _rpc;
        private readonly CancellationToken _connectionCancellationToken;

        public MutationServerRpcTarget(
            MutationServerService service,
            JsonRpc rpc,
            CancellationToken connectionCancellationToken)
        {
            _service = service;
            _rpc = rpc;
            _connectionCancellationToken = connectionCancellationToken;
        }

        [JsonRpcMethod(MutationServerProtocol.ConfigureMethod)]
        public ConfigureResult Configure(ConfigureParams parameters)
            => _service.Configure(parameters);

        [JsonRpcMethod(MutationServerProtocol.DiscoverMethod)]
        public async Task<DiscoverResult> DiscoverAsync(
            DiscoverParams parameters,
            CancellationToken cancellationToken)
        {
            using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _connectionCancellationToken);
            return await _service.DiscoverAsync(parameters, linkedCancellation.Token).ConfigureAwait(false);
        }

        [JsonRpcMethod(MutationServerProtocol.MutationTestMethod)]
        public async Task<MutationTestResult> MutationTestAsync(
            MutationTestParams parameters,
            CancellationToken cancellationToken)
        {
            using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                _connectionCancellationToken);
            return await _service.MutationTestAsync(
                parameters,
                result => _rpc.NotifyWithParameterObjectAsync(
                    MutationServerProtocol.ReportMutationTestProgressMethod,
                    result),
                linkedCancellation.Token).ConfigureAwait(false);
        }
    }
}

internal sealed class MutationServerException : Exception
{
    public MutationServerException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
