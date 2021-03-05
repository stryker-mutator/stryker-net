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
        IAnalyzerResult AnalyzeProject(string projectFilePath, string solutionFilePath);
    }

    public class ProjectFileReader : IProjectFileReader
    {
        private readonly INugetRestoreProcess _nugetRestoreProcess;
        private readonly ILogger _logger;

        public ProjectFileReader(INugetRestoreProcess nugetRestoreProcess = null)
        {
            _nugetRestoreProcess = nugetRestoreProcess ?? new NugetRestoreProcess();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectFileReader>();
        }

        public IAnalyzerResult AnalyzeProject(string projectFilePath, string solutionFilePath)
        {
            AnalyzerManager manager;
            if (solutionFilePath == null)
            {
                manager = new AnalyzerManager();
            }
            else
            {
                _logger.LogDebug("Analyzing solution file {0}", solutionFilePath);
                try
                {
                    manager = new AnalyzerManager(solutionFilePath);
                }
                catch (InvalidProjectFileException)
                {
                    throw new StrykerInputException($"Incorrect solution path \"{solutionFilePath}\". Solution file not found. Please review your solution path setting.");
                }
            }

            _logger.LogDebug("Analyzing project file {0}", projectFilePath);
            var analyzerResult = manager.GetProject(projectFilePath).Build().First();

            LogAnalyzerResult(analyzerResult);

            if (!analyzerResult.Succeeded)
            {
                if (analyzerResult.GetTargetFramework() == Framework.DotNetClassic)
                {
                    // buildalyzer failed to find restored packages, retry after nuget restore
                    _logger.LogDebug("Project analyzer result not successful, restoring packages");
                    _nugetRestoreProcess.RestorePackages(solutionFilePath);
                    analyzerResult = manager.GetProject(projectFilePath).Build().First();
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
            _logger.LogDebug("**** Buildalyzer result ****");

            _logger.LogDebug("Project: {0}", analyzerResult.ProjectFilePath);
            _logger.LogDebug("TargetFramework: {0}", analyzerResult.TargetFramework);

            foreach (var property in analyzerResult?.Properties ?? new Dictionary<string, string>())
            {
                _logger.LogDebug("Property {0}={1}", property.Key, property.Value);
            }
            foreach (var sourceFile in analyzerResult?.SourceFiles ?? Enumerable.Empty<string>())
            {
                _logger.LogDebug("SourceFile {0}", sourceFile);
            }
            foreach (var reference in analyzerResult?.References ?? Enumerable.Empty<string>())
            {
                _logger.LogDebug("References: {0}", reference);
            }
            _logger.LogDebug("Succeeded: {0}", analyzerResult.Succeeded);

            _logger.LogDebug("**** Buildalyzer result ****");
        }
    }
}
