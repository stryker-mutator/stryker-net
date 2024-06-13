using System.Reflection;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions.Messages;
using Stryker.TestRunner.MSTest.Testing.Consumers;
using Stryker.TestRunner.MSTest.Testing.LifeTimeHandlers;
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
        var builder = await TestApplication.CreateBuilderAsync([RunOptions.DiscoverySettings]);
        builder.AddMSTest(() => [_assembly]);
        builder.TestHost.AddDataConsumer(_ => new DiscoveryConsumer(assemblyPath, new Uri(ExecutorPath), discoveryResult));
        using var app = await builder.BuildAsync();
        return await app.RunAsync();
    }

    public async Task<int> InitialTestRun(DiscoveryResult discoveryResult, List<TestNode> executed)
    {
        var builder = await TestApplication.CreateBuilderAsync([RunOptions.DiscoverySettings, RunOptions.NoBanner]);
        builder.AddMSTest(() => [_assembly]);
        builder.TestHost.AddDataConsumer((_) => new InitialTestRunConsumer(discoveryResult, executed));
        using var app = await builder.BuildAsync();
        return await app.RunAsync();
    }

    public async Task<int> CoverageRun(CoverageCollector coverageCollector)
    {
        var builder = await TestApplication.CreateBuilderAsync([]);
        builder.AddMSTest(() => [_assembly]);
        builder.TestHost.AddTestSessionLifetimeHandle((_) => new CoverageLifeTimeHandler(_assembly.Location, coverageCollector));
        builder.TestHost.AddDataConsumer((_) => new CoverageConsumer(coverageCollector));
        using var app = await builder.BuildAsync();
        return await app.RunAsync();
    }

    public async Task<int> MutantRun()
    {
        var builder = await TestApplication.CreateBuilderAsync([]);
        builder.AddMSTest(() => [_assembly]);
        builder.TestHost.AddDataConsumer((_) => new TestConsumer());
        var app = await builder.BuildAsync();
        return await app.RunAsync();
    }
}
