using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Options;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;

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
    private readonly ITestServerConnectionFactory _connectionFactory;
    private ITestServerListener? _listener;
    private ITestServerProcess? _process;
    private Stream? _stream;
    private IDisposable? _connection;
    private ITestingPlatformClient? _client;
    private bool _isInitialized;
    private bool _disposed;

    public AssemblyTestServer(
        string assembly,
        Dictionary<string, string?> environmentVariables,
        ILogger logger,
        string runnerId,
        IStrykerOptions? options = null,
        ITestServerConnectionFactory? connectionFactory = null)
    {
        _assembly = assembly;
        _environmentVariables = environmentVariables;
        _logger = logger;
        _runnerId = runnerId;
        _options = options;
        _connectionFactory = connectionFactory ?? new DefaultTestServerConnectionFactory(options);
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
            var (listener, port) = _connectionFactory.CreateListener();
            _listener = listener;

            _process = _connectionFactory.StartProcess(_assembly, port, _environmentVariables);

            var acceptTask = _listener.AcceptConnectionAsync(cancellationToken);
            var connectionTimeout = Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            var completedTask = await Task.WhenAny(_process.WaitForExitAsync(), acceptTask, connectionTimeout).ConfigureAwait(false);

            if (completedTask == connectionTimeout)
            {
                _logger.LogDebug("{RunnerId}: Timeout waiting for test server connection for {Assembly}", _runnerId, _assembly);
                await StopAsync().ConfigureAwait(false);
                return false;
            }

            if (_process.HasExited)
            {
                _logger.LogDebug("{RunnerId}: Test process exited prematurely for {Assembly}", _runnerId, _assembly);
                await StopAsync().ConfigureAwait(false);
                return false;
            }

            (_stream, _connection) = await acceptTask.ConfigureAwait(false);

            var rpcLogFilePath = BuildRpcLogFilePath();
            _client = _connectionFactory.CreateClient(_stream, _process.ProcessHandle, _logger, rpcLogFilePath);

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

        Func<TestNodeUpdate[], Task> onUpdate = updates =>
        {
            foreach (var update in updates)
            {
                testResults.Add(update);
            }
            return Task.CompletedTask;
        };

        if (timeout.HasValue)
        {
            var deadline = DateTime.UtcNow + timeout.Value;

            ResponseListener executeTestsResponse;
            try
            {
                // The RPC call itself can block when the server is stuck (e.g. infinite loop in mutated code)
                executeTestsResponse = await _client.RunTestsAsync(runId, onUpdate, testsToRun)
                    .WaitAsync(timeout.Value).ConfigureAwait(false);
            }
            catch (TimeoutException ex)
            {
                _logger.LogDebug(ex, "{RunnerId}: Test run RPC call timed out for {Assembly}", _runnerId, _assembly);
                return (testResults.ToList(), true);
            }

            var remaining = deadline - DateTime.UtcNow;
            if (remaining <= TimeSpan.Zero)
            {
                return (testResults.ToList(), true);
            }

            var completed = await executeTestsResponse.WaitCompletionAsync(remaining).ConfigureAwait(false);
            return (testResults.ToList(), !completed);
        }

        var response = await _client.RunTestsAsync(runId, onUpdate, testsToRun).ConfigureAwait(false);
        await response.WaitCompletionAsync().ConfigureAwait(false);
        return (testResults.ToList(), false);
    }

    public async Task RestartAsync(bool force = false)
    {
        await StopAsync(force).ConfigureAwait(false);
        await StartAsync().ConfigureAwait(false);
    }

    public async Task StopAsync(bool force = false)
    {
        if (force)
        {
            _logger.LogDebug("{RunnerId}: Force-killing test server process for {Assembly}", _runnerId, _assembly);
            _process?.ProcessHandle.Kill();
        }
        else if (_client is not null)
        {
            try
            {
                await _client.ExitAsync().ConfigureAwait(false);
                // Coverage data must be flushed before disposing resources
                var timeout = TimeSpan.FromSeconds(30);
                await _client.WaitServerProcessExitAsync().WaitAsync(timeout).ConfigureAwait(false);
            }
            catch (TimeoutException exception)
            {
                _logger.LogWarning(exception, "{RunnerId}: Test server process for {Assembly} did not exit within the expected time. Killing forcefully.", _runnerId, _assembly);
                _process?.ProcessHandle.Kill();
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "{RunnerId}: Test server process for {Assembly} could not be stopped gracefully.", _runnerId, _assembly);
            }
        }

        _listener?.Stop();
        _listener?.Dispose();
        _listener = null;
        _client?.Dispose();
        _client = null;
        _stream?.Dispose();
        _stream = null;
        _connection?.Dispose();
        _connection = null;
        try
        {
            _process?.Dispose();
        }
        catch (Exception)
        {
            // Process disposal can fail if kill/cleanup didn't complete in time
        }
        _process = null;
        _isInitialized = false;
    }

    /// <summary>
    /// Returns a per-instance RPC trace log path when log-to-file is enabled, or null otherwise.
    /// </summary>
    private string? BuildRpcLogFilePath()
    {
        if (_options?.LogOptions.LogToFile != true || string.IsNullOrEmpty(_options.OutputPath))
        {
            return null;
        }

        var logsDirectory = Path.Combine(_options.OutputPath, "logs", "test-servers");
        var fileName = $"rpc-{Path.GetFileNameWithoutExtension(_assembly)}-{_runnerId}-{DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}.log";
        return Path.Combine(logsDirectory, fileName);
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


