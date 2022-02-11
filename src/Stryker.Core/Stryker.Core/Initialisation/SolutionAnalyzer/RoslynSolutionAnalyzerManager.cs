using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stryker.Core.Initialisation.SolutionAnalyzer
{
    public class RoslynSolutionAnalyzerManager : ISolutionAnalyzerManager
    {
        public RoslynSolutionAnalyzerManager(string solutionFilePath)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyDictionary<string, IProjectAnalyzer> Projects { get; }
    }
}
