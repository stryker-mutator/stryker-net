using System.Collections.Generic;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Reporting;

namespace Stryker.Core.Baseline.Utils;

public interface IBaselineMutantHelper
{
    IEnumerable<IMutant> GetMutantMatchingSourceCode(IEnumerable<IMutant> mutants, IJsonMutant baselineMutant, string baselineMutantSourceCode);

    string GetMutantSourceCode(string source, IJsonMutant baselineMutant);
}
