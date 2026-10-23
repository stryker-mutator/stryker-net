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
    private readonly TimeSpan _shutdownTimeout;
    private ITestServerListener? _listener;
    private ITestServerProcess? _process;
    private ITestingPlatformClient? _client;
    private bool _isInitialized;
    private bool _requestFailed;
    private bool _disposed;
    internal static readonly TimeSpan ShutdownTimeout = TimeSpan.FromSeconds(5);

    public AssemblyTestServer(
        string assembly,
        Dictionary<string, string?> environmentVariables,
        ILogger logger,
        string runnerId,
        IStrykerOptions? options = null,
        ITestServerConnectionFactory? connectionFactory = null,
        TimeSpan? shutdownTimeout = null)
    {
        _assembly = assembly;
        _environmentVariables = environmentVariables;
        _logger = logger;
        _runnerId = runnerId;
        _connectionFactory = connectionFactory ?? new DefaultTestServerConnectionFactory(options);
        _shutdownTimeout = shutdownTimeout ?? ShutdownTimeout;
    }

    public bool IsInitialized => _isInitialized;
    internal int? ProcessId => _process?.ProcessHandle.Id;
    internal IReadOnlyCollection<TestNodeUpdate> LastRunResults { get; private set; } = [];

    /// <summary>
    /// True when the server has been initialized and its underlying process is still running.
    /// A test host that crashed mid-run (e.g. a mutation causing a fatal fault such as a
    /// <see cref="StackOverflowException"/>) leaves <see cref="IsInitialized"/> true while the
    /// process is gone; this flag detects that so the server can be recreated instead of reused.
    /// </summary>
    public bool IsAlive => _isInitialized && !_requestFailed && !_disposed && _process is { HasExited: false };

    public async Task<bool> StartAsync(CancellationToken cancellationToken = default)
    {
        if (_isInitialized && !_requestFailed)
        {
            return true;
        }

        if (_process is not null)
        {
            await StopAsync(force: true).ConfigureAwait(false);
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var (listener, port) = _connectionFactory.CreateListener();
            _listener = listener;

            _process = _connectionFactory.StartProcess(_assembly, port, _environmentVariables);

            var acceptTask = _listener.AcceptConnectionAsync(cancellationToken);
            var connectionTimeout = Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            var completedTask = await Task.WhenAny(_process.WaitForExitAsync(), acceptTask, connectionTimeout).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            if (completedTask == connectionTimeout)
            {
                _logger.LogDebug("{RunnerId}: Timeout waiting for test server connection for {Assembly}", _runnerId, _assembly);
                await StopAsync(force: true).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                return false;
            }

            if (_process.HasExited)
            {
                _logger.LogDebug("{RunnerId}: Test process exited prematurely for {Assembly}", _runnerId, _assembly);
                await StopAsync().ConfigureAwait(false);
                return false;
            }

            var tcpClient = await acceptTask.ConfigureAwait(false);
            try
            {
                _client = _connectionFactory.CreateClient(tcpClient, _process.ProcessHandle, _logger);
            }
            catch
            {
                tcpClient.Dispose();
                throw;
            }

            await _client.InitializeAsync(cancellationToken).ConfigureAwait(false);
            _isInitialized = true;
            _requestFailed = false;

            _logger.LogDebug("{RunnerId}: Test server started successfully for {Assembly}", _runnerId, _assembly);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "{RunnerId}: Failed to start test server for {Assembly}", _runnerId, _assembly);
            await StopAsync(force: true).ConfigureAwait(false);
            throw;
        }
    }

    public Task<List<TestNode>> DiscoverTestsAsync() => DiscoverTestsAsync(CancellationToken.None);

    public async Task<List<TestNode>> DiscoverTestsAsync(CancellationToken cancellationToken)
    {
        if (!_isInitialized || _client is null)
        {
            throw new InvalidOperationException("Server not initialized. Call StartAsync first.");
        }

        List<TestNodeUpdate> discoveredResults = [];

        try
        {
            await _client.DiscoverTestsAsync(updates =>
            {
                discoveredResults.AddRange(updates);
                return Task.CompletedTask;
            }, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            _requestFailed = true;
            await StopAsync(force: true).ConfigureAwait(false);
            throw;
        }

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

    public async Task<(List<TestNodeUpdate> Results, bool TimedOut)> RunTestsAsync(
        TestNode[]? testsToRun,
        TimeSpan? timeout,
        Func<IReadOnlyCollection<TestNodeUpdate>, bool>? shouldBail = null,
        CancellationToken cancellationToken = default)
    {
        LastRunResults = [];
        if (!_isInitialized || _client is null)
        {
            throw new InvalidOperationException("Server not initialized. Call StartAsync first.");
        }

        using var timeoutSource = new CancellationTokenSource();
        if (timeout.HasValue)
        {
            timeoutSource.CancelAfter(timeout.Value);
        }
        using var bailSource = new CancellationTokenSource();
        using var requestSource = CancellationTokenSource.CreateLinkedTokenSource(
            timeoutSource.Token, bailSource.Token, cancellationToken);
        var selectedIds = testsToRun?.Select(test => test.Uid).ToHashSet();
        var testResults = new Dictionary<string, TestNodeUpdate>();

        Func<TestNodeUpdate[], Task> onUpdate = updates =>
        {
            foreach (var update in updates)
            {
                if (selectedIds is not null && !selectedIds.Contains(update.Node.Uid))
                {
                    continue;
                }

                if (testResults.TryGetValue(update.Node.Uid, out var previous)
                    && ((previous.Node.RetryAttempt ?? 0) > (update.Node.RetryAttempt ?? 0)
                        || previous.Node.RetryAttempt == update.Node.RetryAttempt
                        && previous.Node.RetryIsSuperseded == false && update.Node.RetryIsSuperseded != false))
                {
                    continue;
                }
                testResults[update.Node.Uid] = update;
            }

            if (!requestSource.IsCancellationRequested && shouldBail?.Invoke(testResults.Values.ToArray()) == true)
            {
                bailSource.Cancel();
            }
            return Task.CompletedTask;
        };

        try
        {
            await _client.RunTestsAsync(onUpdate, testsToRun, requestSource.Token).ConfigureAwait(false);
        }
        catch (TestHostTerminationException)
        {
            _requestFailed = true;
            throw;
        }
        catch (OperationCanceledException) when (requestSource.IsCancellationRequested)
        {
            _requestFailed = true;
            // The official client cancels its wait before server execution has stopped. Only verified
            // process termination permits the runner to change its shared active-mutant control file.
            await StopAsync(force: true).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
        }
        catch
        {
            _requestFailed = true;
            LastRunResults = testResults.Values.Where(update => update.Node.RetryIsSuperseded != true
                && !TestNodeStates.IsCancellation(update.Node.ExecutionState)).ToArray();
            ThrowIfHostCrashed();
            throw;
        }

        var stopped = requestSource.IsCancellationRequested;
        if (stopped && _process is not null)
        {
            await StopAsync(force: true).ConfigureAwait(false);
        }
        cancellationToken.ThrowIfCancellationRequested();
        var results = testResults.Values
            .Where(update => update.Node.RetryIsSuperseded != true
                && (!stopped || !TestNodeStates.IsCancellation(update.Node.ExecutionState)))
            .ToList();
        LastRunResults = results;
        var timedOut = timeoutSource.IsCancellationRequested && !bailSource.IsCancellationRequested;
        if (timedOut)
        {
            _logger.LogDebug("{RunnerId}: Test run timed out for {Assembly}; cancelled host discarded.", _runnerId, _assembly);
        }
        return (results, timedOut);
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
        var requestFailed = _requestFailed;
        _requestFailed = true;
        var process = _process;
        try
        {
            if (force || requestFailed)
            {
                _logger.LogDebug("{RunnerId}: Force-killing test server process for {Assembly}", _runnerId, _assembly);
                process?.ProcessHandle.Kill();
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
                    process?.ProcessHandle.Kill();
                }
            }
            else if (process is { HasExited: false })
            {
                process.ProcessHandle.Kill();
            }

            if (process is not null)
            {
                try
                {
                    await process.WaitForExitAsync().WaitAsync(_shutdownTimeout).ConfigureAwait(false);
                }
                catch (Exception exception) when (process.HasExited)
                {
                    // CliWrap reports a nonzero host exit as a fault even when termination succeeded.
                    _logger.LogDebug(exception, "{RunnerId}: Test server process for {Assembly} exited during shutdown.", _runnerId, _assembly);
                }
                if (!process.HasExited)
                {
                    throw new TestHostTerminationException(
                        $"The test host for {_assembly} has not terminated. Its mutant control file cannot be reused.",
                        new TimeoutException());
                }
            }
        }
        catch (InvalidOperationException exception) when (process is { HasExited: true })
        {
            _logger.LogDebug(exception, "{RunnerId}: Test server process for {Assembly} exited while shutdown was requested.", _runnerId, _assembly);
        }
        catch (Exception exception) when (process is { HasExited: false } && exception is not TestHostTerminationException)
        {
            throw new TestHostTerminationException(
                $"The test host for {_assembly} could not be terminated. Mutation testing cannot safely continue.", exception);
        }
        finally
        {
            _listener?.Stop();
            _listener?.Dispose();
            _listener = null;
            _client?.Dispose();
            _client = null;
            if (process is null || process.HasExited)
            {
                try
                {
                    _process?.Dispose();
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(exception, "{RunnerId}: Failed to dispose the stopped test process for {Assembly}.", _runnerId, _assembly);
                }
                _process = null;
            }
            _isInitialized = false;
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        StopAsync().GetAwaiter().GetResult();
        _disposed = true;
    }
}
