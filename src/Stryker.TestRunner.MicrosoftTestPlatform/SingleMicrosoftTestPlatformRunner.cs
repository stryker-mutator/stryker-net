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
/// Individual test runner instance that handles test execution with mutation-specific
/// environment variables. Used by MicrosoftTestPlatformRunnerPool.
/// </summary>
internal sealed class SingleMicrosoftTestPlatformRunner : IDisposable
{
    private readonly int _id;
    private readonly Dictionary<string, List<TestNode>> _testsByAssembly;
    private readonly Dictionary<string, MtpTestDescription> _testDescriptions;
    private readonly TestSet _testSet;
    private readonly object _discoveryLock;
    private readonly ILogger _logger;

    private string RunnerId => $"MtpRunner-{_id}";
    private string ControlVariableName => $"ACTIVE_MUTATION_{_id}";

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
    }

    public bool DiscoverTests(string assembly)
    {
        var discoveryTask = DiscoverTestsInternalAsync(assembly);
        discoveryTask.Wait();
        return discoveryTask.Result;
    }

    public ITestRunResult InitialTest(IProjectAndTests project)
    {
        var assemblies = project.GetTestAssemblies();
        var runTask = RunAllTestsAsync(assemblies, mutantId: null, mutants: null, update: null);
        runTask.Wait();
        return runTask.Result;
    }

    public ITestRunResult TestMultipleMutants(
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

        var runTask = RunAllTestsAsync(assemblies, mutantId, mutants, update);
        runTask.Wait();
        return runTask.Result;
    }

    public void Dispose()
    {
        // Nothing to dispose for now
    }

    private async Task<bool> DiscoverTestsInternalAsync(string assembly)
    {
        try
        {
            var cancellationToken = CancellationToken.None;
            var listener = new TcpListener(new IPEndPoint(IPAddress.Any, 0));
            listener.Start();

            var port = ((IPEndPoint)listener.LocalEndpoint).Port;

            var cliProcess = Cli.Wrap("dotnet")
                .WithWorkingDirectory(Path.GetDirectoryName(assembly) ?? string.Empty)
                .WithArguments([assembly, "--server", "--client-port", port.ToString()])
                .WithStandardOutputPipe(PipeTarget.ToDelegate(_ => { }))
                .WithStandardErrorPipe(PipeTarget.ToDelegate(_ => { }))
                .ExecuteAsync(cancellationToken: cancellationToken);

            var tcpClientTask = listener.AcceptTcpClientAsync(cancellationToken).AsTask();
            var connectionTimeout = Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            var completedTask = await Task.WhenAny(cliProcess.Task, tcpClientTask, connectionTimeout);

            if (completedTask == connectionTimeout || completedTask == cliProcess.Task)
            {
                listener.Stop();
                return false;
            }

            using var tcpClient = await tcpClientTask;
            await using var stream = tcpClient.GetStream();

            using var rpc = new JsonRpc(new HeaderDelimitedMessageHandler(stream, stream, new SystemTextJsonFormatter
            {
                JsonSerializerOptions = RpcJsonSerializerOptions.Default
            }));

            using var output = new MemoryStream();
            using var client = new TestingPlatformClient(rpc, tcpClient, new ProcessHandle(cliProcess, output), enableDiagnostic: false);

            await client.InitializeAsync();

            var discoveryId = Guid.NewGuid();
            List<TestNodeUpdate> discoveredResults = [];

            var discoverTestsResponse = await client.DiscoverTestsAsync(discoveryId, updates =>
            {
                discoveredResults.AddRange(updates);
                return Task.CompletedTask;
            });

            await discoverTestsResponse.WaitCompletionAsync();

            var tests = discoveredResults
                .Where(x => x.Node.ExecutionState is "discovered")
                .Select(x => x.Node)
                .ToList();

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

            await client.ExitAsync();
            listener.Stop();

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
        int? mutantId,
        IReadOnlyList<IMutant>? mutants,
        TestUpdateHandler? update)
    {
        try
        {
            var allExecutedTests = new List<string>();
            var allFailedTests = new List<string>();
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

                var testResults = await RunTestsInternalAsync(assembly, CancellationToken.None, null, mutantId, mutants, update);

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

            var failedTestIds = allFailedTests.Any()
                ? new TestIdentifierList(allFailedTests)
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
                TestIdentifierList.NoTest(),
                string.Join(Environment.NewLine, errorMessages),
                allMessages,
                totalDuration);
        }
        catch (Exception ex)
        {
            return new TestRunResult(false, ex.Message);
        }
    }

    private async Task<ITestRunResult> RunTestsInternalAsync(
        string assembly,
        CancellationToken cancellationToken,
        Func<TestNode, bool>? testUidFilter,
        int? mutantId,
        IReadOnlyList<IMutant>? mutants,
        TestUpdateHandler? update)
    {
        var startTime = DateTime.UtcNow;
        var listener = new TcpListener(new IPEndPoint(IPAddress.Any, 0));
        listener.Start();

        var port = ((IPEndPoint)listener.LocalEndpoint).Port;

        await using var output = new MemoryStream();
        var outputPipe = PipeTarget.ToStream(output);

        // Build environment variables for mutation control
        var environmentVariables = new Dictionary<string, string?>
        {
            ["STRYKER_MUTANT_ID_CONTROL_VAR"] = ControlVariableName
        };

        // Set the active mutation ID if testing a mutant
        if (mutantId.HasValue)
        {
            environmentVariables[ControlVariableName] = mutantId.Value.ToString();
            _logger.LogDebug("{RunnerId}: Setting {ControlVar}={MutantId} for mutation testing",
                RunnerId, ControlVariableName, mutantId.Value);
        }
        else
        {
            // No active mutation for initial test run
            environmentVariables[ControlVariableName] = "-1";
        }

        var cliProcess = Cli.Wrap("dotnet")
            .WithWorkingDirectory(Path.GetDirectoryName(assembly) ?? string.Empty)
            .WithArguments([assembly, "--server", "--client-port", port.ToString()])
            .WithEnvironmentVariables(environmentVariables)
            .WithStandardOutputPipe(outputPipe)
            .WithStandardErrorPipe(outputPipe)
            .ExecuteAsync(cancellationToken: cancellationToken);

        var tcpClientTask = listener.AcceptTcpClientAsync(cancellationToken).AsTask();
        var connectionTimeout = Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
        var completedTask = await Task.WhenAny(cliProcess.Task, tcpClientTask, connectionTimeout);

        if (completedTask == connectionTimeout)
        {
            listener.Stop();
            return new TestRunResult(false, "Timeout waiting for test connection");
        }

        if (completedTask == cliProcess.Task)
        {
            listener.Stop();
            var result = await cliProcess.Task;
            return new TestRunResult(false, $"Test process exited with code {result.ExitCode}");
        }

        using var tcpClient = await tcpClientTask;
        await using var stream = tcpClient.GetStream();

        using var rpc = new JsonRpc(new HeaderDelimitedMessageHandler(stream, stream, new SystemTextJsonFormatter
        {
            JsonSerializerOptions = RpcJsonSerializerOptions.Default
        }));

        using var client = new TestingPlatformClient(rpc, tcpClient, new ProcessHandle(cliProcess, output), enableDiagnostic: false);

        await client.InitializeAsync();

        var runId = Guid.NewGuid();
        List<TestNodeUpdate> testResults = [];

        List<TestNode>? tests = null;
        lock (_discoveryLock)
        {
            if (_testsByAssembly.TryGetValue(assembly, out var assemblyTests))
            {
                tests = assemblyTests;
            }
        }

        var testsToRun = tests?.Where(t => testUidFilter == null || testUidFilter(t)).ToArray();

        var executeTestsResponse = await client.RunTestsAsync(runId, updates =>
        {
            testResults.AddRange(updates);
            return Task.CompletedTask;
        }, testsToRun);

        await executeTestsResponse.WaitCompletionAsync();
        await client.ExitAsync();
        listener.Stop();

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

        var errorMessages = string.Join(Environment.NewLine,
            finishedTests.Where(x => x.Node.ExecutionState is "failed")
                .Select(x => $"{x.Node.DisplayName}{Environment.NewLine}{Environment.NewLine}Test failed"));

        var messages = finishedTests.Select(x =>
            $"{x.Node.DisplayName}{Environment.NewLine}{Environment.NewLine}State: {x.Node.ExecutionState}");

        var totalDiscoveredTests = tests?.Count ?? 0;
        var executedTestCount = finishedTests.Count;
        var executedTests = totalDiscoveredTests > 0 && executedTestCount >= totalDiscoveredTests
            ? TestIdentifierList.EveryTest()
            : new TestIdentifierList(finishedTests.Select(x => x.Node.Uid));

        var failedTestIds = failedTests.Any()
            ? new TestIdentifierList(failedTests)
            : TestIdentifierList.NoTest();

        if (update != null && mutants != null)
        {
            update.Invoke(mutants, failedTestIds, executedTests, TestIdentifierList.NoTest());
        }

        IEnumerable<MtpTestDescription> testDescriptionValues;
        lock (_discoveryLock)
        {
            testDescriptionValues = _testDescriptions.Values.ToList();
        }

        return new TestRunResult(
            testDescriptionValues,
            executedTests,
            failedTestIds,
            TestIdentifierList.NoTest(),
            errorMessages,
            messages,
            duration);
    }
}

