using System.Collections.Generic;
using System.Linq;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions;
using Stryker.Abstractions.ProjectComponents;

namespace Stryker.Abstractions.MutantFilters
{
    /// <summary>
    /// Checks if the mutation type of the mutant should be excluded.
    /// </summary>
    /// <seealso cref="IMutantFilter" />
    public class IgnoreMutationMutantFilter : IMutantFilter
    {
        public MutantFilter Type => MutantFilter.IgnoreMutation;
        public string DisplayName => "mutation type filter";

        /// <inheritdoc />
        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, IReadOnlyFileLeaf file, StrykerOptions options) => mutants.Where(mutant => !options.ExcludedMutations.Contains(mutant.Mutation.Type));
    }
}
