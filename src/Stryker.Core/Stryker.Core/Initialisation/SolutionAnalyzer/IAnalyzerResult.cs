using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stryker.Core.Initialisation.SolutionAnalyzer
{
    public interface IAnalyzerResult
    {
        string ProjectFilePath { get; }

        IEnumerable<string> References { get; }

        IEnumerable<string> ProjectReferences { get; }

        IEnumerable<string> AnalyzerReferences { get; }
       
        IEnumerable<string> PreprocessorSymbols { get; }

        IReadOnlyDictionary<string, string> Properties { get; }

        string[] SourceFiles { get; }

        bool Succeeded { get; }

        string TargetFramework { get; }
    }
}
