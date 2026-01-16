using System.Net;
using System.Net.Sockets;
using CliWrap;
using MsTestRunnerDemo;
using MsTestRunnerDemo.Models;
using StreamJsonRpc;
using Stryker.Abstractions;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

public sealed class MicrosoftTestPlatformRunner : ITestRunner
{
    private readonly TestSet _testSet = new();
    private readonly Dictionary<string, List<TestNode>> _testsByAssembly = new();
    private readonly Dictionary<string, MtpTestDescription> _testDescriptions = new();

    public bool DiscoverTests(string assembly)
    {
        if (string.IsNullOrEmpty(assembly))
        {
            return false;
        }

        if (!File.Exists(assembly))
        {
            return false;
        }

        var discoveryTask = DiscoverTestsInternalAsync(assembly);
        discoveryTask.Wait();
        var result = discoveryTask.Result;
        return result;
    }

    public ITestSet GetTests(IProjectAndTests project)
    {
        return _testSet;
    }

    public ITestRunResult InitialTest(IProjectAndTests project)
    {
        var assemblies = project.GetTestAssemblies();
        if (!assemblies.Any())
        {
            return new TestRunResult(false, "No test assemblies found");
        }

        var runTask = RunAllTestsAsync(assemblies);
        runTask.Wait();
        return runTask.Result;
    }

    public IEnumerable<ICoverageRunResult> CaptureCoverage(IProjectAndTests project)
    {
        // MicrosoftTestPlatform doesn't have built-in coverage collection infrastructure yet.
        // Return dubious coverage for all tests, meaning they should be tested against all mutants.
        // This is similar to VsTest behavior when coverage data collection fails.
        var coverageResults = new List<ICoverageRunResult>();

        foreach (var testDescription in _testDescriptions.Values)
        {
            // Create coverage result with dubious confidence and no specific mutant coverage
            // This ensures all mutants will be tested but without specific coverage optimization
            var coverageResult = CoverageRunResult.Create(
                testDescription.Id,
                CoverageConfidence.Dubious,
                [],
                [],
                []);

            coverageResults.Add(coverageResult);
        }

        return coverageResults;
    }

    public ITestRunResult TestMultipleMutants(IProjectAndTests project, ITimeoutValueCalculator timeoutCalc,
        IReadOnlyList<IMutant> mutants, ITestRunner.TestUpdateHandler update)
    {
        var assemblies = project.GetTestAssemblies();
        if (!assemblies.Any())
        {
            return new TestRunResult(false, "No test assemblies found");
        }

        var runTask = RunAllTestsAsync(assemblies, mutants, update);
        runTask.Wait();
        return runTask.Result;
    }

    public void Dispose()
    {
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
                .WithArguments([
                    assembly,
                    "--server",
                    "--client-port",
                    port.ToString()
                ])
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

            _testsByAssembly[assembly] = tests;

            foreach (var test in tests)
            {
                var mtpTestDescription = new MtpTestDescription(test);
                _testDescriptions[test.Uid] = mtpTestDescription;
                _testSet.RegisterTest(mtpTestDescription.Description);
            }

            await client.ExitAsync();
            listener.Stop();

            return tests.Count > 0;
        }
        catch
        {
            return false;
        }
    }

    private async Task<ITestRunResult> RunAllTestsAsync(IReadOnlyList<string> assemblies, IReadOnlyList<IMutant>? mutants = null, ITestRunner.TestUpdateHandler? update = null)
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

                // Track total discovered tests for EveryTest() optimization
                if (_testsByAssembly.TryGetValue(assembly, out var discoveredTests))
                {
                    totalDiscoveredTests += discoveredTests.Count;
                }

                var testResults = await RunTestsInternalAsync(assembly, CancellationToken.None, null, mutants, update);

                if (testResults is TestRunResult result)
                {
                    // Track if we got EveryTest() back from the assembly run
                    if (result.ExecutedTests.IsEveryTest)
                    {
                        // Assembly returned EveryTest, count all its discovered tests as executed
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

            // Return EveryTest() if all discovered tests were executed
            var executedTests = totalDiscoveredTests > 0 && totalExecutedTests >= totalDiscoveredTests
                ? TestIdentifierList.EveryTest()
                : new TestIdentifierList(allExecutedTests);

            var failedTestIds = allFailedTests.Any()
                ? new TestIdentifierList(allFailedTests)
                : TestIdentifierList.NoTest();

            return new TestRunResult(
                _testDescriptions.Values,
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

    private async Task<ITestRunResult> RunTestsInternalAsync(string assembly, CancellationToken cancellationToken,
        Func<TestNode, bool>? testUidFilter, IReadOnlyList<IMutant>? mutants = null, ITestRunner.TestUpdateHandler? update = null)
    {
        var startTime = DateTime.UtcNow;
        var listener = new TcpListener(new IPEndPoint(IPAddress.Any, 0));
        listener.Start();

        var port = ((IPEndPoint)listener.LocalEndpoint).Port;

        await using var output = new MemoryStream();
        var outputPipe = PipeTarget.ToStream(output);

        var cliProcess = Cli.Wrap("dotnet")
            .WithWorkingDirectory(Path.GetDirectoryName(assembly) ?? string.Empty)
            .WithArguments([
                assembly,
                "--server",
                "--client-port",
                port.ToString()
            ])
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

        var testsToRun = _testsByAssembly.TryGetValue(assembly, out var tests)
            ? tests.Where(t => testUidFilter == null || testUidFilter(t)).ToArray()
            : null;

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

        foreach (var testResult in finishedTests)
        {
            if (_testDescriptions.TryGetValue(testResult.Node.Uid, out var testDescription))
            {
                testDescription.RegisterInitialTestResult(new MtpTestResult(duration));
            }
        }

        var errorMessages = string.Join(Environment.NewLine,
            finishedTests.Where(x => x.Node.ExecutionState is "failed")
                .Select(x => $"{x.Node.DisplayName}{Environment.NewLine}{Environment.NewLine}Test failed"));

        var messages = finishedTests.Select(x =>
            $"{x.Node.DisplayName}{Environment.NewLine}{Environment.NewLine}State: {x.Node.ExecutionState}");

        // Optimize by returning EveryTest() when all tests were executed
        // This is critical for AnalyzeTestRun to correctly identify survived mutants
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

        return new TestRunResult(
            _testDescriptions.Values,
            executedTests,
            failedTestIds,
            TestIdentifierList.NoTest(),
            errorMessages,
            messages,
            duration);
    }
}
