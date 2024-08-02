using System.Collections.Generic;
using Stryker.Configuration.Mutants;
using Stryker.Configuration.Reporters.Json.SourceFiles;

namespace Stryker.Configuration.Baseline.Utils
{
    public interface IBaselineMutantHelper
    {
        IEnumerable<Mutant> GetMutantMatchingSourceCode(IEnumerable<Mutant> mutants, JsonMutant baselineMutant, string baselineMutantSourceCode);

        string GetMutantSourceCode(string source, JsonMutant baselineMutant);
    }
}
