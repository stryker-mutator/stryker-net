using System.Diagnostics.CodeAnalysis;

namespace Stryker.Core.Initialisation.ProjectAnalyzer
{
    /// <summary>
    /// This is an interface to substitute or mock buildalyzer classes
    /// </summary>
    public interface ISolutionAnalyzerManagerProvider
    {
        IProjectsAnalyzerManager Provide(string solutionFilePath);
    }

    [ExcludeFromCodeCoverage]
    public class SolutionAnalyzerManagerProvider : ISolutionAnalyzerManagerProvider
    {
        public IProjectsAnalyzerManager Provide(string solutionFilePath)
        {
            return new BuildalyzerProjectsAnalyzerManager(solutionFilePath);
        }
    }
}
