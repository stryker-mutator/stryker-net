using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Buildalyzer;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using Stryker.Core.TestRunners.VsTest;

namespace Stryker.Core.Initialisation
{
    // For mocking purposes

    public interface IInitialisationProcess
    {
        MutationTestInput Initialize(StrykerOptions options, IEnumerable<IAnalyzerResult> solutionProjects);
        InitialTestRun InitialTest(StrykerOptions options);
    }

    public class InitialisationProcess : IInitialisationProcess
    {
        private readonly IInputFileResolver _inputFileResolver;
        private readonly IInitialBuildProcess _initialBuildProcess;
        private readonly IInitialTestProcess _initialTestProcess;
        private readonly IAssemblyReferenceResolver _assemblyReferenceResolver;
        private ITestRunner _testRunner;
        private ProjectInfo _projectInfo;
        private readonly ILogger _logger;
       

        public InitialisationProcess(
            IInputFileResolver inputFileResolver = null,
            IInitialBuildProcess initialBuildProcess = null,
            IInitialTestProcess initialTestProcess = null,
            ITestRunner testRunner = null,
            IAssemblyReferenceResolver assemblyReferenceResolver = null)
        {
            _inputFileResolver = inputFileResolver ?? new InputFileResolver();
            _initialBuildProcess = initialBuildProcess ?? new InitialBuildProcess();
            _initialTestProcess = initialTestProcess ?? new InitialTestProcess();
            _testRunner = testRunner;
            _assemblyReferenceResolver = assemblyReferenceResolver ?? new AssemblyReferenceResolver();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<InitialisationProcess>();
        }

        public MutationTestInput Initialize(StrykerOptions options, IEnumerable<IAnalyzerResult> solutionProjects)
        {
            // resolve project info
            _projectInfo = _inputFileResolver.ResolveInput(options, solutionProjects);

            // initial build
            if (!options.IsSolutionContext)
            {
                var testProjects = _projectInfo.TestProjectAnalyzerResults.ToList();
                for (var i = 0; i < testProjects.Count; i++)
                {
                    _logger.LogInformation(
                        "Building test project {ProjectFilePath} ({CurrentTestProject}/{OfTotalTestProjects})",
                        testProjects[i].ProjectFilePath, i + 1,
                        _projectInfo.TestProjectAnalyzerResults.Count());

                    _initialBuildProcess.InitialBuild(
                        testProjects[i].TargetsFullFramework(),
                        testProjects[i].ProjectFilePath,
                        options.SolutionPath,
                        options.MsBuildPath);
                }
            }

            InitializeDashboardProjectInformation(options, _projectInfo);

            _testRunner ??= new VsTestRunnerPool(options, _projectInfo);

            var input = new MutationTestInput
            {
                ProjectInfo = _projectInfo,
                AssemblyReferences = _assemblyReferenceResolver.LoadProjectReferences(_projectInfo.ProjectUnderTestAnalyzerResult.References).ToList(),
                TestRunner = _testRunner,
            };

            return input;
        }

        public InitialTestRun InitialTest(StrykerOptions options)
        {
            // initial test
            var result = _initialTestProcess.InitialTest(options, _testRunner);

            if (_testRunner.DiscoverTests().Count != 0)
            {
                return result;
            }
            // no test have been discovered, diagnose this
            DiagnoseLackOfDetectedTest(_projectInfo);
            if (options.IsSolutionContext)
            {
                return result;
            }
            throw new InputException("No test has been detected. Make sure your test project contains test and is compatible with VsTest."+string.Join(Environment.NewLine, _projectInfo.ProjectWarnings));
        }

        private static readonly Dictionary<string, string> TestFrameworks = new()
        {
            ["xunit.core"] = "xunit.runner.visualstudio",
            ["nunit.framework"] = "NUnit3.TestAdapter",
            ["Microsoft.VisualStudio.TestPlatform.TestFramework"] = "Microsoft.VisualStudio.TestPlatform.MSTest.TestAdapter"
        };

        public void DiagnoseLackOfDetectedTest(ProjectInfo projectInfo)
        {
            foreach (var testProject in projectInfo.TestProjectAnalyzerResults)
            {
                foreach (var (framework, adapter) in TestFrameworks)
                {
                    if (testProject.References.Any(r => r.Contains(framework)) && !testProject.References.Any(r => r.Contains(adapter)))
                    {
                        var message = $"Project '{testProject.ProjectFilePath}' did not report any test. This may be because it is missing an appropriate VstTest adapter for '{framework}'. " +
                                      $"Adding '{adapter}' to this project references may resolve the issue.";
                        projectInfo.ProjectWarnings.Add(message);
                        _logger.LogWarning(message);
                    }
                }
            }
        }


        private void InitializeDashboardProjectInformation(StrykerOptions options, ProjectInfo projectInfo)
        {
            var dashboardReporterEnabled = options.Reporters.Contains(Reporter.Dashboard) || options.Reporters.Contains(Reporter.All);
            var dashboardBaselineEnabled = options.WithBaseline && options.BaselineProvider == BaselineProvider.Dashboard;
            var requiresProjectInformation = dashboardReporterEnabled || dashboardBaselineEnabled;
            if (!requiresProjectInformation)
            {
                return;
            }

            // try to read the repository URL + version for the dashboard report or dashboard baseline
            var missingProjectName = string.IsNullOrEmpty(options.ProjectName);
            var missingProjectVersion = string.IsNullOrEmpty(options.ProjectVersion);
            if (missingProjectName || missingProjectVersion)
            {
                var subject = missingProjectName switch
                {
                    true when missingProjectVersion => "Project name and project version",
                    true => "Project name",
                    _ => "Project version"
                };
                var projectFilePath = projectInfo.ProjectUnderTestAnalyzerResult.ProjectFilePath;

                if (!projectInfo.ProjectUnderTestAnalyzerResult.Properties.TryGetValue("TargetPath", out var targetPath))
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
    }
}
