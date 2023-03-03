using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.MutantFilters
{
    public class IgnoreBlockMutantFilter : IMutantFilter
    {
        private readonly HashSet<MutantStatus> _inactiveStatuses;

        public string DisplayName => "block already covered filter";

        public MutantFilter Type => MutantFilter.IgnoreBlockRemoval;

        public IgnoreBlockMutantFilter()
        {
            _inactiveStatuses = new HashSet<MutantStatus>
            {
                MutantStatus.Ignored,
                MutantStatus.CompileError,
            };
        }

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, IReadOnlyFileLeaf file, StrykerOptions options)
        {
            var blockMutants = mutants.Where(m => m.Mutation.Type == Mutator.Block);
            var mutantsToIgnore = blockMutants.Where(blockMutant => mutants.Any(
                m => m != blockMutant
                && !_inactiveStatuses.Contains(m.ResultStatus)
                && blockMutant.Span.Value.Contains(m.Span.Value)
            ));

            return mutants.Except(mutantsToIgnore);
        }
    }
}
