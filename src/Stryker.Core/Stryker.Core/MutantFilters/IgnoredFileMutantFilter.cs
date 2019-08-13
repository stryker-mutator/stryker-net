using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.MutantFilters
{
    /// <summary>
    /// Checks if the origin file of a mutant should be skipped.
    /// </summary>
    /// <seealso cref="Stryker.Core.MutantFilters.IMutantFilter" />
    public class IgnoredFileMutantFilter : IMutantFilter
    {
        /// <inheritdoc />
        public string DisplayName => "file filter";

        /// <inheritdoc />
        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, FileLeaf file, StrykerOptions options)
        {
            if (file.IsExcluded)
            {
                return Enumerable.Empty<Mutant>();
            }

            return mutants;
        }
    }
}