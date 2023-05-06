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
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using Stryker.Core.TestRunners.VsTest;

namespace Stryker.Core.Initialisation;

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
    private ITestRunner _testRunner;
    private TestProjectsInfo _testProjectsInfo;
    private readonly ILogger _logger;


    public InitialisationProcess(
        IInputFileResolver inputFileResolver = null,
        IInitialBuildProcess initialBuildProcess = null,
        IInitialTestProcess initialTestProcess = null,
        ITestRunner testRunner = null)
    {
        _inputFileResolver = inputFileResolver ?? new InputFileResolver();
        _initialBuildProcess = initialBuildProcess ?? new InitialBuildProcess();
        _initialTestProcess = initialTestProcess ?? new InitialTestProcess();
        _testRunner = testRunner;
        _logger = ApplicationLogging.LoggerFactory.CreateLogger<InitialisationProcess>();
    }

    public MutationTestInput Initialize(StrykerOptions options, IEnumerable<IAnalyzerResult> solutionProjects)
    {
        // resolve project info
        _testProjectsInfo = _inputFileResolver.ResolveTestProjectsInfo(options, solutionProjects);
        var targetProjectInfo = _inputFileResolver.ResolveSourceProjectInfo(options, _testProjectsInfo, solutionProjects);

        // initial build
        if (!options.IsSolutionContext)
        {
            var testProjects = _testProjectsInfo.AnalyzerResults.ToList();
            for (var i = 0; i < testProjects.Count; i++)
            {
                _logger.LogInformation(
                    "Building test project {ProjectFilePath} ({CurrentTestProject}/{OfTotalTestProjects})",
                    testProjects[i].ProjectFilePath, i + 1,
                    testProjects.Count);

                _initialBuildProcess.InitialBuild(
                    testProjects[i].TargetsFullFramework(),
                    testProjects[i].ProjectFilePath,
                    options.SolutionPath,
                    options.MsBuildPath);
            }
        }

        InitializeDashboardProjectInformation(options, targetProjectInfo);

        if (_testRunner == null)
        {
            _testRunner = new VsTestRunnerPool(options, _testProjectsInfo);
        }

        var input = new MutationTestInput
        {
            TestProjectsInfo = _testProjectsInfo,
            SourceProjectInfo = targetProjectInfo,
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
        DiagnoseLackOfDetectedTest(_testProjectsInfo);
        throw new InputException("No test has been detected. Make sure your test project contains test and is compatible with VsTest.");
    }

    private static readonly Dictionary<string, string> TestFrameworks = new()
    {
        ["xunit.core"] = "xunit.runner.visualstudio",
        ["nunit.framework"] = "NUnit3.TestAdapter",
        ["Microsoft.VisualStudio.TestPlatform.TestFramework"] = "Microsoft.VisualStudio.TestPlatform.MSTest.TestAdapter"
    };

    public static void DiagnoseLackOfDetectedTest(TestProjectsInfo testProjectsInfo)
    {
        foreach (var testProject in testProjectsInfo.AnalyzerResults)
        {
            foreach (var (framework, adapter) in TestFrameworks)
            {
                if (testProject.References.Any(r => r.Contains(framework)) && !testProject.References.Any(r => r.Contains(adapter)))
                {
                    throw new InputException($"Project '{testProject.ProjectFilePath}' is not supported by VsTest because it is missing an appropriate VstTest adapter for '{framework}'. " +
                        $"Adding '{adapter}' to this project references may resolve the issue.");
                }
            }
        }
    }

    private void InitializeDashboardProjectInformation(StrykerOptions options, SourceProjectInfo projectInfo)
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
            InnerInitializeDashboardProjectInformation(options, projectInfo, missingProjectName, missingProjectVersion);
        }
    }

    private void InnerInitializeDashboardProjectInformation(StrykerOptions options, SourceProjectInfo projectInfo, bool missingProjectName, bool missingProjectVersion)
    {
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
        var repositoryUrl = module.Assembly.CustomAttributes
            .FirstOrDefault(e => e.AttributeType.Name == "AssemblyMetadataAttribute"
                                 && e.ConstructorArguments.Count == 2
                                 && e.ConstructorArguments[0].Value.Equals("RepositoryUrl"))?.ConstructorArguments[1].Value as string;

        if (repositoryUrl == null)
        {
            throw new InputException($"Failed to retrieve the RepositoryUrl from the AssemblyMetadataAttribute of {module.FileName}", details);
        }

        const string schemeSeparator = "://";
        var indexOfScheme = repositoryUrl.IndexOf(schemeSeparator, StringComparison.Ordinal);
        if (indexOfScheme < 0)
        {
            throw new InputException($"Failed to compute the project name from the repository URL ({repositoryUrl}) because it doesn't contain a scheme ({schemeSeparator})", details);
        }

        return repositoryUrl.Substring(indexOfScheme + schemeSeparator.Length);
    }

    private static string ReadProjectVersion(ModuleDefinition module, string details)
    {
        var assemblyInformationalVersion = module.Assembly.CustomAttributes
            .FirstOrDefault(e => e.AttributeType.Name == "AssemblyInformationalVersionAttribute"
                                 && e.ConstructorArguments.Count == 1)?.ConstructorArguments[0].Value as string;

        if (assemblyInformationalVersion == null)
        {
            throw new InputException($"Failed to retrieve the AssemblyInformationalVersionAttribute of {module.FileName}", details);
        }

        return assemblyInformationalVersion;
    }
}
