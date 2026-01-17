using System.Net;
using System.Net.Sockets;
using CliWrap;
using Microsoft.Extensions.Logging;
using MsTestRunnerDemo;
using MsTestRunnerDemo.Models;
using StreamJsonRpc;
using Stryker.Abstractions;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;
using static Stryker.Abstractions.Testing.ITestRunner;

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

    private TcpListener? _listener;
    private TcpClient? _tcpClient;
    private NetworkStream? _stream;
    private JsonRpc? _rpc;
    private TestingPlatformClient? _client;
    private CommandTask<CommandResult>? _cliProcess;
    private MemoryStream? _outputStream;
    private bool _isInitialized;
    private bool _disposed;

    public AssemblyTestServer(string assembly, Dictionary<string, string?> environmentVariables, ILogger logger, string runnerId)
    {
        _assembly = assembly;
        _environmentVariables = environmentVariables;
        _logger = logger;
        _runnerId = runnerId;
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

            _cliProcess = Cli.Wrap("dotnet")
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

            _client = new TestingPlatformClient(_rpc, _tcpClient, new ProcessHandle(_cliProcess, _outputStream), enableDiagnostic: false);

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
        List<TestNodeUpdate> testResults = [];

        var executeTestsResponse = await _client.RunTestsAsync(runId, updates =>
        {
            testResults.AddRange(updates);
            return Task.CompletedTask;
        }, testsToRun).ConfigureAwait(false);

        if (timeout.HasValue)
        {
            var completed = await executeTestsResponse.WaitCompletionAsync(timeout.Value).ConfigureAwait(false);
            return (testResults, !completed);
        }

        await executeTestsResponse.WaitCompletionAsync().ConfigureAwait(false);
        return (testResults, false);
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
        _stream?.Dispose();
        _stream = null;
        _tcpClient?.Dispose();
        _tcpClient = null;
        _outputStream?.Dispose();
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

/// <summary>
/// Individual test runner instance that handles test execution with mutation-specific
/// environment variables. Used by MicrosoftTestPlatformRunnerPool.
/// Maintains persistent test server connections per assembly to reduce process startup overhead.
/// Uses file-based mutant control to allow changing the active mutant without restarting processes.
/// </summary>
internal sealed class SingleMicrosoftTestPlatformRunner : IDisposable
{
    private readonly int _id;
    private readonly Dictionary<string, List<TestNode>> _testsByAssembly;
    private readonly Dictionary<string, MtpTestDescription> _testDescriptions;
    private readonly TestSet _testSet;
    private readonly object _discoveryLock;
    private readonly ILogger _logger;
    private readonly string _mutantFilePath;

    private readonly Dictionary<string, AssemblyTestServer> _assemblyServers = new();
    private readonly object _serverLock = new();
    private bool _disposed;

    private string RunnerId => $"MtpRunner-{_id}";

    public SingleMicrosoftTestPlatformRunner(
        int id,
        Dictionary<string, List<TestNode>> testsByAssembly,
        Dictionary<string, MtpTestDescription> testDescriptions,
        TestSet testSet,
        object discoveryLock,
        ILogger logger)
    {
        _id = id;
        _testsByAssembly = testsByAssembly;
        _testDescriptions = testDescriptions;
        _testSet = testSet;
        _discoveryLock = discoveryLock;
        _logger = logger;

        // Create a unique file path for this runner to communicate the active mutant ID
        _mutantFilePath = Path.Combine(Path.GetTempPath(), $"stryker-mutant-{_id}.txt");

        // Initialize with no active mutation
        WriteMutantIdToFile(-1);
    }

    public Task<bool> DiscoverTestsAsync(string assembly)
    {
        return DiscoverTestsInternalAsync(assembly);
    }

    public Task<ITestRunResult> InitialTestAsync(IProjectAndTests project)
    {
        var assemblies = project.GetTestAssemblies();
        return RunAllTestsAsync(assemblies, mutantId: -1, mutants: null, update: null);
    }

    public Task<ITestRunResult> TestMultipleMutantsAsync(
        IProjectAndTests project,
        ITimeoutValueCalculator? timeoutCalc,
        IReadOnlyList<IMutant> mutants,
        TestUpdateHandler? update)
    {
        var assemblies = project.GetTestAssemblies();

        // Determine which mutant to activate
        // When testing a single mutant, activate it; otherwise use -1 (no mutation)
        var mutantId = mutants.Count == 1 ? mutants[0].Id : -1;

        _logger.LogDebug("{RunnerId}: Testing mutant(s) [{Mutants}] with active mutation ID: {MutantId}",
            RunnerId, string.Join(",", mutants.Select(m => m.Id)), mutantId);

        return RunAllTestsAsync(assemblies, mutantId, mutants, update, timeoutCalc);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        lock (_serverLock)
        {
            foreach (var server in _assemblyServers.Values)
            {
                server.Dispose();
            }
            _assemblyServers.Clear();
        }

        // Clean up the mutant file
        try
        {
            if (File.Exists(_mutantFilePath))
            {
                File.Delete(_mutantFilePath);
            }
        }
        catch
        {
            // Ignore cleanup errors
        }
    }

    private void WriteMutantIdToFile(int mutantId)
    {
        try
        {
            File.WriteAllText(_mutantFilePath, mutantId.ToString());
            _logger.LogDebug("{RunnerId}: Wrote mutant ID {MutantId} to file {FilePath}",
                RunnerId, mutantId, _mutantFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "{RunnerId}: Failed to write mutant ID to file {FilePath}",
                RunnerId, _mutantFilePath);
        }
    }

    private Dictionary<string, string?> BuildEnvironmentVariables()
    {
        // Use file-based mutant control for process reuse
        return new Dictionary<string, string?>
        {
            ["STRYKER_MUTANT_FILE"] = _mutantFilePath
        };
    }

    private async Task<AssemblyTestServer> GetOrCreateServerAsync(string assembly)
    {
        AssemblyTestServer? server;
        lock (_serverLock)
        {
            if (_assemblyServers.TryGetValue(assembly, out server) && server.IsInitialized)
            {
                return server;
            }
        }

        var environmentVariables = BuildEnvironmentVariables();
        server = new AssemblyTestServer(assembly, environmentVariables, _logger, RunnerId);

        var started = await server.StartAsync().ConfigureAwait(false);
        if (!started)
        {
            throw new InvalidOperationException($"Failed to start test server for {assembly}");
        }

        lock (_serverLock)
        {
            _assemblyServers[assembly] = server;
        }

        return server;
    }

    private async Task<bool> DiscoverTestsInternalAsync(string assembly)
    {
        try
        {
            var server = await GetOrCreateServerAsync(assembly).ConfigureAwait(false);
            var tests = await server.DiscoverTestsAsync().ConfigureAwait(false);

            lock (_discoveryLock)
            {
                _testsByAssembly[assembly] = tests;

                foreach (var test in tests)
                {
                    if (!_testDescriptions.ContainsKey(test.Uid))
                    {
                        var mtpTestDescription = new MtpTestDescription(test);
                        _testDescriptions[test.Uid] = mtpTestDescription;
                        _testSet.RegisterTest(mtpTestDescription.Description);
                    }
                }
            }

            _logger.LogDebug("{RunnerId}: Discovered {TestCount} tests in {Assembly}", RunnerId, tests.Count, assembly);
            return tests.Count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "{RunnerId}: Failed to discover tests in {Assembly}", RunnerId, assembly);
            return false;
        }
    }

    private async Task<ITestRunResult> RunAllTestsAsync(
        IReadOnlyList<string> assemblies,
        int mutantId,
        IReadOnlyList<IMutant>? mutants,
        TestUpdateHandler? update,
        ITimeoutValueCalculator? timeoutCalc = null)
    {
        try
        {
            // Update the mutant file before running tests
            WriteMutantIdToFile(mutantId);

            var allExecutedTests = new List<string>();
            var allFailedTests = new List<string>();
            var allTimedOutTests = new List<string>();
            var allMessages = new List<string>();
            var totalDuration = TimeSpan.Zero;
            var errorMessages = new List<string>();
            var totalDiscoveredTests = 0;
            var totalExecutedTests = 0;

            foreach (var assembly in assemblies)
            {
                if (!File.Exists(assembly))
                {
                    continue;
                }

                List<TestNode>? discoveredTests = null;
                lock (_discoveryLock)
                {
                    if (_testsByAssembly.TryGetValue(assembly, out var tests))
                    {
                        discoveredTests = tests;
                        totalDiscoveredTests += tests.Count;
                    }
                }

                // Calculate timeout based on expected test duration
                TimeSpan? timeout = null;
                if (timeoutCalc is not null && discoveredTests is not null)
                {
                    var estimatedTimeMs = (int)discoveredTests
                        .Where(t => _testDescriptions.TryGetValue(t.Uid, out _))
                        .Sum(t => _testDescriptions[t.Uid].InitialRunTime.TotalMilliseconds);
                    var timeoutMs = timeoutCalc.CalculateTimeoutValue(estimatedTimeMs);
                    timeout = TimeSpan.FromMilliseconds(timeoutMs);
                    _logger.LogDebug("{RunnerId}: Using {TimeoutMs} ms as test run timeout for {Assembly}",
                        RunnerId, timeoutMs, Path.GetFileName(assembly));
                }

                var (testResults, timedOut) = await RunTestsInternalAsync(assembly, null, mutants, update, timeout).ConfigureAwait(false);

                if (timedOut)
                {
                    _logger.LogDebug("{RunnerId}: Test run timed out for {Assembly}", RunnerId, Path.GetFileName(assembly));

                    // Mark all tests from this assembly as timed out
                    if (discoveredTests is not null)
                    {
                        allTimedOutTests.AddRange(discoveredTests.Select(t => t.Uid));
                    }

                    // Restart the server since the test process is likely stuck
                    AssemblyTestServer? server;
                    lock (_serverLock)
                    {
                        _assemblyServers.TryGetValue(assembly, out server);
                    }
                    if (server is not null)
                    {
                        _logger.LogDebug("{RunnerId}: Restarting test server for {Assembly} after timeout", RunnerId, Path.GetFileName(assembly));
                        await server.RestartAsync().ConfigureAwait(false);
                    }
                }

                if (testResults is TestRunResult result)
                {
                    if (result.ExecutedTests.IsEveryTest)
                    {
                        totalExecutedTests += discoveredTests?.Count ?? 0;
                    }
                    else
                    {
                        var executedIds = result.ExecutedTests.GetIdentifiers().ToList();
                        allExecutedTests.AddRange(executedIds);
                        totalExecutedTests += executedIds.Count;
                    }

                    allFailedTests.AddRange(result.FailingTests.GetIdentifiers());
                    totalDuration += result.Duration;
                    allMessages.AddRange(result.Messages);
                    if (!string.IsNullOrWhiteSpace(result.ResultMessage))
                    {
                        errorMessages.Add(result.ResultMessage);
                    }
                }
            }

            var executedTests = totalDiscoveredTests > 0 && totalExecutedTests >= totalDiscoveredTests
                ? TestIdentifierList.EveryTest()
                : new TestIdentifierList(allExecutedTests);

            var failedTestIds = allFailedTests.Count > 0
                ? new TestIdentifierList(allFailedTests)
                : TestIdentifierList.NoTest();

            var timedOutTestIds = allTimedOutTests.Count > 0
                ? new TestIdentifierList(allTimedOutTests)
                : TestIdentifierList.NoTest();

            IEnumerable<MtpTestDescription> testDescriptionValues;
            lock (_discoveryLock)
            {
                testDescriptionValues = _testDescriptions.Values.ToList();
            }

            return new TestRunResult(
                testDescriptionValues,
                executedTests,
                failedTestIds,
                timedOutTestIds,
                string.Join(Environment.NewLine, errorMessages),
                allMessages,
                totalDuration);
        }
        catch (Exception ex)
        {
            return new TestRunResult(false, ex.Message);
        }
    }

    private async Task<(ITestRunResult Result, bool TimedOut)> RunTestsInternalAsync(
        string assembly,
        Func<TestNode, bool>? testUidFilter,
        IReadOnlyList<IMutant>? mutants,
        TestUpdateHandler? update,
        TimeSpan? timeout = null)
    {
        var startTime = DateTime.UtcNow;

        try
        {
            // Get or create the server for this assembly (reuses existing server)
            var server = await GetOrCreateServerAsync(assembly).ConfigureAwait(false);

            List<TestNode>? tests = null;
            lock (_discoveryLock)
            {
                if (_testsByAssembly.TryGetValue(assembly, out var assemblyTests))
                {
                    tests = assemblyTests;
                }
            }

            var testsToRun = tests?.Where(t => testUidFilter is null || testUidFilter(t)).ToArray();

            var (testResults, timedOut) = await server.RunTestsAsync(testsToRun, timeout).ConfigureAwait(false);

            var duration = DateTime.UtcNow - startTime;
            var finishedTests = testResults.Where(x => x.Node.ExecutionState is not "in-progress").ToList();
            var failedTests = finishedTests.Where(x => x.Node.ExecutionState is "failed").Select(x => x.Node.Uid).ToList();

            lock (_discoveryLock)
            {
                foreach (var testResult in finishedTests)
                {
                    if (_testDescriptions.TryGetValue(testResult.Node.Uid, out var testDescription))
                    {
                        testDescription.RegisterInitialTestResult(new MtpTestResult(duration));
                    }
                }
            }

            var errorMessagesStr = string.Join(Environment.NewLine,
                finishedTests.Where(x => x.Node.ExecutionState is "failed")
                    .Select(x => $"{x.Node.DisplayName}{Environment.NewLine}{Environment.NewLine}Test failed"));

            var messages = finishedTests.Select(x =>
                $"{x.Node.DisplayName}{Environment.NewLine}{Environment.NewLine}State: {x.Node.ExecutionState}");

            var totalDiscoveredTests = tests?.Count ?? 0;
            var executedTestCount = finishedTests.Count;
            var executedTests = totalDiscoveredTests > 0 && executedTestCount >= totalDiscoveredTests
                ? TestIdentifierList.EveryTest()
                : new TestIdentifierList(finishedTests.Select(x => x.Node.Uid));

            var failedTestIds = failedTests.Count > 0
                ? new TestIdentifierList(failedTests)
                : TestIdentifierList.NoTest();

            if (update is not null && mutants is not null)
            {
                var timedOutTests = timedOut ? new TestIdentifierList(tests?.Select(t => t.Uid) ?? []) : TestIdentifierList.NoTest();
                update.Invoke(mutants, failedTestIds, executedTests, timedOutTests);
            }

            IEnumerable<MtpTestDescription> testDescriptionValues;
            lock (_discoveryLock)
            {
                testDescriptionValues = _testDescriptions.Values.ToList();
            }

            var result = new TestRunResult(
                testDescriptionValues,
                executedTests,
                failedTestIds,
                TestIdentifierList.NoTest(),
                errorMessagesStr,
                messages,
                duration);

            return (result, timedOut);
        }
        catch (Exception ex)
        {
            return (new TestRunResult(false, ex.Message), false);
        }
    }
}


