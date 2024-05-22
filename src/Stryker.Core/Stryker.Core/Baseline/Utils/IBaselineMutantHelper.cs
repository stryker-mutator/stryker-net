using System.Collections.Generic;
using Stryker.Core.Reporters.Json.SourceFiles;
using Stryker.Shared.Mutants;

namespace Stryker.Core.Baseline.Utils;

public interface IBaselineMutantHelper
{
    IEnumerable<IMutant> GetMutantMatchingSourceCode(IEnumerable<IMutant> mutants, JsonMutant baselineMutant, string baselineMutantSourceCode);

    string GetMutantSourceCode(string source, JsonMutant baselineMutant);
}
