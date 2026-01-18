using System.Diagnostics.CodeAnalysis;
using Buildalyzer;

namespace Stryker.Utilities.Buildalyzer;

/// <summary>
/// This is an interface to mock buildalyzer classes
/// </summary>
public interface IBuildalyzerProvider
{
    IAnalyzerManager Provide(AnalyzerManagerOptions options = null);
}

[ExcludeFromCodeCoverage]
public class BuildalyzerProvider : IBuildalyzerProvider
{
    public IAnalyzerManager Provide(AnalyzerManagerOptions options = null) => new AnalyzerManager(options);
}
