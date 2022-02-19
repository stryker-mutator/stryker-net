using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation.ProjectAnalyzer;
using Stryker.Core.Logging;

namespace Stryker.Core.Initialisation
{
    public interface IProjectFileReader
    {
        IAnalysisResult AnalyzeProject(
            string projectFilePath,
            string solutionFilePath,
            string targetFramework);
    }

    public class ProjectFileReader : IProjectFileReader
    {
        private readonly ILogger _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectFileReader>();

        public IAnalysisResult AnalyzeProject(
            string projectFilePath,
            string solutionFilePath,
            string targetFramework)
        {
            string filePath = solutionFilePath;
            if (filePath == null)
            {
                filePath = projectFilePath;
            }

            ProjectsAnalyzerManagerProvider provider = new();
            var projectsAnalyzer = provider.Provide(filePath);

            _logger.LogDebug("Analyzing project file {0}", projectFilePath);
            var analyzerResult = projectsAnalyzer.Projects[projectFilePath].Analyze(targetFramework);

            if (!analyzerResult.Succeeded)
            {
                if (analyzerResult.TargetsFullFramework())
                {
                    // buildalyzer failed to find restored packages, retry after nuget restore
                    _logger.LogDebug("Project analyzer result not successful, restoring packages");
                    var nugetRestoreProcess = new NugetRestoreProcess();
                    nugetRestoreProcess.RestorePackages(solutionFilePath);

                    analyzerResult = projectsAnalyzer.Projects[projectFilePath].Analyze(targetFramework);
                }
                else
                {
                    // buildalyzer failed, but seems to work anyway.
                    _logger.LogDebug("Project analyzer result not successful");
                }
            }

            return analyzerResult;
        }
    }
}
