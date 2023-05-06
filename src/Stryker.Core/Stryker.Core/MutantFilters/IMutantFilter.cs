using System.Collections.Generic;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.MutantFilters;

/// <summary>
/// Allows to filter a set of mutants before the mutants are tested.
/// </summary>
public interface IMutantFilter
{
    /// <summary>
    /// Filters the mutants.
    /// </summary>
    /// <param name="mutants">The mutants.</param>
    /// <param name="set">test description set</param>
    /// <param name="file">The origin file of the mutants.</param>
    /// <param name="options">The stryker options.</param>
    /// <returns>Return only the mutants that made it through the filter.</returns>
    IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, IReadOnlyFileLeaf file, StrykerOptions options);

    /// <summary>
    /// Gets the display name of the filter.
    /// </summary>
    /// <value>
    /// The display name.
    /// </value>
    string DisplayName { get; }

    /// <summary>
    /// The type of this mutant filter
    /// </summary>
    MutantFilter Type { get; }
}
