using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
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

    private readonly Dictionary<string, AssemblyTestServer> _assemblyServers = new();
    private readonly object _serverLock = new();
    private bool _disposed;
    private bool _coverageMode;

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

        // Create unique file paths for this runner to communicate with the test process
        _mutantFilePath = Path.Combine(Path.GetTempPath(), $"stryker-mutant-{_id}.txt");
        _coverageFilePath = Path.Combine(Path.GetTempPath(), $"stryker-coverage-{_id}.txt");

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

    public async Task ResetServerAsync()
    {
        _logger.LogDebug("{RunnerId}: Resetting test servers to reload assemblies", RunnerId);
        
        lock (_serverLock)
        {
            foreach (var server in _assemblyServers.Values)
            {
                server.Dispose();
            }
            _assemblyServers.Clear();
        }
        
        _logger.LogDebug("{RunnerId}: Test servers reset complete", RunnerId);
        await Task.CompletedTask;
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
        if (_coverageMode == enabled)
        {
            // Already in the desired state; no action needed
            return;
        }

        _coverageMode = enabled;
        _logger.LogDebug("{RunnerId}: Coverage mode {Status}", RunnerId, enabled ? "enabled" : "disabled");

        // Reset servers to apply the new environment variables
        lock (_serverLock)
        {
            foreach (var server in _assemblyServers.Values)
            {
                server.Dispose();
            }
            _assemblyServers.Clear();
        }

        // Clean up any existing coverage file, even when enabling, to ensure we start fresh
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
            _logger.LogDebug("{RunnerId}: Coverage file not found at {Path}", RunnerId, _coverageFilePath);
            return (Array.Empty<int>(), Array.Empty<int>());
        }

        try
        {
            var content = File.ReadAllText(_coverageFilePath).Trim();
            _logger.LogDebug("{RunnerId}: Read coverage data: {Content}", RunnerId, content);

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
            _logger.LogWarning(ex, "{RunnerId}: Failed to read coverage file at {Path}", RunnerId, _coverageFilePath);
            return (Array.Empty<int>(), Array.Empty<int>());
        }
    }

    private static IReadOnlyList<int> ParseMutantIds(string idString)
    {
        if (string.IsNullOrWhiteSpace(idString))
        {
            return Array.Empty<int>();
        }

        return idString
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s.Trim(), out var id) ? id : (int?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToList();
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
        catch
        {
            // Ignore cleanup errors
        }
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

                foreach (var test in tests.Where(t => !_testDescriptions.ContainsKey(t.Uid)))
                {
                    var mtpTestDescription = new MtpTestDescription(test);
                    _testDescriptions[test.Uid] = mtpTestDescription;
                    _testSet.RegisterTest(mtpTestDescription.Description);
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

    private List<TestNode>? GetDiscoveredTests(string assembly)
    {
        lock (_discoveryLock)
        {
            return _testsByAssembly.TryGetValue(assembly, out var tests) ? tests : null;
        }
    }

    private TimeSpan? CalculateAssemblyTimeout(List<TestNode> discoveredTests, ITimeoutValueCalculator timeoutCalc, string assembly)
    {
        var estimatedTimeMs = (int)discoveredTests
            .Where(t => _testDescriptions.TryGetValue(t.Uid, out _))
            .Sum(t =>
            {
                lock (_discoveryLock)
                {
                    return _testDescriptions.TryGetValue(t.Uid, out var desc)
                        ? desc.InitialRunTime.TotalMilliseconds
                        : 0;
                }
            });
        
        var timeoutMs = timeoutCalc.CalculateTimeoutValue(estimatedTimeMs);
        _logger.LogDebug("{RunnerId}: Using {TimeoutMs} ms as test run timeout for {Assembly}",
            RunnerId, timeoutMs, Path.GetFileName(assembly));
        
        return TimeSpan.FromMilliseconds(timeoutMs);
    }

    private async Task HandleAssemblyTimeoutAsync(string assembly, List<TestNode> discoveredTests, List<string> allTimedOutTests)
    {
        _logger.LogDebug("{RunnerId}: Test run timed out for {Assembly}", RunnerId, Path.GetFileName(assembly));
        
        allTimedOutTests.AddRange(discoveredTests.Select(t => t.Uid));
        
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

    private void AggregateTestResults(
        TestRunResult result,
        List<TestNode>? discoveredTests,
        List<string> allExecutedTests,
        List<string> allFailedTests,
        List<string> allMessages,
        List<string> errorMessages,
        ref int totalExecutedTests,
        ref TimeSpan totalDuration)
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

    private async Task<(TestRunResult? Result, bool TimedOut, List<TestNode>? DiscoveredTests)> ProcessSingleAssemblyAsync(
        string assembly,
        ITimeoutValueCalculator? timeoutCalc)
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

        var (testResults, timedOut) = await RunTestsInternalAsync(assembly, null, timeout).ConfigureAwait(false);
        
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
                var (result, timedOut, discoveredTests) = await ProcessSingleAssemblyAsync(assembly, timeoutCalc).ConfigureAwait(false);
                
                if (discoveredTests is not null)
                {
                    totalDiscoveredTests += discoveredTests.Count;
                }

                if (timedOut && discoveredTests is not null)
                {
                    await HandleAssemblyTimeoutAsync(assembly, discoveredTests, allTimedOutTests).ConfigureAwait(false);
                }

                if (result is not null)
                {
                    AggregateTestResults(result, discoveredTests, allExecutedTests, allFailedTests, 
                        allMessages, errorMessages, ref totalExecutedTests, ref totalDuration);
                }
            }

            return BuildFinalTestResult(allExecutedTests, allFailedTests, allTimedOutTests, 
                allMessages, errorMessages, totalDuration, totalDiscoveredTests, totalExecutedTests, 
                mutants, update);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "{RunnerId}: Failed to run tests for mutant ID {MutantId}", RunnerId, mutantId);
            return new TestRunResult(false, ex.Message);
        }
    }

    private ITestRunResult BuildFinalTestResult(
        List<string> allExecutedTests,
        List<string> allFailedTests,
        List<string> allTimedOutTests,
        List<string> allMessages,
        List<string> errorMessages,
        TimeSpan totalDuration,
        int totalDiscoveredTests,
        int totalExecutedTests,
        IReadOnlyList<IMutant>? mutants,
        TestUpdateHandler? update)
    {
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

        if (update is not null && mutants is not null)
        {
            update.Invoke(mutants, failedTestIds, executedTests, timedOutTestIds);
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

    private async Task<(ITestRunResult Result, bool TimedOut)> RunTestsInternalAsync(
        string assembly,
        Func<TestNode, bool>? testUidFilter,
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
                foreach (var testResult in finishedTests.Where(tr => _testDescriptions.ContainsKey(tr.Node.Uid)))
                {
                    var testDescription = _testDescriptions[testResult.Node.Uid];
                    testDescription.RegisterInitialTestResult(new MtpTestResult(duration));
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
            lock (_serverLock)
            {
                foreach (var server in _assemblyServers.Values)
                {
                    server.Dispose();
                }
                _assemblyServers.Clear();
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
                _logger.LogWarning(ex, "{RunnerId}: Failed to clean up temp files", RunnerId);
            }
        }
        _disposed = true;
    }
}


