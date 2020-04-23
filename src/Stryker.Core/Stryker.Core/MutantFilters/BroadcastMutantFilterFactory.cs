using Stryker.Core.DiffProviders;
using Stryker.Core.Options;

namespace Stryker.Core.MutantFilters
{
    class BroadcastMutantFilterFactory
    {
        public static BroadcastMutantFilter Create(StrykerOptions options)
        {
            return new BroadcastMutantFilter(new IMutantFilter[]
            {
             new FilePatternMutantFilter(),
                    new IgnoredMethodMutantFilter(),
                    new ExcludeMutationMutantFilter(),
                    new DiffMutantFilter(options, new GitDiffProvider(options)),
                    new ExcludeFromCodeCoverageFilter()
            });
        }
    }
}
