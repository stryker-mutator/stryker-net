using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Extensions.TestHost;
using Stryker.Core.TestRunners.MsTest.Setup;
using Stryker.Core.TestRunners.MsTest.Testing.Tests;
using Stryker.Core.TestRunners.MSTest.Setup;

namespace Stryker.Core.TestRunners.MsTest.Testing.LifecycleCallbacks;
internal class CoverageLifecycleCallbacks : ITestApplicationLifecycleCallbacks
{
    private readonly string _assemblyPath;
    private readonly CoverageCollector _coverageCollector;

    private CoverageLifecycleCallbacks(string assemblyPath, CoverageCollector coverageCollector)
    {
        _assemblyPath = assemblyPath;
        _coverageCollector = coverageCollector;
    }

    public static CoverageLifecycleCallbacks Create(string assemblyPath, CoverageCollector coverageCollector) =>
        new(assemblyPath, coverageCollector);

    public string Uid => nameof(CoverageLifecycleCallbacks);

    public string Version => "1.0.0";

    public string DisplayName => $"Stryker.{Uid}";

    public string Description => "Setup and cleanup for coverage run";

    public Task BeforeRunAsync(CancellationToken cancellationToken)
    {
        var projects = DirectoryScanner.FindProjects(_assemblyPath);

        // In case the assembly has not loaded yet.
        AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoaded;

        // Scan through assemblies containing the name of the .csproj files.
        var loadedProjects = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(assembly => projects.Contains(assembly.GetName().Name))
            .Where(assembly => !assembly.Location.EndsWith($"{AssemblyCopy.CopySuffix}.dll"))
            .Where(assembly => assembly.Location != _assemblyPath);

        foreach (var project in loadedProjects)
        {
            InitializeCoverageCollector(project);
        }

        return Task.CompletedTask;
    }


    public Task AfterRunAsync(int exitCode, CancellationToken cancellation)
    {
        // Disables capturing coverage
        _coverageCollector.SetCoverage(false);
        return Task.CompletedTask;
    }

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

    private void OnAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
    {
        var assembly = args.LoadedAssembly;
        InitializeCoverageCollector(assembly);
    }

    private void InitializeCoverageCollector(Assembly assembly)
    {
        var mutantControlType = assembly.DefinedTypes?.FirstOrDefault(t => t.FullName == _coverageCollector.MutantControlClassName);

        if (mutantControlType is null)
        {
            return;
        }

        // Sets active mutation to None
        var activeMutantField = mutantControlType.GetField("ActiveMutant");
        activeMutantField?.SetValue(null, -1);

        // Sets capture coverage to true
        _coverageCollector.CaptureCoverageField = mutantControlType.GetField("CaptureCoverage");
        _coverageCollector.SetCoverage(true);

        // Method to receive coverage
        _coverageCollector.GetCoverageDataMethod = mutantControlType.GetMethod("GetCoverageData")!;
    }
}
