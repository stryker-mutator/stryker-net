using System.Linq;

namespace Stryker.Core.Initialisation.ProjectAnalyzer
{
    public class BuildalyzerProjectAnalyzer : IProjectAnalyzer
    {
        private readonly Buildalyzer.IProjectAnalyzer _projectAnalyzer;

        public BuildalyzerProjectAnalyzer(Buildalyzer.IProjectAnalyzer projectAnalyzer)
        {
            _projectAnalyzer = projectAnalyzer;
        }

        public string ProjectFilePath
        {
            get { return _projectAnalyzer.ProjectFile.Path; }
        }

        public IAnalysisResult Analyze()
        {
            var buildAlyzerResult = _projectAnalyzer.Build().FirstOrDefault();
            var analyzerResult = new AnalyzerResult(buildAlyzerResult.ProjectFilePath, buildAlyzerResult.References, buildAlyzerResult.ProjectReferences,
                                                    buildAlyzerResult.AnalyzerReferences, buildAlyzerResult.PreprocessorSymbols, buildAlyzerResult.Properties,
                                                    buildAlyzerResult.SourceFiles, buildAlyzerResult.Succeeded, buildAlyzerResult.TargetFramework);
            return analyzerResult;
        }
    }
}
