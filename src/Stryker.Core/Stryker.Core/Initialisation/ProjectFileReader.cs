using System.Collections.Generic;
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
            string targetFramework);
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
            string targetFramework)
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
            var analyzerResults = _manager.GetProject(projectFilePath).Build();
            var analyzerResult = SelectAnalyzerResult(analyzerResults, targetFramework);

            LogAnalyzerResult(analyzerResult);

            if (!analyzerResult.Succeeded)
            {
                if (analyzerResult.TargetsFullFramework())
                {
                    // buildalyzer failed to find restored packages, retry after nuget restore
                    _logger.LogDebug("Project analyzer result not successful, restoring packages");
                    _nugetRestoreProcess.RestorePackages(solutionFilePath);
                    analyzerResult = _manager.GetProject(projectFilePath).Build(targetFramework).First();
                }
                else
                {
                    // buildalyzer failed, but seems to work anyway.
                    _logger.LogDebug("Project analyzer result not successful");
                }
            }

            return analyzerResult;
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

        private IAnalyzerResult SelectAnalyzerResult(IAnalyzerResults analyzerResults, string targetFramework)
        {
            if (targetFramework == null)
            {
                var analyzerResult = analyzerResults.FirstOrDefault(e => e.TargetFramework is not null);

                if (analyzerResult is null)
                {
                    throw new InputException("No analyzer result with a valid target framework could be found.");
                }

                return analyzerResult;
            }

            var analyzerResultForFramework = analyzerResults.SingleOrDefault(result => result.TargetFramework == targetFramework);
            if (analyzerResultForFramework is not null)
            {
                return analyzerResultForFramework;
            }

            var firstAnalyzerResult = analyzerResults.First(e => e.TargetFramework is not null);
            _logger.LogWarning(
                "The configured target framework '{0}' isn't available for this project. " +
                "It will be built against the first framework available " +
                "which is {1}.", targetFramework, firstAnalyzerResult.TargetFramework);

            return firstAnalyzerResult;
        }
    }
}
