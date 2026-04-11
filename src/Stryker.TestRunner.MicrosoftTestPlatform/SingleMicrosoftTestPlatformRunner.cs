using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;
using static Stryker.Abstractions.Testing.ITestRunner;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

/// <summary>
/// Individual test runner instance that handles test execution with mutation-specific
/// environment variables. Used by MicrosoftTestPlatformRunnerPool.
/// Maintains persistent test server connections per assembly to reduce process startup overhead.
/// Uses file-based mutant control to allow changing the active mutant without restarting processes.
/// </summary>
public class SingleMicrosoftTestPlatformRunner : IDisposable
{
    private readonly int _id;
    private readonly Dictionary<string, List<TestNode>> _testsByAssembly;
    private readonly Dictionary<string, MtpTestDescription> _testDescriptions;
    private readonly TestSet _testSet;
    private readonly object _discoveryLock;
    private readonly ILogger _logger;
    private readonly string _mutantFilePath;
    private readonly string _coverageFilePath;
    private readonly IStrykerOptions? _options;

    private readonly Dictionary<string, AssemblyTestServer> _assemblyServers = new();
    private readonly SemaphoreSlim _serverLock = new(1, 1);
    private bool _disposed;
    private bool _coverageMode;

    private readonly string _runnerId;

    public SingleMicrosoftTestPlatformRunner(
        int id,
        Dictionary<string, List<TestNode>> testsByAssembly,
        Dictionary<string, MtpTestDescription> testDescriptions,
        TestSet testSet,
        object discoveryLock,
        ILogger logger,
        IStrykerOptions? options = null)
    {
        _id = id;
        _testsByAssembly = testsByAssembly;
        _testDescriptions = testDescriptions;
        _testSet = testSet;
        _discoveryLock = discoveryLock;
        _logger = logger;
        _options = options;

        // Create unique file paths for this runner to communicate with the test process
        _mutantFilePath = Path.Combine(Path.GetTempPath(), $"stryker-mutant-{_id}.txt");
        _coverageFilePath = Path.Combine(Path.GetTempPath(), $"stryker-coverage-{_id}.txt");
        _runnerId = $"MtpRunner-{_id}";

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

        if (_logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
        {
            _logger.LogDebug("{RunnerId}: Testing mutant(s) [{Mutants}] with active mutation ID: {MutantId}",
                _runnerId, string.Join(",", mutants.Select(m => m.Id)), mutantId);
        }

        return RunAllTestsAsync(assemblies, mutantId, mutants, update, timeoutCalc);
    }

    public async Task ResetServerAsync()
    {
        _logger.LogDebug("{RunnerId}: Resetting test servers to reload assemblies", _runnerId);
        
        await _serverLock.WaitAsync().ConfigureAwait(false);
        try
        {
            foreach (var server in _assemblyServers.Values)
            {
                server.Dispose();
            }
            _assemblyServers.Clear();
        }
        finally
        {
            _serverLock.Release();
        }
        
        _logger.LogDebug("{RunnerId}: Test servers reset complete", _runnerId);
    }

    /// <summary>
    /// Stops and removes the server for a specific assembly. This triggers ProcessExit
    /// in the test process, causing MutantControl.FlushCoverageToFile() to be called.
    /// The server is removed from the cache so a fresh one is created on next use.
    /// </summary>
    internal async Task StopAndRemoveServerAsync(string assembly)
    {
        AssemblyTestServer? server;
        await _serverLock.WaitAsync().ConfigureAwait(false);
        try
        {
            _assemblyServers.TryGetValue(assembly, out server);
            _assemblyServers.Remove(assembly);
        }
        finally
        {
            _serverLock.Release();
        }

        if (server is not null)
        {
            await server.StopAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Runs a single test in isolation to capture its per-test coverage data.
    /// The flow is: start server → run one test → stop server (triggers coverage flush) → read coverage file.
    /// This is used by the pool's CaptureCoverageTestByTest method.
    /// </summary>
    internal virtual async Task<ICoverageRunResult> RunSingleTestForCoverageAsync(
        string assembly, TestNode test, string testId, CoverageConfidence confidence)
    {
        try
        {
            DeleteCoverageFile();

            var server = await GetOrCreateServerAsync(assembly).ConfigureAwait(false);
            await server.RunTestsAsync(new[] { test }).ConfigureAwait(false);
            await StopAndRemoveServerAsync(assembly).ConfigureAwait(false);

            var (coveredMutants, staticMutants) = ReadCoverageData();

            DeleteCoverageFile();

            // Empty coverage likely means the process was force-killed before FlushCoverageToFile ran
            if (coveredMutants.Count == 0 && staticMutants.Count == 0)
            {
                _logger.LogWarning(
                    "{RunnerId}: No coverage data captured for test {TestId} — coverage file was empty or missing. Marking as Dubious.",
                    _runnerId, testId);

                return CoverageRunResult.Create(
                    testId,
                    CoverageConfidence.Dubious,
                    coveredMutants,
                    staticMutants,
                    Array.Empty<int>());
            }

            _logger.LogDebug(
                "{RunnerId}: Test {TestId} covers {CoveredCount} mutants ({StaticCount} static)",
                _runnerId, testId, coveredMutants.Count, staticMutants.Count);

            return CoverageRunResult.Create(
                testId,
                confidence,
                coveredMutants,
                staticMutants,
                Array.Empty<int>());
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "{RunnerId}: Failed to capture coverage for test {TestId}", _runnerId, testId);
            try { await StopAndRemoveServerAsync(assembly).ConfigureAwait(false); }
            catch { /* best-effort cleanup to prevent server leak */ }
            DeleteCoverageFile();
            return CoverageRunResult.Create(
                testId,
                CoverageConfidence.Dubious,
                Array.Empty<int>(),
                Array.Empty<int>(),
                Array.Empty<int>());
        }
    }

    private void WriteMutantIdToFile(int mutantId)
    {
        try
        {
            File.WriteAllText(_mutantFilePath, mutantId.ToString());
            _logger.LogDebug("{RunnerId}: Wrote mutant ID {MutantId} to file {FilePath}",
                _runnerId, mutantId, _mutantFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "{RunnerId}: Failed to write mutant ID to file {FilePath}",
                _runnerId, _mutantFilePath);
        }
    }

    private Dictionary<string, string?> BuildEnvironmentVariables()
    {
        var envVars = new Dictionary<string, string?>
        {
            ["STRYKER_MUTANT_FILE"] = _mutantFilePath
        };

        // Add coverage filename when in coverage mode (MutantControl will combine with temp path)
        if (_coverageMode)
        {
            envVars["STRYKER_COVERAGE_FILE"] = Path.GetFileName(_coverageFilePath);
        }

        return envVars;
    }

    /// <summary>
    /// Enables or disables coverage capture mode. When enabled, the test process will track
    /// which mutations are covered and write the data to a file on process exit.
    /// </summary>
    public void SetCoverageMode(bool enabled)
    {
        _serverLock.Wait();
        try
        {
            if (_coverageMode != enabled)
            {
                _coverageMode = enabled;
                _logger.LogDebug("{RunnerId}: Coverage mode {Status}", _runnerId, enabled ? "enabled" : "disabled");

                foreach (var server in _assemblyServers.Values)
                {
                    server.Dispose();
                }
                _assemblyServers.Clear();
            }
        }
        finally
        {
            _serverLock.Release();
        }

        // Always clean up any existing coverage file to prevent stale data,
        // even when the mode hasn't changed (e.g. retry/re-run paths)
        DeleteCoverageFile();
    }

    /// <summary>
    /// Reads coverage data from the coverage file written by the test process.
    /// Returns the covered mutants and static mutants as separate lists.
    /// </summary>
    public (IReadOnlyList<int> CoveredMutants, IReadOnlyList<int> StaticMutants) ReadCoverageData()
    {
        if (!File.Exists(_coverageFilePath))
        {
            _logger.LogDebug("{RunnerId}: Coverage file not found at {Path}", _runnerId, _coverageFilePath);
            return (Array.Empty<int>(), Array.Empty<int>());
        }

        try
        {
            var content = File.ReadAllText(_coverageFilePath).Trim();
            _logger.LogDebug("{RunnerId}: Read coverage data: {Content}", _runnerId, content);

            if (string.IsNullOrEmpty(content))
            {
                return (Array.Empty<int>(), Array.Empty<int>());
            }

            var parts = content.Split(';');
            var coveredMutants = ParseMutantIds(parts.Length > 0 ? parts[0] : string.Empty);
            var staticMutants = ParseMutantIds(parts.Length > 1 ? parts[1] : string.Empty);

            return (coveredMutants, staticMutants);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "{RunnerId}: Failed to read coverage file at {Path}", _runnerId, _coverageFilePath);
            return (Array.Empty<int>(), Array.Empty<int>());
        }
    }

    private static IReadOnlyList<int> ParseMutantIds(string idString)
    {
        if (string.IsNullOrWhiteSpace(idString))
        {
            return Array.Empty<int>();
        }

        var parts = idString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var result = new List<int>(parts.Length);
        foreach (var part in parts)
        {
            if (int.TryParse(part, out var id))
            {
                result.Add(id);
            }
        }
        return result;
    }

    private void DeleteCoverageFile()
    {
        try
        {
            if (File.Exists(_coverageFilePath))
            {
                File.Delete(_coverageFilePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "{RunnerId}: Failed to delete coverage file at {Path}", _runnerId, _coverageFilePath);
        }
    }

    private async Task<AssemblyTestServer> GetOrCreateServerAsync(string assembly)
    {
        await _serverLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_assemblyServers.TryGetValue(assembly, out var existing) && existing.IsInitialized)
            {
                return existing;
            }

            var environmentVariables = BuildEnvironmentVariables();
            var server = new AssemblyTestServer(assembly, environmentVariables, _logger, _runnerId, _options);

            var started = await server.StartAsync().ConfigureAwait(false);
            if (!started)
            {
                throw new InvalidOperationException($"Failed to start test server for {assembly}");
            }

            _assemblyServers[assembly] = server;
            return server;
        }
        finally
        {
            _serverLock.Release();
        }
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

                foreach (var test in tests.Where(t => !_testDescriptions.ContainsKey(t.Uid)))
                {
                    var mtpTestDescription = new MtpTestDescription(test);
                    _testDescriptions[test.Uid] = mtpTestDescription;
                    _testSet.RegisterTest(mtpTestDescription.Description);
                }
            }

            _logger.LogDebug("{RunnerId}: Discovered {TestCount} tests in {Assembly}", _runnerId, tests.Count, assembly);
            return tests.Count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "{RunnerId}: Failed to discover tests in {Assembly}", _runnerId, assembly);
            return false;
        }
    }

    internal List<TestNode>? GetDiscoveredTests(string assembly)
    {
        lock (_discoveryLock)
        {
            return _testsByAssembly.TryGetValue(assembly, out var tests) ? tests : null;
        }
    }

    internal TimeSpan? CalculateAssemblyTimeout(List<TestNode> discoveredTests, ITimeoutValueCalculator timeoutCalc, string assembly)
    {
        int estimatedTimeMs;
        lock (_discoveryLock)
        {
            estimatedTimeMs = (int)discoveredTests
                .Sum(t => _testDescriptions.TryGetValue(t.Uid, out var desc)
                    ? desc.InitialRunTime.TotalMilliseconds
                    : 0);
        }

        var timeoutMs = timeoutCalc.CalculateTimeoutValue(estimatedTimeMs);
        _logger.LogDebug("{RunnerId}: Using {TimeoutMs} ms as test run timeout for {Assembly}",
            _runnerId, timeoutMs, Path.GetFileName(assembly));

        return TimeSpan.FromMilliseconds(timeoutMs);
    }

    internal async Task HandleAssemblyTimeoutAsync(string assembly, List<TestNode> discoveredTests, List<string> allTimedOutTests)
    {
        _logger.LogDebug("{RunnerId}: Test run timed out for {Assembly}", _runnerId, Path.GetFileName(assembly));

        allTimedOutTests.AddRange(discoveredTests.Select(t => t.Uid));
        
        AssemblyTestServer? server;
        await _serverLock.WaitAsync().ConfigureAwait(false);
        try
        {
            _assemblyServers.TryGetValue(assembly, out server);
        }
        finally
        {
            _serverLock.Release();
        }
        
        if (server is not null)
        {
            _logger.LogDebug("{RunnerId}: Restarting test server for {Assembly} after timeout", _runnerId, Path.GetFileName(assembly));
            await server.RestartAsync().ConfigureAwait(false);
        }
    }

    private sealed class TestRunAccumulator
    {
        private readonly List<string> _executedTests = [];
        private readonly List<string> _failedTests = [];
        private readonly List<string> _messages = [];
        private readonly List<string> _errorMessages = [];
        private int _totalDiscoveredTests;
        private int _totalExecutedTests;

        public List<string> TimedOutTests { get; } = [];
        public TimeSpan TotalDuration { get; private set; }

        public void Aggregate(TestRunResult result, List<TestNode>? discoveredTests)
        {
            if (result.ExecutedTests.IsEveryTest)
            {
                _totalExecutedTests += discoveredTests?.Count ?? 0;
            }
            else
            {
                var before = _executedTests.Count;
                _executedTests.AddRange(result.ExecutedTests.GetIdentifiers());
                _totalExecutedTests += _executedTests.Count - before;
            }

            _failedTests.AddRange(result.FailingTests.GetIdentifiers());
            TotalDuration += result.Duration;
            _messages.AddRange(result.Messages);

            if (!string.IsNullOrWhiteSpace(result.ResultMessage))
            {
                _errorMessages.Add(result.ResultMessage);
            }
        }

        public void AddDiscoveredCount(int count) => _totalDiscoveredTests += count;

        public ITestIdentifiers BuildExecutedTests() =>
            _totalDiscoveredTests > 0 && _totalExecutedTests >= _totalDiscoveredTests
                ? TestIdentifierList.EveryTest()
                : new TestIdentifierList(_executedTests);

        public ITestIdentifiers BuildFailedTests() => new TestIdentifierList(_failedTests);

        public ITestIdentifiers BuildTimedOutTests() => new TestIdentifierList(TimedOutTests);

        public string BuildErrorMessage() => string.Join(Environment.NewLine, _errorMessages);

        public IEnumerable<string> Messages => _messages;
    }

    internal async Task<(TestRunResult? Result, bool TimedOut, List<TestNode>? DiscoveredTests)> ProcessSingleAssemblyAsync(
        string assembly,
        ITimeoutValueCalculator? timeoutCalc,
        bool registerInitialResults = false)
    {
        if (!File.Exists(assembly))
        {
            return (null, false, null);
        }

        var discoveredTests = GetDiscoveredTests(assembly);

        TimeSpan? timeout = null;
        if (timeoutCalc is not null && discoveredTests is not null)
        {
            timeout = CalculateAssemblyTimeout(discoveredTests, timeoutCalc, assembly);
        }

        var (testResults, timedOut) = await RunTestsInternalAsync(assembly, null, timeout, registerInitialResults).ConfigureAwait(false);

        return (testResults as TestRunResult, timedOut, discoveredTests);
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
            WriteMutantIdToFile(mutantId);

            var accumulator = new TestRunAccumulator();

            foreach (var assembly in assemblies)
            {
                var (result, timedOut, discoveredTests) = await ProcessSingleAssemblyAsync(assembly, timeoutCalc, mutantId == -1).ConfigureAwait(false);

                if (discoveredTests is not null)
                {
                    accumulator.AddDiscoveredCount(discoveredTests.Count);

                    if (timedOut)
                    {
                        await HandleAssemblyTimeoutAsync(assembly, discoveredTests, accumulator.TimedOutTests).ConfigureAwait(false);
                    }
                }

                if (result is not null)
                {
                    accumulator.Aggregate(result, discoveredTests);
                }
            }

            var executedTests = accumulator.BuildExecutedTests();
            var failedTestIds = accumulator.BuildFailedTests();
            var timedOutTestIds = accumulator.BuildTimedOutTests();

            if (update is not null && mutants is not null)
            {
                update.Invoke(mutants, failedTestIds, executedTests, timedOutTestIds);
            }

            lock (_discoveryLock)
            {
                return new TestRunResult(
                    _testDescriptions.Values,
                    executedTests,
                    failedTestIds,
                    timedOutTestIds,
                    accumulator.BuildErrorMessage(),
                    accumulator.Messages,
                    accumulator.TotalDuration);
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "{RunnerId}: Failed to run tests for mutant ID {MutantId}", _runnerId, mutantId);
            return new TestRunResult(false, ex.Message);
        }
    }

    internal async Task<(ITestRunResult Result, bool TimedOut)> RunTestsInternalAsync(
        string assembly,
        Func<TestNode, bool>? testUidFilter,
        TimeSpan? timeout = null,
        bool registerInitialResults = false)
    {
        var startTime = DateTime.UtcNow;

        try
        {
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

            // Single pass: build finished/failed lists, error messages, and per-test messages together
            var finishedTests = new List<TestNodeUpdate>(testResults.Count);
            var failedTestUids = new List<string>();
            var errorParts = new List<string>();
            var messageParts = new List<string>();
            foreach (var tr in testResults)
            {
                if (tr.Node.ExecutionState is "in-progress")
                {
                    continue;
                }

                finishedTests.Add(tr);
                messageParts.Add($"{tr.Node.DisplayName}{Environment.NewLine}{Environment.NewLine}State: {tr.Node.ExecutionState}");

                if (tr.Node.ExecutionState is "failed")
                {
                    failedTestUids.Add(tr.Node.Uid);
                    errorParts.Add($"{tr.Node.DisplayName}{Environment.NewLine}{Environment.NewLine}Test failed");
                }
            }

            var errorMessagesStr = string.Join(Environment.NewLine, errorParts);

            var totalDiscoveredTests = tests?.Count ?? 0;
            var executedTests = totalDiscoveredTests > 0 && finishedTests.Count >= totalDiscoveredTests
                ? TestIdentifierList.EveryTest()
                : new TestIdentifierList(finishedTests.Select(x => x.Node.Uid));

            var failedTestIds = new TestIdentifierList(failedTestUids);

            TestRunResult result;
            lock (_discoveryLock)
            {
                if (registerInitialResults)
                {
                    foreach (var tr in finishedTests)
                    {
                        if (_testDescriptions.TryGetValue(tr.Node.Uid, out var desc))
                        {
                            desc.RegisterInitialTestResult(new MtpTestResult(duration));
                        }
                    }
                }

                result = new TestRunResult(
                    _testDescriptions.Values,
                    executedTests,
                    failedTestIds,
                    TestIdentifierList.NoTest(),
                    errorMessagesStr,
                    messageParts,
                    duration);
            }

            return (result, timedOut);
        }
        catch (Exception ex)
        {
            return (new TestRunResult(false, ex.Message), false);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _serverLock.Wait();
            try
            {
                foreach (var server in _assemblyServers.Values)
                {
                    server.Dispose();
                }
                _assemblyServers.Clear();
            }
            finally
            {
                _serverLock.Release();
            }

            // Clean up temp files
            try
            {
                if (File.Exists(_mutantFilePath))
                {
                    File.Delete(_mutantFilePath);
                }
                if (File.Exists(_coverageFilePath))
                {
                    File.Delete(_coverageFilePath);
                }
            }
            catch (Exception ex)
            {
                // Ignore cleanup errors
                _logger.LogWarning(ex, "{RunnerId}: Failed to clean up temp files", _runnerId);
            }
        }
        _disposed = true;
    }
}


