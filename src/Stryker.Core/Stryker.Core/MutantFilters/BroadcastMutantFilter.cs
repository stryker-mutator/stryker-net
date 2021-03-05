using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.MutantFilters
{
    public class BroadcastMutantFilter : IMutantFilter
    {
        public IEnumerable<IMutantFilter> MutantFilters { get; }

        public BroadcastMutantFilter(IEnumerable<IMutantFilter> mutantFilters)
        {
            MutantFilters = mutantFilters;
        }

        public string DisplayName => "broadcast filter";

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, ReadOnlyFileLeaf file, IStrykerOptions options)
        {
            IEnumerable<Mutant> mutantsToTest = mutants.ToList();

            // Then all other filters except diff filter
            foreach (var mutantFilter in MutantFilters.Where(f => !(f is DiffMutantFilter)))
            {
                // These mutants should be tested according to current filter
                var extraMutantsToTest = mutantFilter.FilterMutants(mutantsToTest, file, options);

                // All mutants that weren't filtered out by a previous filter but were by the current filter are set to Ignored
                foreach (var skippedMutant in mutantsToTest.Except(extraMutantsToTest))
                {
                    skippedMutant.ResultStatus = MutantStatus.Ignored;
                    skippedMutant.ResultStatusReason = $"Removed by {mutantFilter.DisplayName}";
                }

                mutantsToTest = extraMutantsToTest;
            }

            // Diff filter goes last if it is enabled
            if (MutantFilters.SingleOrDefault(f => f is DiffMutantFilter) is var diffFilter && diffFilter is { })
            {
                mutantsToTest = diffFilter.FilterMutants(mutantsToTest, file, options);
            }

            return mutantsToTest;
        }
    }
}