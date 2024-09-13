using System.Collections.Generic;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;

namespace Stryker.Core.MutantFilters
{
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
        IEnumerable<IMutant> FilterMutants(IEnumerable<IMutant> mutants, IReadOnlyFileLeaf file, IStrykerOptions options);

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
}
