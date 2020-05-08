using Stryker.Core.DiffProviders;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using System;
using System.Collections.Generic;

namespace Stryker.Core.MutantFilters
{
    public class MutantFilterFactory
    {
        private IProjectMutantFilter _coverageMutantFilter;
        private IDiffProvider _diffProvider;
        private StrykerOptions _options;

        public IMutantFilter Create(IDiffProvider diffProvider = null)
        {
            if (_options == null)
            {
                throw new ArgumentNullException(nameof(_options));
            }

            _diffProvider = diffProvider ?? new GitDiffProvider(_options);

            return new BroadcastMutantFilter(DetermineEnabledMutantFilters(_options), _coverageMutantFilter ?? null);
        }

        private IEnumerable<IMutantFilter> DetermineEnabledMutantFilters(StrykerOptions options)
        {
            var enabledFilters = new List<IMutantFilter> {
                    new FilePatternMutantFilter(),
                    new IgnoredMethodMutantFilter(),
                    new ExcludeMutationMutantFilter(),
                    new ExcludeFromCodeCoverageFilter()
                };

            if (options.DiffEnabled)
            {
                enabledFilters.Add(new DiffMutantFilter(options, _diffProvider));
            }

            return enabledFilters;
        }

        public MutantFilterFactory WithOptions(StrykerOptions options)
        {
            _options = options;
            return this;
        }

        public MutantFilterFactory WithCoverageMutantFilter(IMutationTestExecutor mutationTestExecutor, MutationTestInput input)
        {
            _coverageMutantFilter = new CoverageProjectMutantFilter(_options, mutationTestExecutor, input);

            return this;
        }
    }
}
