using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.MutantFilters;

public class BroadcastMutantFilter : IMutantFilter
{
    public MutantFilter Type => MutantFilter.Broadcast;
    public IEnumerable<IMutantFilter> MutantFilters { get; }

    public BroadcastMutantFilter(IEnumerable<IMutantFilter> mutantFilters) => MutantFilters = mutantFilters;

    public string DisplayName => "broadcast filter";

    public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, IReadOnlyFileLeaf file, StrykerOptions options)
    {
        var mutantsToTest = mutants.Where(m => m.ResultStatus is not MutantStatus.Ignored);

        foreach (var mutantFilter in MutantFilters)
        {
            // These mutants should be tested according to current filter
            var remainingMutantsToTest = mutantFilter.FilterMutants(mutantsToTest, file, options);

            // All mutants that weren't filtered out by a previous filter but were by the current filter are set to Ignored
            foreach (var skippedMutant in mutantsToTest.Except(remainingMutantsToTest))
            {
                skippedMutant.ResultStatus = MutantStatus.Ignored;
                skippedMutant.ResultStatusReason = $"Removed by {mutantFilter.DisplayName}";
            }

            mutantsToTest = remainingMutantsToTest;
        }

        return mutantsToTest;
    }
}
