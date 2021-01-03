using System.Collections.Generic;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters.Json;

namespace Stryker.Core.Baseline.Utils
{
    public interface IBaselineMutantHelper
    {
        IEnumerable<Mutant> GetMutantMatchingSourceCode(IEnumerable<Mutant> mutants, JsonMutant baselineMutant, string baselineMutantSourceCode);

        string GetMutantSourceCode(string source, JsonMutant baselineMutant);
    }
}
