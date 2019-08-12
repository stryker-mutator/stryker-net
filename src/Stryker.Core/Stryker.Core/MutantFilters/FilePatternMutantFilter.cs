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

            // If there is no include pattern which matches the current file, no mutants will be included.
            if (!includePattern.Any(ip => ip.Glob.IsMatch(file.FullPath)))
            {
                return Enumerable.Empty<Mutant>();
            }

            return mutants.Where(IsMutantIncluded);

            bool IsMutantIncluded(Mutant mutant)
            {
                // Check if the the mutant is included.
                if (!includePattern.Any(ip => MatchesPattern(mutant, file, ip)))
                {
                    return false;
                }

                // Check if the mutant is excluded.
                return !excludePattern.Any(ep => MatchesPattern(mutant, file, ep));
            }
        }

        private bool MatchesPattern(Mutant mutant, FileLeaf file, FilePattern pattern)
        {
            // Check if the file path is matched.
            if (!pattern.Glob.IsMatch(file.FullPath))
            {
                return false;
            }

            // Check if the text spans match
            // If the intersection of the specified span and the span of the mutation returns the span of the mutation, the mutation is completely inside of the specified span.
            if (pattern.TextSpans.Any(span => span.Overlap(mutant.Mutation.OriginalNode.Span) == mutant.Mutation.OriginalNode.Span))
            {
                return true;
            }

            return false;
        }
    }
}