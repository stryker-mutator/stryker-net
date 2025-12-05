using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Reporting;
using Stryker.Abstractions.Testing;
using Stryker.Core.MutationTest;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.TestRunner.VsTest;

namespace Stryker.Core.Initialisation;

public interface IProjectOrchestrator : IDisposable
{
    IEnumerable<IMutationTestProcess> MutateProjects(IStrykerOptions options, IReporter reporters, ITestRunner runner = null);
}

public sealed class ProjectOrchestrator : IProjectOrchestrator
{
    private IInitialisationProcess _initializationProcess;
    private readonly ILogger _logger;
    private readonly IProjectMutator _projectMutator;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMutationTestExecutor _mutationTestExecutor;
    private readonly IInputFileResolver _fileResolver;
    private ITestRunner _runner;

    public ProjectOrchestrator(
        IProjectMutator projectMutator,
        IInitialisationProcess initializationProcess,
        IInputFileResolver fileResolver,
        IServiceProvider serviceProvider,
        IMutationTestExecutor mutationTestExecutor,
        ILogger<ProjectOrchestrator> logger)
    {
        _projectMutator = projectMutator ?? throw new ArgumentNullException(nameof(projectMutator));
        _initializationProcess = initializationProcess ?? throw new ArgumentNullException(nameof(initializationProcess));
        _fileResolver = fileResolver ?? throw new ArgumentNullException(nameof(fileResolver));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _mutationTestExecutor = mutationTestExecutor ?? throw new ArgumentNullException(nameof(mutationTestExecutor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IEnumerable<IMutationTestProcess> MutateProjects(IStrykerOptions options, IReporter reporters,
        ITestRunner runner = null)
    {
        _initializationProcess ??= _serviceProvider.GetRequiredService<IInitialisationProcess>();
        var projectInfos = _initializationProcess.GetMutableProjectsInfo(options);

        if (!projectInfos.Any())
        {
            _logger.LogWarning("No project to mutate. Stryker will exit prematurely.");
            return [];
        }

        _initializationProcess.BuildProjects(options, projectInfos);

        // create a test runner
        _runner = runner ?? new VsTestRunnerPool(options, fileSystem: _fileResolver.FileSystem);
        _mutationTestExecutor.TestRunner = _runner;
        InitializeDashboardProjectInformation(options, projectInfos.First());
        var inputs = _initializationProcess.GetMutationTestInputs(options, projectInfos, _runner);

        var mutationTestProcesses = new ConcurrentBag<IMutationTestProcess>();
        Parallel.ForEach(inputs, mutationTestInput =>
        {
            mutationTestProcesses.Add(_projectMutator.MutateProject(options, mutationTestInput, reporters));
        });
        return mutationTestProcesses;
    }

    private void InitializeDashboardProjectInformation(IStrykerOptions options, SourceProjectInfo projectInfo)
    {
        var dashboardReporterEnabled = options.Reporters.Contains(Reporter.Dashboard) || options.Reporters.Contains(Reporter.All);
        var dashboardBaselineEnabled = options.WithBaseline && options.BaselineProvider == BaselineProvider.Dashboard;
        var requiresProjectInformation = dashboardReporterEnabled || dashboardBaselineEnabled;
        var missingProjectName = string.IsNullOrEmpty(options.ProjectName);
        var missingProjectVersion = string.IsNullOrEmpty(options.ProjectVersion);
        if (!requiresProjectInformation || !missingProjectVersion && !missingProjectName)
        {
            return;
        }

        // try to read the repository URL + version for the dashboard report or dashboard baseline
        var subject = missingProjectName switch
        {
            true when missingProjectVersion => "Project name and project version",
            true => "Project name",
            _ => "Project version"
        };
        var projectFilePath = projectInfo.AnalyzerResult.ProjectFilePath;

        if (!projectInfo.AnalyzerResult.Properties.TryGetValue("TargetPath", out var targetPath))
        {
            throw new InputException($"Can't read {subject.ToLowerInvariant()} because the TargetPath property was not found in {projectFilePath}");
        }

        _logger.LogTrace("{Subject} missing for the dashboard reporter, reading it from {TargetPath}. " +
                         "Note that this requires SourceLink to be properly configured in {ProjectPath}", subject, targetPath, projectFilePath);

        try
        {
            var targetName = Path.GetFileName(targetPath);
            using var module = ModuleDefinition.ReadModule(targetPath);

            var details = $"To solve this issue, either specify the {subject.ToLowerInvariant()} in the stryker configuration or configure [SourceLink](https://github.com/dotnet/sourcelink#readme) in {projectFilePath}";
            if (missingProjectName)
            {
                options.ProjectName = ReadProjectName(module, details);
                _logger.LogDebug("Using {ProjectName} as project name for the dashboard reporter. (Read from the AssemblyMetadata/RepositoryUrl assembly attribute of {TargetName})", options.ProjectName, targetName);
            }

            if (missingProjectVersion)
            {
                options.ProjectVersion = ReadProjectVersion(module, details);
                _logger.LogDebug("Using {ProjectVersion} as project version for the dashboard reporter. (Read from the AssemblyInformationalVersion assembly attribute of {TargetName})", options.ProjectVersion, targetName);
            }
        }
        catch (Exception e) when (e is not InputException)
        {
            throw new InputException($"Failed to read {subject.ToLowerInvariant()} from {targetPath} because of error {e.Message}");
        }
    }

    private static string ReadProjectName(ModuleDefinition module, string details)
    {
        if (module.Assembly.CustomAttributes
                .FirstOrDefault(e => e.AttributeType.Name == "AssemblyMetadataAttribute"
                                     && e.ConstructorArguments.Count == 2
                                     && e.ConstructorArguments[0].Value.Equals("RepositoryUrl"))?.ConstructorArguments[1].Value is not string repositoryUrl)
        {
            throw new InputException($"Failed to retrieve the RepositoryUrl from the AssemblyMetadataAttribute of {module.FileName}", details);
        }

        const string SchemeSeparator = "://";
        var indexOfScheme = repositoryUrl.IndexOf(SchemeSeparator, StringComparison.Ordinal);
        if (indexOfScheme < 0)
        {
            throw new InputException($"Failed to compute the project name from the repository URL ({repositoryUrl}) because it doesn't contain a scheme ({SchemeSeparator})", details);
        }

        return repositoryUrl[(indexOfScheme + SchemeSeparator.Length)..];
    }

    private static string ReadProjectVersion(ModuleDefinition module, string details)
    {
        if (module.Assembly.CustomAttributes
                .FirstOrDefault(e => e.AttributeType.Name == "AssemblyInformationalVersionAttribute"
                                     && e.ConstructorArguments.Count == 1)?.ConstructorArguments[0].Value is not string assemblyInformationalVersion)
        {
            throw new InputException($"Failed to retrieve the AssemblyInformationalVersionAttribute of {module.FileName}", details);
        }

        return assemblyInformationalVersion;
    }

    public void Dispose() => _runner?.Dispose();
}
