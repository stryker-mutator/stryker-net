using System.Diagnostics.CodeAnalysis;

namespace Stryker.Core.Initialisation.ProjectAnalyzer
{
    /// <summary>
    /// This is an interface to substitute or mock buildalyzer classes
    /// </summary>
    public interface IProjectsAnalyzerManagerProvider
    {
        IProjectsAnalyzerManager Provide(string filePath);
    }

    [ExcludeFromCodeCoverage]
    public class ProjectsAnalyzerManagerProvider : IProjectsAnalyzerManagerProvider
    {
        public IProjectsAnalyzerManager Provide(string filePath)
        {
            return new MsBuildProjectsAnalyzerManager(filePath);
        }
    }
}
