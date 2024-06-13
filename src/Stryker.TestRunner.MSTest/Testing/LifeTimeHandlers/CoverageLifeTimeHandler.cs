using System.Reflection;
using Microsoft.Testing.Platform.Extensions.TestHost;
using Microsoft.Testing.Platform.TestHost;
using Stryker.TestRunner.MSTest.Setup;
using Stryker.TestRunner.MSTest.Testing.Tests;

namespace Stryker.TestRunner.MSTest.Testing.LifeTimeHandlers;
internal class CoverageLifeTimeHandler : ITestSessionLifetimeHandler
{
    private readonly string _assemblyPath;
    private readonly CoverageCollector _coverageCollector;

    public CoverageLifeTimeHandler(string assemblyPath, CoverageCollector coverageCollector)
    {
        _assemblyPath = assemblyPath;
        _coverageCollector = coverageCollector;
    }

    public string Uid => nameof(CoverageLifeTimeHandler);

    public string Version => "1.0.0";

    public string DisplayName => "CoverageLifeTimeHandler";

    public string Description => "Handler for coverage";

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);
    public Task OnTestSessionFinishingAsync(SessionUid sessionUid, CancellationToken cancellationToken) => Task.FromResult(true);
    public Task OnTestSessionStartingAsync(SessionUid sessionUid, CancellationToken cancellationToken)
    {
        var projects = DirectoryScanner.FindProjects(_assemblyPath);

        AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoaded;

        var loadedProjects = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(assembly => projects.Contains(assembly.GetName().Name))
            .Where(assembly => !assembly.Location.EndsWith($"{AssemblyCopy.CopySuffix}.dll"))
            .Where(assembly => assembly.Location != _assemblyPath);

        foreach(var project in loadedProjects)
        {
            InitializeCoverageCollector(project);
        }

        return Task.FromResult(true);
    }


    private void OnAssemblyLoaded(object? sender, AssemblyLoadEventArgs args)
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

        _coverageCollector.ActiveMutantField = mutantControlType.GetField("ActiveMutant")!;
        _coverageCollector.CaptureCoverageField = mutantControlType.GetField("CaptureCoverage")!;
        _coverageCollector.GetCoverageDataMethod = mutantControlType.GetMethod("GetCoverageData")!;

        if (_coverageCollector.IsCoverageRun)
        {
            _coverageCollector.CaptureCoverageField.SetValue(null, true);
        }

        _coverageCollector.SetActiveMutation(CoverageCollector.AnyId);
    }
}
