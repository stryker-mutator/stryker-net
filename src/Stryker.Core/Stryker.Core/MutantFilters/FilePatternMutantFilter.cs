using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.MutantFilters
{
    /// <summary>
    /// Checks if the mutation should be skipped depending on the file and position of the mutation.
    /// </summary>
    /// <seealso cref="Stryker.Core.MutantFilters.IMutantFilter" />
    public class FilePatternMutantFilter : IMutantFilter
    {
        /// <inheritdoc />
        public string DisplayName => "file filter";

        /// <inheritdoc />
        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, FileLeaf file, StrykerOptions options)
        {
            var includePattern = options.FilePatterns.Where(x => !x.IsExclude).ToList();
            var excludePattern = options.FilePatterns.Where(x => x.IsExclude).ToList();

            return mutants.Where(IsMutantIncluded);

            bool IsMutantIncluded(Mutant mutant)
            {
                // Check if the the mutant is included.
                if (!includePattern.Any(MatchesPattern))
                {
                    return false;
                }

                // Check if the mutant is excluded.
                if (excludePattern.Any(MatchesPattern))
                {
                    return false;
                }

                return true;

                bool MatchesPattern(FilePattern pattern)
                {
                    // We check both the full and the relative path to allow for relative paths.
                    return pattern.IsMatch(file.FullPath, mutant.Mutation.OriginalNode.Span) ||
                           pattern.IsMatch(file.RelativePath, mutant.Mutation.OriginalNode.Span);
                }
            }
        }
    }
}