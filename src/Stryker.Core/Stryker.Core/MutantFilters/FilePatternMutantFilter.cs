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

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, IReadOnlyFileLeaf file, StrykerOptions options)
        {
            return mutants.Where(IsMutantIncluded);

            bool IsMutantIncluded(Mutant mutant)
            {
                // if we do not have the original node, we cannot exclude the mutation according to its location
                if (mutant.Mutation.OriginalNode == null)
                {
                    return false;
                }

                var textSpan = mutant.Mutation.OriginalNode.Span;
                var mutantSpan = new MutantSpan(textSpan.Start, textSpan.End);
                return file.IsMatch(mutantSpan);
            }
        }
    }
}
