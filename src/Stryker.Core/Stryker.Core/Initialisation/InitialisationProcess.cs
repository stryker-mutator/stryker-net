using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;

namespace Stryker.Core.Initialisation
{
    // For mocking perposes
    public interface IInitialisationProcessProvider
    {
        IInitialisationProcess Provide();
    }

    [ExcludeFromCodeCoverage]
    public class InitialisationProcessProvider : IInitialisationProcessProvider
    {
        public IInitialisationProcess Provide()
        {
            return new InitialisationProcess();
        }
    }

    public interface IInitialisationProcess
    {
        MutationTestInput Initialize(IStrykerOptions options);
        int InitialTest(IStrykerOptions options);
    }

    public class InitialisationProcess : IInitialisationProcess
    {
        private readonly IInputFileResolver _inputFileResolver;
        private readonly IInitialBuildProcess _initialBuildProcess;
        private readonly IInitialTestProcess _initialTestProcess;
        private readonly IAssemblyReferenceResolver _assemblyReferenceResolver;
        private ITestRunner _testRunner;
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

        public MutationTestInput Initialize(IStrykerOptions options)
        {
            // resolve project info
            var projectInfo = _inputFileResolver.ResolveInput(options);

            // initial build
            var testProjects = projectInfo.TestProjectAnalyzerResults.ToList();
            for (var i = 0; i < testProjects.Count; i++)
            {
                _logger.LogInformation(
                    "Building test project {ProjectFilePath} ({CurrentTestProject}/{OfTotalTestProjects})",
                    testProjects[i].ProjectFilePath, i + 1,
                    projectInfo.TestProjectAnalyzerResults.Count());

                _initialBuildProcess.InitialBuild(
                    testProjects[i].GetTargetFramework() == Framework.DotNetClassic,
                    testProjects[i].ProjectFilePath,
                    options.SolutionPath);
            }

            InitializeDashboardReporterOptions(options, projectInfo);

            if (_testRunner == null)
            {
                _testRunner = new TestRunnerFactory().Create(options, options.Optimizations, projectInfo);
            }

            var input = new MutationTestInput()
            {
                ProjectInfo = projectInfo,
                AssemblyReferences = _assemblyReferenceResolver.LoadProjectReferences(projectInfo.ProjectUnderTestAnalyzerResult.References).ToList(),
                TestRunner = _testRunner
            };

            return input;
        }

        private void InitializeDashboardReporterOptions(IStrykerOptions options, ProjectInfo projectInfo)
        {
            // try to read the repository URL + version for the dashboard report
            var missingProjectName = options.ProjectName == null;
            var missingProjectVersion = options.ProjectVersion == null;
            var missingProjectInformation = (missingProjectName || missingProjectVersion) && options.Reporters.Contains(Reporter.Dashboard);
            if (missingProjectInformation)
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
                    throw new GeneralStrykerException($"Can't read {subject.ToLowerInvariant()} because the TargetPath property was not found in {projectFilePath}");
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
                catch (Exception e) when (e is not GeneralStrykerException)
                {
                    throw new GeneralStrykerException($"Failed to read {subject.ToLowerInvariant()} from {targetPath}", e);
                }
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
                throw new GeneralStrykerException($"Failed to retrieve the RepositoryUrl from the AssemblyMetadataAttribute of {module.FileName}", details);
            }

            const string schemeSeparator = "://";
            var indexOfScheme = repositoryUrl.IndexOf(schemeSeparator, StringComparison.Ordinal);
            if (indexOfScheme < 0)
            {
                throw new GeneralStrykerException($"Failed to compute the project name from the repository URL ({repositoryUrl}) because it doesn't contain a scheme ({schemeSeparator})", details);
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
                throw new GeneralStrykerException($"Failed to retrieve the AssemblyInformationalVersionAttribute of {module.FileName}", details);
            }

            return assemblyInformationalVersion;
        }

        public int InitialTest(IStrykerOptions options)
        {
            // initial test
            var initialTestDuration = _initialTestProcess.InitialTest(_testRunner);

            return new TimeoutValueCalculator().CalculateTimeoutValue(initialTestDuration, options.AdditionalTimeoutMS);
        }
    }
}
