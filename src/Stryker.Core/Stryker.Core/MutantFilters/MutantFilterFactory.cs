using Stryker.Core.Clients;
using Stryker.Core.DashboardCompare;
using Stryker.Core.DiffProviders;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using System;
using System.Collections.Generic;

namespace Stryker.Core.MutantFilters
{
    public class MutantFilterFactory
    {
        private static IDiffProvider _diffProvider;
        private static IGitInfoProvider _gitInfoProvider;
        private static IDashboardClient _dashboardClient;

        public static IMutantFilter Create(StrykerOptions options, IDiffProvider diffProvider = null, IDashboardClient dashboardClient = null, IGitInfoProvider gitInfoProvider = null)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _diffProvider = diffProvider ?? new GitDiffProvider(options);
            _dashboardClient = dashboardClient ?? new DashboardClient(options);
            _gitInfoProvider = gitInfoProvider ?? new GitInfoProvider(options);

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
                enabledFilters.Add(new DiffMutantFilter(options, _diffProvider, dashboardClient: _dashboardClient, gitInfoProvider: _gitInfoProvider));
            }

            return enabledFilters;
        }
    }
}
