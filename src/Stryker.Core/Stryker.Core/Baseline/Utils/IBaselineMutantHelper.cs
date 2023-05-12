namespace Stryker.Core.Baseline.Utils;
using System.Collections.Generic;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters.Json.SourceFiles;

public interface IBaselineMutantHelper
{
    IEnumerable<Mutant> GetMutantMatchingSourceCode(IEnumerable<Mutant> mutants, JsonMutant baselineMutant, string baselineMutantSourceCode);

    string GetMutantSourceCode(string source, JsonMutant baselineMutant);
}
