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
    private bool _disposed;

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
            _client.Dispose();
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
        var callbackLock = new object();
        var callbacks = new List<Task>();
        void OnTestNodesUpdated(object? _, MtpTestNodeUpdateEventArgs eventArgs)
        {
            if (eventArgs.Changes.Count > 0)
            {
                var callback = action(eventArgs.Changes.Select(ToTestNodeUpdate).ToArray());
                lock (callbackLock)
                {
                    callbacks.Add(callback);
                }
            }
        }

        _client.TestNodesUpdated += OnTestNodesUpdated;
        try
        {
            await request(cancellationToken).ConfigureAwait(false);
            await AwaitCallbacksAsync(callbacks, callbackLock, logger: _logger).ConfigureAwait(false);
        }
        finally
        {
            _client.TestNodesUpdated -= OnTestNodesUpdated;
            _requestGate.Release();
        }
    }

    /// <summary>
    /// Waits for the callbacks the request raised. The MTP client invokes every
    /// <see cref="IMtpServerClient.TestNodesUpdated"/> handler of a request before that request completes
    /// (it reads the server's messages in order and finishes a request only after the terminal response),
    /// so the list is complete by now and no settle delay is needed: only the asynchronous work the handlers
    /// started has to finish.
    /// </summary>
    /// <param name="callbacks">The tasks returned by the update handlers, guarded by <paramref name="callbackLock"/>.</param>
    /// <param name="callbackLock">The lock protecting <paramref name="callbacks"/>.</param>
    /// <param name="maxWaitMs">Upper bound for the wait, so a callback that never completes cannot hang a run.</param>
    /// <param name="logger">Receives a warning when the bound is reached, before the failure is raised.</param>
    /// <exception cref="TimeoutException">Pending callbacks did not complete within <paramref name="maxWaitMs"/>; the run's
    /// results may be missing updates, so the caller must treat the request as failed instead of trusting a partial verdict.</exception>
    internal static async Task AwaitCallbacksAsync(List<Task> callbacks, object callbackLock, int maxWaitMs = 5_000, ILogger? logger = null)
    {
        Task[] batch;
        lock (callbackLock)
        {
            batch = callbacks.ToArray();
            callbacks.Clear();
        }

        if (batch.Length == 0)
        {
            return;
        }

        var batchCompletion = Task.WhenAll(batch);
        if (await Task.WhenAny(batchCompletion, Task.Delay(maxWaitMs)).ConfigureAwait(false) != batchCompletion)
        {
            logger?.LogWarning("{PendingCount} test update callback(s) did not complete within {MaxWaitMs} ms; their results may be missing from this run",
                batch.Count(task => !task.IsCompleted), maxWaitMs);
            throw new TimeoutException($"{batch.Count(task => !task.IsCompleted)} test update callback(s) did not complete within {maxWaitMs} ms; the test run result is incomplete.");
        }

        // Surfaces a failing callback.
        await batchCompletion.ConfigureAwait(false);
    }
    private async Task ExecuteRequestAsync(Func<CancellationToken, Task> request, CancellationToken cancellationToken)
    {
        await _requestGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await request(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _requestGate.Release();
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
            update.ErrorMessage,
            update.ErrorStackTrace);

        return new TestNodeUpdate(node, update.ParentUid ?? string.Empty);
    }

    private static string? GetString(IReadOnlyDictionary<string, object?> properties, string key)
        => properties.TryGetValue(key, out var value) ? value as string : null;

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

        _client.LogReceived -= OnLogReceived;
        _client.Dispose();
        _requestGate.Dispose();
        _disposed = true;
    }
}
