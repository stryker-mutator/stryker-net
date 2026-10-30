using Microsoft.Extensions.Logging;
using Microsoft.Testing.Platform.ServerMode.Client;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

internal sealed class TestingPlatformClient : ITestingPlatformClient
{
    private const string LocationType = "location.type";
    private const string LocationMethod = "location.method";
    private static readonly TimeSpan RequestTimeout = TimeSpan.FromMinutes(3);

    private readonly IMtpServerClient _client;
    private readonly IProcessHandle _processHandler;
    private readonly ILogger _logger;
    private readonly SemaphoreSlim _requestGate = new(1, 1);
    private readonly HashSet<Guid> _completedRuns = [];
    private bool _disposed;
    private bool _requestFailed;

    public TestingPlatformClient(IMtpServerClient client, IProcessHandle processHandler, ILogger logger)
    {
        _client = client;
        _processHandler = processHandler;
        _logger = logger;
        _client.LogReceived += OnLogReceived;
    }

    public int ExitCode => _processHandler.ExitCode;

    public async Task<int> WaitServerProcessExitAsync()
    {
        await _processHandler.WaitForExitAsync();
        return _processHandler.ExitCode;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        using var timeout = CreateRequestTimeout(cancellationToken);
        await ExecuteRequestAsync(
            async token => _ = await _client.InitializeAsync(token).ConfigureAwait(false),
            timeout.Token).ConfigureAwait(false);
    }

    public async Task ExitAsync(bool gracefully = true)
    {
        if (gracefully)
        {
            using var timeout = CreateRequestTimeout(CancellationToken.None);
            await ExecuteRequestAsync(_client.ExitAsync, timeout.Token).ConfigureAwait(false);
        }
        else
        {
            Dispose();
        }
    }

    public Task DiscoverTestsAsync(Func<TestNodeUpdate[], Task> action, CancellationToken cancellationToken = default)
        => CollectUpdatesAsync(action, token => _client.DiscoverTestsAsync(token), cancellationToken);

    public Task RunTestsAsync(Func<TestNodeUpdate[], Task> action, TestNode[]? testNodes = null, CancellationToken cancellationToken = default)
        => CollectUpdatesAsync(
            action,
            testNodes is null
                ? token => _client.RunTestsAsync(token)
                : token => _client.RunTestsAsync(testNodes.Select(test => test.Uid).ToArray(), token),
            cancellationToken);

    private async Task CollectUpdatesAsync(
        Func<TestNodeUpdate[], Task> action,
        Func<CancellationToken, Task> request,
        CancellationToken cancellationToken)
    {
        await _requestGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        var callbacks = new List<Task>();
        Guid? runId = null;
        var collecting = true;
        Task previousCallback = Task.CompletedTask;
        Exception? requestException = null;
        void OnTestNodesUpdated(object? _, MtpTestNodeUpdateEventArgs eventArgs)
        {
            lock (callbacks)
            {
                if (!collecting)
                {
                    return;
                }
                // The source client owns the run ID. A failed request is never reused; for completed
                // requests, reject a previously observed ID and batches from a different run.
                if (eventArgs.RunId != Guid.Empty
                    && (_completedRuns.Contains(eventArgs.RunId) || runId is not null && runId != eventArgs.RunId))
                {
                    _logger.LogWarning("Ignoring MTP updates for an inactive run {RunId}.", eventArgs.RunId);
                    return;
                }

                if (eventArgs.RunId != Guid.Empty)
                {
                    runId ??= eventArgs.RunId;
                }
                if (eventArgs.Changes.Count > 0)
                {
                    try
                    {
                        var updates = eventArgs.Changes.Select(ToTestNodeUpdate).ToArray();
                        var precedingCallback = previousCallback;
                        // Reporting must not block the ordered transport read loop.
                        previousCallback = Task.Run(async () =>
                        {
                            await precedingCallback.ConfigureAwait(false);
                            await action(updates).ConfigureAwait(false);
                        });
                        callbacks.Add(previousCallback);
                    }
                    catch (Exception exception)
                    {
                        callbacks.Add(Task.FromException(exception));
                    }
                }
            }
        }

        _client.TestNodesUpdated += OnTestNodesUpdated;
        try
        {
            ThrowIfUnusable();
            await request(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            // Cancellation completes locally, not at the server's terminal response. No subsequent
            // request may attach a new result handler to this connection.
            _requestFailed = true;
            requestException = exception;
            throw;
        }
        finally
        {
            _client.TestNodesUpdated -= OnTestNodesUpdated;
            Task[] pendingCallbacks;
            lock (callbacks)
            {
                collecting = false;
                pendingCallbacks = callbacks.ToArray();
            }
            if (runId is not null)
            {
                _completedRuns.Add(runId.Value);
            }
            var callbackDrain = Task.WhenAll(pendingCallbacks);
            try
            {
                await callbackDrain.WaitAsync(RequestTimeout).ConfigureAwait(false);
            }
            catch (TimeoutException exception) when (!callbackDrain.IsCompleted)
            {
                _requestFailed = true;
                throw new TestHostTerminationException("MTP result processing did not drain within the request deadline. Mutation testing cannot safely continue.",
                    exception);
            }
            catch (Exception exception) when (requestException is not null)
            {
                _requestFailed = true;
                _logger.LogError(exception, "An MTP result callback failed while the request was already failing.");
            }
            catch
            {
                _requestFailed = true;
                throw;
            }
            finally
            {
                _requestGate.Release();
            }
        }
    }

    private async Task ExecuteRequestAsync(Func<CancellationToken, Task> request, CancellationToken cancellationToken)
    {
        await _requestGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfUnusable();
            await request(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            _requestFailed = true;
            throw;
        }
        finally
        {
            _requestGate.Release();
        }
    }

    private void ThrowIfUnusable()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_requestFailed)
        {
            throw new InvalidOperationException("The previous MTP request did not complete. Discard this test host before running more tests.");
        }
    }

    private static CancellationTokenSource CreateRequestTimeout(CancellationToken cancellationToken)
    {
        var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(RequestTimeout);
        return timeout;
    }

    private static TestNodeUpdate ToTestNodeUpdate(MtpTestNodeUpdate update)
    {
        var uid = update.Uid ?? throw new InvalidOperationException("The MTP server returned a test node without a UID.");
        var displayName = update.DisplayName ?? uid;
        var executionState = update.ExecutionState
            ?? throw new InvalidOperationException($"The MTP server returned test node '{uid}' without an execution state.");

        var node = new TestNode(
            uid,
            displayName,
            update.NodeType ?? string.Empty,
            executionState,
            update.FilePath,
            update.LineStart,
            update.LineEnd,
            GetString(update.Node, LocationType),
            GetString(update.Node, LocationMethod),
            GetRetryAttempt(update.Node),
            update.Node.TryGetValue("retry.is-superseded", out var superseded) ? superseded as bool? : null);

        return new TestNodeUpdate(node, update.ParentUid ?? string.Empty);
    }

    private static string? GetString(IReadOnlyDictionary<string, object?> properties, string key)
        => properties.TryGetValue(key, out var value) ? value as string : null;

    private static int? GetRetryAttempt(IReadOnlyDictionary<string, object?> properties)
        => properties.TryGetValue("retry.attempt", out var value)
            ? value switch
            {
                int attempt => attempt,
                long attempt when attempt is >= 1 and <= int.MaxValue => (int)attempt,
                double attempt when attempt is >= 1 and <= int.MaxValue && attempt == Math.Truncate(attempt) => (int)attempt,
                _ => null
            }
            : null;

    private void OnLogReceived(object? sender, MtpLogEventArgs eventArgs)
        => _logger.LogDebug(
            "MTP server {MtpServerLogLevel}: {MtpServerMessage}",
            eventArgs.Level,
            eventArgs.Message);

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _client.LogReceived -= OnLogReceived;
        _client.Dispose();
    }
}
