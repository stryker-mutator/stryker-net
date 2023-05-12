using System.Collections.Generic;
using System.Linq;
using Buildalyzer;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.Testing;

namespace Stryker.Core.Initialisation
{
    public interface IProjectFileReader
    {
        IAnalyzerResult AnalyzeProject(string projectFilePath,
            string solutionFilePath,
            string targetFramework,
            string msBuildPath = null);
        IAnalyzerManager AnalyzeSolution(string solutionPath);
    }

    /// <summary>
    /// This class is an abstraction of Buildalyzers AnalyzerManager to make the rest of our code better testable. Mocking AnalyzerManager is really ugly and we want to avoid it.
    /// </summary>
    public class ProjectFileReader : IProjectFileReader
    {
        private readonly INugetRestoreProcess _nugetRestoreProcess;
        private readonly IBuildalyzerProvider _analyzerProvider;
        private IAnalyzerManager _analyzerManager;
        private readonly ILogger _logger;

        public ProjectFileReader(
            INugetRestoreProcess nugetRestoreProcess = null,
            IBuildalyzerProvider analyzerProvider = null)
        {
            _nugetRestoreProcess = nugetRestoreProcess ?? new NugetRestoreProcess();
            _analyzerProvider = analyzerProvider ?? new BuildalyzerProvider();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectFileReader>();
        }

        private IAnalyzerManager AnalyzerManager
        {
            get
            {
                _analyzerManager ??= _analyzerProvider.Provide();
                return _analyzerManager;
            }
        }

        public IAnalyzerResult AnalyzeProject(string projectFilePath,
            string solutionFilePath,
            string targetFramework,
            string msBuildPath = null)
        {

            _logger.LogDebug("Analyzing project file {0}", projectFilePath);
            var analyzerResult = GetProjectInfo(projectFilePath, targetFramework);
            LogAnalyzerResult(analyzerResult);

            if (analyzerResult.Succeeded)
            {
                return analyzerResult;
            }
            if (analyzerResult.TargetsFullFramework())
            {
                // buildalyzer failed to find restored packages, retry after nuget restore
                _logger.LogDebug("Project analyzer result not successful, restoring packages");
                _nugetRestoreProcess.RestorePackages(solutionFilePath, msBuildPath);
                analyzerResult = GetProjectInfo(projectFilePath, targetFramework);
            }
            else
            {
                // buildalyzer failed, but seems to work anyway.
                _logger.LogDebug("Project analyzer result not successful");
            }

            return analyzerResult;
        }

        public IAnalyzerManager AnalyzeSolution(string solutionPath) => _analyzerProvider.Provide(solutionPath);

        /// <summary>
        /// Checks if project info is already present in solution projects. If not, analyze here.
        /// </summary>
        /// <returns></returns>
        private IAnalyzerResult GetProjectInfo(string projectFilePath,
            string targetFramework)
        {
            var analyzerResults = AnalyzerManager.GetProject(projectFilePath).Build();
            return SelectAnalyzerResult(analyzerResults, targetFramework);
        }

        private IAnalyzerResult SelectAnalyzerResult(IAnalyzerResults analyzerResults, string targetFramework)
        {
            var validResults = analyzerResults.Where(a => a.TargetFramework is not null);
            if (!validResults.Any())
            {
                throw new InputException("No valid project analysis results could be found.");
            }

            if (targetFramework is null)
            {
                return validResults.First();
            }

            var resultForRequestedFramework = validResults.FirstOrDefault(a => a.TargetFramework == targetFramework);
            if (resultForRequestedFramework is not null)
            {
                return resultForRequestedFramework;
            }

            var firstAnalyzerResult = validResults.First();
            var availableFrameworks = validResults.Select(a => a.TargetFramework).Distinct();
            var firstFramework = firstAnalyzerResult.TargetFramework;
            _logger.LogWarning(
                $"Could not find a project analysis for the chosen target framework {targetFramework}. \n" +
                $"The available target frameworks are: {string.Join(',', availableFrameworks)}. \n" +
                $"The first available framework will be selected, which is {firstFramework}.");

            return firstAnalyzerResult;
        }

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
