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
    public class FilePatternMutantFilter : IMutantFilter
    {
        public MutantFilter Type => MutantFilter.FilePattern;
        public string DisplayName => "mutate filter";
        private readonly IEnumerable<FilePattern> _includePattern;
        private readonly IEnumerable<FilePattern> _excludePattern;

        public FilePatternMutantFilter(StrykerOptions options)
        {
            _includePattern = options.Mutate.Where(x => !x.IsExclude).ToList();
            _excludePattern = options.Mutate.Where(x => x.IsExclude).ToList();
        }

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, IReadOnlyFileLeaf file, StrykerOptions options)
        {
            return mutants.Where(IsMutantIncluded);

            bool IsMutantIncluded(Mutant mutant)
            {
                // Check if the the mutant is included.
                if (!_includePattern.Any(MatchesPattern))
                {
                    return false;
                }

                // Check if the mutant is excluded.
                if (_excludePattern.Any(MatchesPattern))
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
