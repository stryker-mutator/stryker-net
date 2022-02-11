using System;
using System.Collections.Generic;

namespace Stryker.Core.Initialisation.SolutionAnalyzer
{
    public interface ISolutionAnalyzerManager
    {
        IReadOnlyDictionary<string, IProjectAnalyzer> Projects { get; }
    }
}
