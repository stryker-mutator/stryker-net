using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions.Messages;
using Stryker.Core.TestRunners.MsTest.Testing.Consumers;
using Stryker.Core.TestRunners.MsTest.Testing.LifecycleCallbacks;
using Stryker.Core.TestRunners.MsTest.Testing.Options;
using Stryker.Core.TestRunners.MsTest.Testing.Results;
using Stryker.Core.TestRunners.MsTest.Testing.Tests;
using Stryker.Core.TestRunners.MSTest.Testing.Consumers;

namespace Stryker.Core.TestRunners.MsTest.Testing.TestProjects;
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

    public async Task<int> Discover(DiscoveryResult discoveryResult, List<TestNode> executed, string assemblyPath)
    {
        var builder = await TestApplication.CreateBuilderAsync([RunOptions.DiscoverySettings, RunOptions.NoBanner, RunOptions.NoConsole]);
        builder.AddMSTest(() => [_assembly]);
        builder.TestHost.AddDataConsumer(_ => DiscoveryConsumer.Create(assemblyPath, new Uri(ExecutorPath), discoveryResult, executed));
        using var app = await builder.BuildAsync();
        return await app.RunAsync();
    }

    public async Task<int> InitialTestRun(DiscoveryResult discoveryResult, List<TestNode> executed)
    {
        var builder = await TestApplication.CreateBuilderAsync([RunOptions.RunSettings, RunOptions.NoBanner, RunOptions.NoConsole]);
        builder.AddMSTest(() => [_assembly]);
        builder.TestHost.AddDataConsumer((_) => InitialTestRunConsumer.Create(discoveryResult, executed));
        using var app = await builder.BuildAsync();
        return await app.RunAsync();
    }

    public async Task<int> CoverageRun(CoverageCollector coverageCollector)
    {
        var builder = await TestApplication.CreateBuilderAsync([RunOptions.CoverageSettings, RunOptions.NoConsole, RunOptions.NoBanner]);
        builder.AddMSTest(() => [_assembly]);
        builder.TestHost.AddTestApplicationLifecycleCallbacks((_) => CoverageLifecycleCallbacks.Create(_assembly.Location, coverageCollector));
        builder.TestHost.AddDataConsumer((_) => CoverageConsumer.Create(coverageCollector));
        using var app = await builder.BuildAsync();
        return await app.RunAsync();
    }

    public async Task<int> MutantRun(MutantController mutantController, IEnumerable<string>? testCases, List<TestNode> executed)
    {
        List<string> args = [RunOptions.RunSettings, RunOptions.NoBanner, RunOptions.NoConsole];

        var testCaseFilter = GetTestCaseFilterString(testCases);

        if (testCaseFilter is not null)
        {
            args.Add(testCaseFilter);
        }

        var builder = await TestApplication.CreateBuilderAsync([.. args]);

        builder.AddMSTest(() => [_assembly]);
        builder.TestHost.AddTestApplicationLifecycleCallbacks((_) => MutantControlLifecycleCallbacks.Create(_assembly.Location, mutantController));
        builder.TestHost.AddDataConsumer((_) => MutantRunConsumer.Create(executed));

        var app = await builder.BuildAsync();
        return await app.RunAsync();
    }

    private static string GetTestCaseFilterString(IEnumerable<string>? testCases) =>
        testCases is null ? null : $"--filter {string.Join("|", testCases)}";
}
