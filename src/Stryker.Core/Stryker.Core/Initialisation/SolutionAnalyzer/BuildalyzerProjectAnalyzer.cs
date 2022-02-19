using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;

namespace Stryker.Core.Initialisation.ProjectAnalyzer
{
    public class BuildalyzerProjectAnalyzer : IProjectAnalyzer
    {
        private readonly Buildalyzer.IProjectAnalyzer _projectAnalyzer;
        private readonly ILogger _logger;


        private IAnalysisResult ToAnalysisResult(Buildalyzer.IAnalyzerResult buildAlyzerResult)
        {
            var result = new AnalyzerResult(buildAlyzerResult.ProjectFilePath, buildAlyzerResult.References, buildAlyzerResult.ProjectReferences,
                                                    buildAlyzerResult.AnalyzerReferences, buildAlyzerResult.PreprocessorSymbols, buildAlyzerResult.Properties,
                                                    buildAlyzerResult.SourceFiles, buildAlyzerResult.Succeeded, buildAlyzerResult.TargetFramework);
            result.Log();
            return result;
        }

        public BuildalyzerProjectAnalyzer(Buildalyzer.IProjectAnalyzer projectAnalyzer)
        {
            _projectAnalyzer = projectAnalyzer;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<BuildalyzerProjectAnalyzer>();
        }

        public string ProjectFilePath
        {
            get { return _projectAnalyzer.ProjectFile.Path; }
        }

        public IAnalysisResult Analyze(string targetFramework)
        {
            var buildAlyzerResults = _projectAnalyzer.Build();
            Buildalyzer.IAnalyzerResult buildAlyzerResult = buildAlyzerResults.SingleOrDefault(result => result.TargetFramework == targetFramework);

            if (buildAlyzerResult == null)
            {
                buildAlyzerResult = buildAlyzerResults.First();

                if (!string.IsNullOrWhiteSpace(targetFramework))
                {
                    _logger.LogWarning(
                        "The configured target framework '{0}' isn't available for this project. " +
                        "It was analyzed with the first framework available " +
                        "which is {1}.", targetFramework, buildAlyzerResult.TargetFramework);
                }
            }
            return ToAnalysisResult(buildAlyzerResult);
        }
    }
}
