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

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, FileLeaf file, StrykerOptions options)
        {
            IEnumerable<Mutant> filteredMutants = mutants;

            foreach (var mutantFilter in MutantFilters)
            {
                var current = mutantFilter.FilterMutants(mutants, file, options);

                foreach (var skippedMutant in filteredMutants.Except(current))
                {
                    if (skippedMutant.ResultStatus == MutantStatus.NotRun)
                    {
                        skippedMutant.ResultStatus = MutantStatus.Ignored;
                        skippedMutant.ResultStatusReason = $"Removed by {mutantFilter.DisplayName}";
                    }
                }

                filteredMutants = current;
            }

            return filteredMutants;
        }
    }
}