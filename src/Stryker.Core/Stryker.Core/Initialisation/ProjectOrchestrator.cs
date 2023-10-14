using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using Stryker.Core.TestRunners.VsTest;

namespace Stryker.Core.Initialisation
{
    public interface IProjectOrchestrator: IDisposable
    {
        IEnumerable<IMutationTestProcess> MutateProjects(StrykerOptions options, IReporter reporters, ITestRunner runner = null);
    }
    
    public sealed class ProjectOrchestrator : IProjectOrchestrator
    {
        private IInitialisationProcess _initializationProcess;
        private readonly ILogger _logger;
        private readonly IProjectMutator _projectMutator;
        private readonly IInitialBuildProcess _initialBuildProcess;
        private readonly IInputFileResolver _fileResolver;
        private ITestRunner _runner;

        public ProjectOrchestrator(IProjectMutator projectMutator = null,
            IInitialBuildProcess initialBuildProcess = null,
            IInputFileResolver fileResolver = null,
            IInitialisationProcess initializationProcess = null)
        {
            _projectMutator = projectMutator ?? new ProjectMutator();
            _initialBuildProcess = initialBuildProcess ?? new InitialBuildProcess();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectOrchestrator>();
            _fileResolver = fileResolver ?? new InputFileResolver();
            _initializationProcess = initializationProcess;
        }

        public IEnumerable<IMutationTestProcess> MutateProjects(StrykerOptions options, IReporter reporters,
            ITestRunner runner = null)
        {

            _initializationProcess ??= new InitialisationProcess(_fileResolver, _initialBuildProcess);
            var projectInfos = _initializationProcess.GetMutableProjectsInfo(options);

            if (!projectInfos.Any())
            {
                _logger.LogWarning("No project to mutate. Stryker will exit prematurely.");
                yield break;
            }

            _initializationProcess.BuildProjects(options, projectInfos);

            // create a test runner
            _runner = runner ?? new VsTestRunnerPool(options, fileSystem: _fileResolver.FileSystem);

            InitializeDashboardProjectInformation(options, projectInfos.First());
            var inputs = _initializationProcess.GetMutationTestInputs(options, projectInfos, _runner);

            foreach (var mutationTestInput in inputs)
            {
                yield return _projectMutator.MutateProject(options, mutationTestInput, reporters);
            }
        }

        private void InitializeDashboardProjectInformation(StrykerOptions options, SourceProjectInfo projectInfo)
        {
            var dashboardReporterEnabled = options.Reporters.Contains(Reporter.Dashboard) || options.Reporters.Contains(Reporter.All);
            var dashboardBaselineEnabled = options.WithBaseline && options.BaselineProvider == BaselineProvider.Dashboard;
            var requiresProjectInformation = dashboardReporterEnabled || dashboardBaselineEnabled;
            var missingProjectName = string.IsNullOrEmpty(options.ProjectName);
            var missingProjectVersion = string.IsNullOrEmpty(options.ProjectVersion);
            if (!requiresProjectInformation || (!missingProjectVersion && !missingProjectName))
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
}
