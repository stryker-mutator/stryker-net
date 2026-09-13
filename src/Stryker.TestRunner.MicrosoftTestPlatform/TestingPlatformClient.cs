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
        var callbacks = new List<Task>();
        void OnTestNodesUpdated(object? _, MtpTestNodeUpdateEventArgs eventArgs)
        {
            if (eventArgs.Changes.Count > 0)
            {
                callbacks.Add(action(eventArgs.Changes.Select(ToTestNodeUpdate).ToArray()));
            }
        }

        _client.TestNodesUpdated += OnTestNodesUpdated;
        try
        {
            await request(cancellationToken).ConfigureAwait(false);
            await Task.WhenAll(callbacks).ConfigureAwait(false);
        }
        finally
        {
            _client.TestNodesUpdated -= OnTestNodesUpdated;
            _requestGate.Release();
        }
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
            GetString(update.Node, LocationMethod));

        return new TestNodeUpdate(node, update.ParentUid ?? string.Empty);
    }

    private static string? GetString(IReadOnlyDictionary<string, object?> properties, string key)
        => properties.TryGetValue(key, out var value) ? value as string : null;

    private void OnLogReceived(object? sender, MtpLogEventArgs eventArgs)
    {
        var logLevel = eventArgs.Level.ToLowerInvariant() switch
        {
            "trace" => Microsoft.Extensions.Logging.LogLevel.Trace,
            "debug" => Microsoft.Extensions.Logging.LogLevel.Debug,
            "information" or "info" => Microsoft.Extensions.Logging.LogLevel.Information,
            "warning" or "warn" => Microsoft.Extensions.Logging.LogLevel.Warning,
            "error" => Microsoft.Extensions.Logging.LogLevel.Error,
            "critical" => Microsoft.Extensions.Logging.LogLevel.Critical,
            _ => Microsoft.Extensions.Logging.LogLevel.Debug
        };

        _logger.Log(logLevel, "{MtpServerMessage}", eventArgs.Message);
    }

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
