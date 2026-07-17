using System.Collections.Generic;
using Stryker.Abstractions;
using Stryker.Abstractions.Reporting;
using Stryker.Core.DiffProviders;

namespace Stryker.Core.Baseline.Utils;

public interface IContentMutantMatcher
{
    /// <summary>
    /// Maps a baseline mutant's original location through <paramref name="diff"/> to find its
    /// corresponding location in the current source, then returns the current mutants that occupy
    /// that location and were produced by the same mutator.
    /// </summary>
    IEnumerable<IMutant> MatchByLocation(IEnumerable<IMutant> currentMutants, IJsonMutant baselineMutant, DiffResult diff);
}
