using Stryker.Core.DiffProviders;
using Stryker.Core.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.MutantFilters
{
    public static class MutantFilterFactory
    {
        public static IDiffProvider DiffProvider { get; private set; }

        public static IMutantFilter Create(StrykerOptions options)
        {
            if(options == null)
            {
                throw new ArgumentNullException($"{nameof(options)} cannot be null");
            }

            return new BroadcastMutantFilter(DetermineEnabledMutantFilters(options));
        }

        private static IEnumerable<IMutantFilter> DetermineEnabledMutantFilters(StrykerOptions options)
        {
            var enabledFilters = new List<IMutantFilter> {
                    new FilePatternMutantFilter(),
                    new IgnoredMethodMutantFilter(),
                    new ExcludeMutationMutantFilter(),
                    new ExcludeFromCodeCoverageFilter()
                };

            if (options.DiffEnabled)
            {
                enabledFilters.Add(new DiffMutantFilter(DiffProvider ?? new GitDiffProvider(options)));
            }
            

            return enabledFilters;

           
        }

        public static void SetDiffProvider(IDiffProvider diffProvider)
        {
            DiffProvider = diffProvider;
        }
    }
}
