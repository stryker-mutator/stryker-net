using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.MutantFilters
{
    public class ExcludeMutationMutantFilter : IMutantFilter
    {
        /// <inheritdoc />
        public string DisplayName => "mutation type filter";

        /// <inheritdoc />
        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, FileLeaf file, StrykerOptions options)
        {
            return mutants.Where(mutant => !options.ExcludedMutations.Contains(mutant.Mutation.Type));
        }
    }
}