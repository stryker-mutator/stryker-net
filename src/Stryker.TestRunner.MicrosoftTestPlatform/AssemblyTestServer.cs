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
    private readonly ITestServerConnectionFactory _connectionFactory;
    private ITestServerListener? _listener;
    private ITestServerProcess? _process;
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
        _connectionFactory = connectionFactory ?? new DefaultTestServerConnectionFactory(options);
    }

    public bool IsInitialized => _isInitialized;

    /// <summary>
    /// True when the server has been initialized and its underlying process is still running.
    /// A test host that crashed mid-run (e.g. a mutation causing a fatal fault such as a
    /// <see cref="StackOverflowException"/>) leaves <see cref="IsInitialized"/> true while the
    /// process is gone; this flag detects that so the server can be recreated instead of reused.
    /// </summary>
    public bool IsAlive => _isInitialized && !_disposed && _process is { HasExited: false };

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

            var tcpClient = await acceptTask.ConfigureAwait(false);
            _client = _connectionFactory.CreateClient(tcpClient, _process.ProcessHandle, _logger);

            await _client.InitializeAsync(cancellationToken).ConfigureAwait(false);
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

        List<TestNodeUpdate> discoveredResults = [];

        await _client.DiscoverTestsAsync(updates =>
        {
            discoveredResults.AddRange(updates);
            return Task.CompletedTask;
        }).ConfigureAwait(false);

        return discoveredResults
            .Where(x => x.Node.ExecutionState is TestNodeStates.Discovered)
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
            using var cancellationTokenSource = new CancellationTokenSource(timeout.Value);
            try
            {
                await _client.RunTestsAsync(onUpdate, testsToRun, cancellationTokenSource.Token).ConfigureAwait(false);
                return (testResults.ToList(), false);
            }
            catch (OperationCanceledException ex) when (cancellationTokenSource.IsCancellationRequested)
            {
                _logger.LogDebug(ex, "{RunnerId}: Test run RPC call timed out for {Assembly}", _runnerId, _assembly);
                return (testResults.ToList(), true);
            }
            catch
            {
                ThrowIfHostCrashed();
                throw;
            }
        }

        try
        {
            await _client.RunTestsAsync(onUpdate, testsToRun).ConfigureAwait(false);
            return (testResults.ToList(), false);
        }
        catch
        {
            ThrowIfHostCrashed();
            throw;
        }
    }

    /// <summary>
    /// Throws a <see cref="Stryker.TestRunner.TestHostCrashedException"/> when the test host process has exited before the
    /// run completed. A crashed host never sends a completion signal, so without this check the run would
    /// otherwise wait out the full timeout and be misreported as a timeout instead of a runtime error.
    /// </summary>
    private void ThrowIfHostCrashed()
    {
        if (_process is { HasExited: true })
        {
            _logger.LogDebug("{RunnerId}: Test host for {Assembly} exited unexpectedly during the test run", _runnerId, _assembly);
            throw new Stryker.TestRunner.TestHostCrashedException($"The test host for {_assembly} exited unexpectedly during the test run.");
        }
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
                // Bound both the exit notification and process shutdown so cleanup cannot hang indefinitely.
                var timeout = TimeSpan.FromSeconds(30);
                await _client.ExitAsync().WaitAsync(timeout).ConfigureAwait(false);
                // Coverage data must be flushed before disposing resources
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
