using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.MutantFilters;

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
