using System;
using System.Collections.Generic;

namespace Stryker.Core.Initialisation.ProjectAnalyzer
{
    public interface IProjectsAnalyzerManager
    {
        IReadOnlyDictionary<string, IProjectAnalyzer> Projects { get; }
    }
}
