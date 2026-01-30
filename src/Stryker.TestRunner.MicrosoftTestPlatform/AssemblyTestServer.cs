using System.Net;
using System.Net.Sockets;
using CliWrap;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using StreamJsonRpc;
using Stryker.Abstractions.Options;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.MicrosoftTestPlatform.RPC;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

/// <summary>
/// Manages a persistent test server connection for a single assembly.
/// The server process is started once and reused across multiple test runs.
/// </summary>
internal sealed class AssemblyTestServer : IDisposable
{
    private readonly string _assembly;
    private readonly Dictionary<string, string?> _environmentVariables;
    private readonly ILogger _logger;
    private readonly string _runnerId;
    private readonly IStrykerOptions? _options;
    private TcpListener? _listener;
    private TcpClient? _tcpClient;
    private NetworkStream? _stream;
    private JsonRpc? _rpc;
    private TestingPlatformClient? _client;
    private MemoryStream? _outputStream;
    private bool _isInitialized;
    private bool _disposed;

    public AssemblyTestServer(string assembly, Dictionary<string, string?> environmentVariables, ILogger logger, string runnerId, IStrykerOptions? options = null)
    {
        _assembly = assembly;
        _environmentVariables = environmentVariables;
        _logger = logger;
        _runnerId = runnerId;
        _options = options;
    }

    public bool IsInitialized => _isInitialized;

    public async Task<bool> StartAsync(CancellationToken cancellationToken = default)
    {
        if (_isInitialized)
        {
            return true;
        }

        try
        {
            _listener = new TcpListener(new IPEndPoint(IPAddress.Any, 0));
            _listener.Start();

            var port = ((IPEndPoint)_listener.LocalEndpoint).Port;

            _outputStream = new MemoryStream();
            var outputPipe = PipeTarget.ToStream(_outputStream);

            var _cliProcess = Cli.Wrap("dotnet")
                .WithWorkingDirectory(Path.GetDirectoryName(_assembly) ?? string.Empty)
                .WithArguments([_assembly, "--server", "--client-port", port.ToString()])
                .WithEnvironmentVariables(_environmentVariables)
                .WithStandardOutputPipe(outputPipe)
                .WithStandardErrorPipe(outputPipe)
                .ExecuteAsync(cancellationToken: cancellationToken);

            var tcpClientTask = _listener.AcceptTcpClientAsync(cancellationToken).AsTask();
            var connectionTimeout = Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            var completedTask = await Task.WhenAny(_cliProcess.Task, tcpClientTask, connectionTimeout).ConfigureAwait(false);

            if (completedTask == connectionTimeout)
            {
                _logger.LogDebug("{RunnerId}: Timeout waiting for test server connection for {Assembly}", _runnerId, _assembly);
                await StopAsync().ConfigureAwait(false);
                return false;
            }

            if (completedTask == _cliProcess.Task)
            {
                _logger.LogDebug("{RunnerId}: Test process exited prematurely for {Assembly}", _runnerId, _assembly);
                await StopAsync().ConfigureAwait(false);
                return false;
            }

            _tcpClient = await tcpClientTask.ConfigureAwait(false);
            _stream = _tcpClient.GetStream();

            _rpc = new JsonRpc(new HeaderDelimitedMessageHandler(_stream, _stream, new SystemTextJsonFormatter
            {
                JsonSerializerOptions = RpcJsonSerializerOptions.Default
            }));

            _client = new TestingPlatformClient(_rpc, _tcpClient, new ProcessHandle(_cliProcess, _outputStream), enableDiagnostic: _options?.LogOptions.LogLevel == LogEventLevel.Verbose);

            await _client.InitializeAsync().ConfigureAwait(false);
            _isInitialized = true;

            _logger.LogDebug("{RunnerId}: Test server started successfully for {Assembly}", _runnerId, _assembly);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "{RunnerId}: Failed to start test server for {Assembly}", _runnerId, _assembly);
            await StopAsync().ConfigureAwait(false);
            return false;
        }
    }

    public async Task<List<TestNode>> DiscoverTestsAsync()
    {
        if (!_isInitialized || _client is null)
        {
            throw new InvalidOperationException("Server not initialized. Call StartAsync first.");
        }

        var discoveryId = Guid.NewGuid();
        List<TestNodeUpdate> discoveredResults = [];

        var discoverTestsResponse = await _client.DiscoverTestsAsync(discoveryId, updates =>
        {
            discoveredResults.AddRange(updates);
            return Task.CompletedTask;
        }).ConfigureAwait(false);

        await discoverTestsResponse.WaitCompletionAsync().ConfigureAwait(false);

        return discoveredResults
            .Where(x => x.Node.ExecutionState is "discovered")
            .Select(x => x.Node)
            .ToList();
    }

    public async Task<List<TestNodeUpdate>> RunTestsAsync(TestNode[]? testsToRun)
    {
        var (results, _) = await RunTestsAsync(testsToRun, timeout: null).ConfigureAwait(false);
        return results;
    }

    public async Task<(List<TestNodeUpdate> Results, bool TimedOut)> RunTestsAsync(TestNode[]? testsToRun, TimeSpan? timeout)
    {
        if (!_isInitialized || _client is null)
        {
            throw new InvalidOperationException("Server not initialized. Call StartAsync first.");
        }

        var runId = Guid.NewGuid();
        var testResults = new System.Collections.Concurrent.ConcurrentBag<TestNodeUpdate>();

        var executeTestsResponse = await _client.RunTestsAsync(runId, updates =>
        {
            foreach (var update in updates)
            {
                testResults.Add(update);
            }
            return Task.CompletedTask;
        }, testsToRun).ConfigureAwait(false);

        if (timeout.HasValue)
        {
            var completed = await executeTestsResponse.WaitCompletionAsync(timeout.Value).ConfigureAwait(false);
            return (testResults.ToList(), !completed);
        }

        await executeTestsResponse.WaitCompletionAsync().ConfigureAwait(false);
        return (testResults.ToList(), false);
    }

    public async Task RestartAsync()
    {
        await StopAsync().ConfigureAwait(false);
        await StartAsync().ConfigureAwait(false);
    }

    public async Task StopAsync()
    {
        if (_client is not null)
        {
            try
            {
                await _client.ExitAsync().ConfigureAwait(false);
            }
            catch
            {
                // Ignore errors during graceful shutdown
            }
        }

        _listener?.Stop();
        _listener = null;
        _client?.Dispose();
        _client = null;
        _rpc?.Dispose();
        _rpc = null;
        await _stream?.DisposeAsync().ConfigureAwait(false);
        _stream = null;
        _tcpClient?.Dispose();
        _tcpClient = null;
        await _outputStream?.DisposeAsync().ConfigureAwait(false);
        _outputStream = null;
        _cliProcess = null;
        _isInitialized = false;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        StopAsync().GetAwaiter().GetResult();
    }
}


