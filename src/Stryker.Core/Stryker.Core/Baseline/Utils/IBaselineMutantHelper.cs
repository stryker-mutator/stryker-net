using System.Collections.Generic;
using Stryker.Abstractions.Reporters.Json.SourceFiles;
using Stryker.Abstractions.Mutants;

namespace Stryker.Abstractions.Baseline.Utils
{
    public interface IBaselineMutantHelper
    {
        IEnumerable<Mutant> GetMutantMatchingSourceCode(IEnumerable<Mutant> mutants, JsonMutant baselineMutant, string baselineMutantSourceCode);

        string GetMutantSourceCode(string source, JsonMutant baselineMutant);
    }
}
