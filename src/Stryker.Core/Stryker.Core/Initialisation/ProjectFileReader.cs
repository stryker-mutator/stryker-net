using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Buildalyzer;
using Microsoft.Build.Exceptions;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;

namespace Stryker.Core.Initialisation
{
    public interface IProjectFileReader
    {
        IAnalyzerResult AnalyzeProject(
            string projectFilePath,
            string solutionFilePath,
            string targetFramework,
            IEnumerable<IAnalyzerResult> solutionProjects);
    }

    public class ProjectFileReader : IProjectFileReader
    {
        private readonly INugetRestoreProcess _nugetRestoreProcess;
        private IAnalyzerManager _manager;
        private readonly ILogger _logger;

        public ProjectFileReader(
            INugetRestoreProcess nugetRestoreProcess = null,
            IAnalyzerManager manager = null)
        {
            _nugetRestoreProcess = nugetRestoreProcess ?? new NugetRestoreProcess();
            _manager = manager ?? new AnalyzerManager();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectFileReader>();
        }

        public IAnalyzerResult AnalyzeProject(
            string projectFilePath,
            string solutionFilePath,
            string targetFramework,
            IEnumerable<IAnalyzerResult> solutionProjects)
        {
            if (solutionFilePath != null)
            {
                _logger.LogDebug("Analyzing solution file {0}", solutionFilePath);
                try
                {
                    _manager = new AnalyzerManager(solutionFilePath);
                }
                catch (InvalidProjectFileException)
                {
                    throw new InputException($"Incorrect solution path \"{solutionFilePath}\". Solution file not found. Please review your solution path setting.");
                }
            }

            _logger.LogDebug("Analyzing project file {0}", projectFilePath);
            IAnalyzerResult analyzerResult = GetProjectInfo(projectFilePath, targetFramework, solutionProjects);
            LogAnalyzerResult(analyzerResult);

            if (!analyzerResult.Succeeded)
            {
                if (analyzerResult.TargetsFullFramework())
                {
                    // buildalyzer failed to find restored packages, retry after nuget restore
                    _logger.LogDebug("Project analyzer result not successful, restoring packages");
                    _nugetRestoreProcess.RestorePackages(solutionFilePath);
                    analyzerResult = GetProjectInfo(projectFilePath, targetFramework, solutionProjects);
                }
                else
                {
                    // buildalyzer failed, but seems to work anyway.
                    _logger.LogDebug("Project analyzer result not successful");
                }
            }

            return analyzerResult;
        }

        /// <summary>
        /// Checks if project info is already present in solution projects. If not, analyze here.
        /// </summary>
        /// <returns></returns>
        private IAnalyzerResult GetProjectInfo(string projectFilePath,
            string targetFramework,
            IEnumerable<IAnalyzerResult> solutionProjects)
        {
            if (solutionProjects != null)
            {
                return solutionProjects.FirstOrDefault(x => x.ProjectFilePath == projectFilePath);
            }
            else
            {
                var analyzerResults = _manager.GetProject(projectFilePath).Build();
                return SelectAnalyzerResult(analyzerResults, targetFramework);
            }
        }

        private IAnalyzerResult SelectAnalyzerResult(IAnalyzerResults analyzerResults, string targetFramework)
        {
            if (!analyzerResults.Any() || analyzerResults.All(a => a.TargetFramework is null))
            {
                throw new InputException("No valid project analysis results could be found.");
            }

            if (targetFramework is not null)
            {
                return analyzerResults.FirstOrDefault(a => a.TargetFramework == targetFramework) ??
                    throw new InputException($"Could not find a project analysis for chosen target framework {targetFramework}. \n" +
                    $"The available target frameworks are {analyzerResults.Select(a => a.TargetFramework).Distinct()}");
            }

            return analyzerResults.First(a => a.TargetFramework is not null);
        }

        [ExcludeFromCodeCoverage]
        private void LogAnalyzerResult(IAnalyzerResult analyzerResult)
        {
            // dump all properties as it can help diagnosing build issues for user project.
            _logger.LogTrace("**** Buildalyzer result ****");

            _logger.LogTrace("Project: {0}", analyzerResult.ProjectFilePath);
            _logger.LogTrace("TargetFramework: {0}", analyzerResult.TargetFramework);

            foreach (var property in analyzerResult?.Properties ?? new Dictionary<string, string>())
            {
                _logger.LogTrace("Property {0}={1}", property.Key, property.Value);
            }
            foreach (var sourceFile in analyzerResult?.SourceFiles ?? Enumerable.Empty<string>())
            {
                _logger.LogTrace("SourceFile {0}", sourceFile);
            }
            foreach (var reference in analyzerResult?.References ?? Enumerable.Empty<string>())
            {
                _logger.LogTrace("References: {0}", reference);
            }
            _logger.LogTrace("Succeeded: {0}", analyzerResult.Succeeded);

            _logger.LogTrace("**** Buildalyzer result ****");
        }
    }
}
