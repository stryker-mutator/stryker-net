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
    private readonly string _testAssemblyPath;
    private readonly TestSet _testSet = new();
    private readonly Dictionary<string, TestNode> _discoveredTests = new();

    public MicrosoftTestPlatformRunner(string testAssemblyPath)
    {
        _testAssemblyPath = testAssemblyPath ?? throw new ArgumentNullException(nameof(testAssemblyPath));

        if (!File.Exists(_testAssemblyPath))
        {
            throw new FileNotFoundException($"Test assembly not found at path: {_testAssemblyPath}");
        }
    }

    public bool DiscoverTests(string assembly)
    {
        if (string.IsNullOrEmpty(assembly))
        {
            return false;
        }

        var discoveryTask = DiscoverTestsInternalAsync(assembly);
        discoveryTask.Wait();
        return discoveryTask.Result;
    }

    public ITestSet GetTests(IProjectAndTests project)
    {
        return _testSet;
    }

    public ITestRunResult InitialTest(IProjectAndTests project)
    {
        var runTask = RunAllTestsAsync(project);
        runTask.Wait();
        return runTask.Result;
    }

    public IEnumerable<ICoverageRunResult> CaptureCoverage(IProjectAndTests project)
    {
        return Enumerable.Empty<ICoverageRunResult>();
    }

    public ITestRunResult TestMultipleMutants(IProjectAndTests project, ITimeoutValueCalculator timeoutCalc,
        IReadOnlyList<IMutant> mutants, ITestRunner.TestUpdateHandler update)
    {
        var runTask = RunAllTestsAsync(project);
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

            foreach (var test in tests)
            {
                _discoveredTests[test.Uid] = test;
                var testDescription = new TestDescription(test.Uid, test.DisplayName, string.Empty);
                _testSet.RegisterTest(testDescription);
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

    private async Task<ITestRunResult> RunAllTestsAsync(IProjectAndTests project)
    {
        try
        {
            var testResults = await RunTestsInternalAsync(CancellationToken.None, null);
            return testResults;
        }
        catch (Exception ex)
        {
            return new TestRunResult(false, ex.Message);
        }
    }

    private async Task<ITestRunResult> RunTestsInternalAsync(CancellationToken cancellationToken,
        Func<TestNode, bool>? testUidFilter)
    {
        var startTime = DateTime.UtcNow;
        var listener = new TcpListener(new IPEndPoint(IPAddress.Any, 0));
        listener.Start();

        var port = ((IPEndPoint)listener.LocalEndpoint).Port;

        await using var output = new MemoryStream();
        var outputPipe = PipeTarget.ToStream(output);

        var cliProcess = Cli.Wrap("dotnet")
            .WithWorkingDirectory(Path.GetDirectoryName(_testAssemblyPath) ?? string.Empty)
            .WithArguments([
                _testAssemblyPath,
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

        var executeTestsResponse = await client.RunTestsAsync(runId, updates =>
        {
            testResults.AddRange(updates);
            return Task.CompletedTask;
        }, null);

        await executeTestsResponse.WaitCompletionAsync();
        await client.ExitAsync();
        listener.Stop();

        var duration = DateTime.UtcNow - startTime;
        var finishedTests = testResults.Where(x => x.Node.ExecutionState is not "in-progress").ToList();
        var failedTests = finishedTests.Where(x => x.Node.ExecutionState is "failed").Select(x => x.Node.Uid).ToList();

        var executedTests = new TestIdentifierList(finishedTests.Select(x => x.Node.Uid));
        var failedTestIds = failedTests.Any()
            ? new TestIdentifierList(failedTests)
            : TestIdentifierList.NoTest();

        return new TestRunResult(
            Enumerable.Empty<IFrameworkTestDescription>(),
            executedTests,
            failedTestIds,
            TestIdentifierList.NoTest(),
            string.Empty,
            Enumerable.Empty<string>(),
            duration);
    }
}
