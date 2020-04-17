using Buildalyzer;
using Buildalyzer.Construction;
using Microsoft.Build.Construction;
using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Testing
{
    /// <summary>
    /// This is an interface to mock buildalyzer classes
    /// </summary>
    public interface IBuildalyzerProvider
    {
        IAnalyzerManager Provide(AnalyzerManagerOptions options = null);
        IAnalyzerManager Provide(string solutionFilePath, AnalyzerManagerOptions options = null);
    }

    public class BuildalyzerProvider : IBuildalyzerProvider
    {
        public IAnalyzerManager Provide(AnalyzerManagerOptions options = null)
        {
            return new AnalyzerManager(options);
        }

        public IAnalyzerManager Provide(string solutionFilePath, AnalyzerManagerOptions options = null)
        {
            return new AnalyzerManager(solutionFilePath, options);
        }
    }
}
