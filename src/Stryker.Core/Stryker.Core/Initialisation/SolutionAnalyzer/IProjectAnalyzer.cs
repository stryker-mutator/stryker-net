
namespace Stryker.Core.Initialisation.ProjectAnalyzer
{
    public interface IProjectAnalyzer
    {
        string ProjectFilePath { get; }
        IAnalysisResult Analyze(string targetFramework = null);
    }
}
