using System.Diagnostics.CodeAnalysis;

namespace Stryker.Core.Initialisation.SolutionAnalyzer
{
    /// <summary>
    /// This is an interface to substitute or mock buildalyzer classes
    /// </summary>
    public interface ISolutionAnalyzerManagerProvider
    {
        ISolutionAnalyzerManager Provide(string solutionFilePath);
    }

    [ExcludeFromCodeCoverage]
    public class SolutionAnalyzerManagerProvider : ISolutionAnalyzerManagerProvider
    {
        public ISolutionAnalyzerManager Provide(string solutionFilePath)
        {
            return new BuildalyzerSolutionAnalyzerManager(solutionFilePath);
        }
    }
}
