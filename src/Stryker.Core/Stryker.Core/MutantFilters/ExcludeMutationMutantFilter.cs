using System.Collections.Generic;
using System.Linq;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;

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
    public IEnumerable<IMutant> FilterMutants(IEnumerable<IMutant> mutants, IReadOnlyFileLeaf file, IStrykerOptions options) => mutants.Where(mutant => !options.ExcludedMutations.Contains(mutant.Mutation.Type));
}
