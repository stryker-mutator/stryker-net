using System.Diagnostics.CodeAnalysis;

namespace Stryker.Core.Initialisation.ProjectAnalyzer
{
    /// <summary>
    /// This is an interface to substitute or mock buildalyzer classes
    /// </summary>
    public interface IProjectsAnalyzerManagerProvider
    {
        IProjectsAnalyzerManager Provide(string solutionFilePath);
    }

    [ExcludeFromCodeCoverage]
    public class ProjectsAnalyzerManagerProvider : IProjectsAnalyzerManagerProvider
    {
        public IProjectsAnalyzerManager Provide(string solutionFilePath)
        {
            return new BuildalyzerProjectsAnalyzerManager(solutionFilePath);
        }
    }
}
