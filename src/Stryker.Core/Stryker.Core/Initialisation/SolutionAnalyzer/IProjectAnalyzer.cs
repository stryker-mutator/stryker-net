
namespace Stryker.Core.Initialisation.SolutionAnalyzer
{
    public interface IProjectAnalyzer
    {
        string ProjectFilePath { get; }
        IAnalyzerResult Build();
    }
}
