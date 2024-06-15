using System.Reflection;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions.Messages;
using Stryker.TestRunner.MSTest.Testing.Consumers;
using Stryker.TestRunner.MSTest.Testing.LifecycleCallbacks;
using Stryker.TestRunner.MSTest.Testing.Options;
using Stryker.TestRunner.MSTest.Testing.Results;
using Stryker.TestRunner.MSTest.Testing.Tests;

namespace Stryker.TestRunner.MSTest.Testing.TestProjects;
internal class MsTestProject : ITestProject
{
    public const string EntryPoint = "MSTest";
    public const string ExecutorPath = "executor://mstestadapter";

    private readonly Assembly _assembly;

    private MsTestProject(Assembly assembly)
    {
        _assembly = assembly;
    }

    public static MsTestProject Create(Assembly assembly) => new(assembly);
    
    public async Task<int> Discover(DiscoveryResult discoveryResult, string assemblyPath)
    {
        var builder = await TestApplication.CreateBuilderAsync([RunOptions.DiscoverySettings, RunOptions.NoBanner, RunOptions.NoConsole]);
        builder.AddMSTest(() => [_assembly]);
        builder.TestHost.AddDataConsumer(_ => DiscoveryConsumer.Create(assemblyPath, new Uri(ExecutorPath), discoveryResult));
        using var app = await builder.BuildAsync();
        return await app.RunAsync();
    }

    public async Task<int> InitialTestRun(DiscoveryResult discoveryResult, List<TestNode> executed)
    {
        var builder = await TestApplication.CreateBuilderAsync([RunOptions.DiscoverySettings, RunOptions.NoBanner, RunOptions.NoConsole]);
        builder.AddMSTest(() => [_assembly]);
        builder.TestHost.AddDataConsumer((_) => InitialTestRunConsumer.Create(discoveryResult, executed));
        using var app = await builder.BuildAsync();
        return await app.RunAsync();
    }

    public async Task<int> CoverageRun(CoverageCollector coverageCollector)
    {
        var builder = await TestApplication.CreateBuilderAsync([RunOptions.NoBanner, RunOptions.NoConsole]);
        builder.AddMSTest(() => [_assembly]);
        builder.TestHost.AddTestApplicationLifecycleCallbacks((_) => CoverageLifecycleCallbacks.Create(_assembly.Location, coverageCollector));
        builder.TestHost.AddDataConsumer((_) => CoverageConsumer.Create(coverageCollector));
        using var app = await builder.BuildAsync();
        return await app.RunAsync();
    }

    public async Task<int> MutantRun(MutantController mutantController, IEnumerable<string>? testCases, List<TestNode> executed)
    {
        List<string> args = [RunOptions.NoBanner, RunOptions.NoConsole];

        var testCaseFilter = GetTestCaseFilterString(testCases);

        if (testCaseFilter is not null)
        {
            args.Add(testCaseFilter);
        }
        
        var builder = await TestApplication.CreateBuilderAsync([.. args]);

        builder.AddMSTest(() => [_assembly]);
        builder.TestHost.AddTestApplicationLifecycleCallbacks((_) => MutantControlLifecycleCallbacks.Create(_assembly.Location, mutantController));
        builder.TestHost.AddDataConsumer((_) => MutantRunConsumer.Create(mutantController, executed));

        var app = await builder.BuildAsync();
        return await app.RunAsync();
    }
    
    private static string? GetTestCaseFilterString(IEnumerable<string>? testCases) =>
        testCases is null ? null : $"--filter {string.Join("|", testCases)}";
}
