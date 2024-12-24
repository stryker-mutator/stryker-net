using System.Diagnostics.CodeAnalysis;
using Buildalyzer;

namespace Stryker.Core.Testing;

/// <summary>
/// This is an interface to mock buildalyzer classes
/// </summary>
public interface IBuildalyzerProvider
{
    IAnalyzerManager Provide(AnalyzerManagerOptions options = null);
    IAnalyzerManager Provide(string solutionFilePath, AnalyzerManagerOptions options = null);
}

[ExcludeFromCodeCoverage]
public class BuildalyzerProvider : IBuildalyzerProvider
{
    public IAnalyzerManager Provide(AnalyzerManagerOptions options = null) => new AnalyzerManager(options);

    public IAnalyzerManager Provide(string solutionFilePath, AnalyzerManagerOptions options = null) => new AnalyzerManager(solutionFilePath, options);
}
