using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.DiffProviders;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;

namespace Stryker.Core.MutantFilters
{
    public static class MutantFilterFactory
    {
        private static IDiffProvider _diffProvider;
        private static IGitInfoProvider _gitInfoProvider;
        private static IBaselineProvider _baselineProvider;

        public static IMutantFilter Create(StrykerOptions options, MutationTestInput mutationTestInput,
            IDiffProvider diffProvider = null, IBaselineProvider baselineProvider = null,
            IGitInfoProvider gitInfoProvider = null)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _diffProvider = diffProvider ?? new GitDiffProvider(options, mutationTestInput.TestRunner.DiscoverTests());
            _baselineProvider = baselineProvider ?? BaselineProviderFactory.Create(options);
            _gitInfoProvider = gitInfoProvider ?? new GitInfoProvider(options);

            return new BroadcastMutantFilter(DetermineEnabledMutantFilters(options));
        }

        private static IEnumerable<IMutantFilter> DetermineEnabledMutantFilters(StrykerOptions options)
        {
            var enabledFilters = new SortedSet<IMutantFilter>(new ByMutantFilterType()) {
                    new FilePatternMutantFilter(options),
                    new IgnoredMethodMutantFilter(),
                    new IgnoreMutationMutantFilter(),
                    new ExcludeFromCodeCoverageFilter()
                };

            if (options.WithBaseline)
            {
                enabledFilters.Add(new BaselineMutantFilter(options, _baselineProvider, _gitInfoProvider));
            }
            if (options.Since || options.WithBaseline)
            {
                enabledFilters.Add(new SinceMutantFilter(_diffProvider));
            }
            if (options.ExcludedLinqExpressions.Any())
            {
                enabledFilters.Add(new ExcludeLinqExpressionFilter());
            }

            return enabledFilters;
        }

        private sealed class ByMutantFilterType : IComparer<IMutantFilter>
        {
            public int Compare(IMutantFilter x, IMutantFilter y) => x.Type.CompareTo(y.Type);
        }
    }
}
