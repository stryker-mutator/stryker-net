using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.DiffProviders;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;

namespace Stryker.Core.MutantFilters
{
    public static class MutantFilterFactory
    {
        private static IDiffProvider _diffProvider;
        private static IGitInfoProvider _gitInfoProvider;
        private static IBaselineProvider _baselineProvider;
        private static MutationTestInput _input;
        private static readonly ILogger _logger;
        private static DiffResult _diffResult;
        private static TestSet _tests;

        static MutantFilterFactory()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger(nameof(MutantFilterFactory));
        }

        public static IMutantFilter Create(StrykerOptions options, MutationTestInput mutationTestInput,
                IDiffProvider diffProvider = null, IBaselineProvider baselineProvider = null,
                IGitInfoProvider gitInfoProvider = null)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _input = mutationTestInput;
            _diffProvider = diffProvider;
            _baselineProvider = baselineProvider;
            _gitInfoProvider = gitInfoProvider;

            return new BroadcastMutantFilter(DetermineEnabledMutantFilters(options));
        }

        private static IEnumerable<IMutantFilter> DetermineEnabledMutantFilters(StrykerOptions options)
        {
            var enabledFilters = new SortedSet<IMutantFilter>(new ByMutantFilterType()) {
                    new FilePatternMutantFilter(options),
                    new IgnoredMethodMutantFilter(),
                    new IgnoreMutationMutantFilter(),
                    new ExcludeFromCodeCoverageFilter(),
                    new IgnoreBlockMutantFilter(),
                };

            if (options.WithBaseline)
            {
                enabledFilters.Add(new BaselineMutantFilter(options,
                    _baselineProvider ?? BaselineProviderFactory.Create(options), _gitInfoProvider ?? new GitInfoProvider(options)));
            }
            if (options.Since || options.WithBaseline)
            {
                ScanDiff(options);
                enabledFilters.Add(new SinceMutantFilter(_diffResult, _tests));
            }
            if (options.ExcludedLinqExpressions.Any())
            {
                enabledFilters.Add(new ExcludeLinqExpressionFilter());
            }

            return enabledFilters;
        }

        private static void ScanDiff(StrykerOptions options)
        {
            if (_diffResult is null)
            {
                _tests = _input.TestRunner.GetTests(_input.SourceProjectInfo);
                _diffProvider ??= new GitDiffProvider(options, _gitInfoProvider);

                _diffResult = _diffProvider.ScanDiff();

                if (_diffResult is not null)
                {
                    _logger.LogInformation("{NumberOf} files changed",
                            (_diffResult.ChangedSourceFiles?.Count ?? 0) + (_diffResult.ChangedTestFiles?.Count ?? 0));

                    if (_diffResult.ChangedSourceFiles is not null)
                    {
                        foreach (var changedFile in _diffResult.ChangedSourceFiles)
                        {
                            _logger.LogInformation("Changed file {SourceFile}", changedFile);
                        }
                    }

                    if (_diffResult.ChangedTestFiles is not null)
                    {
                        foreach (var changedFile in _diffResult.ChangedTestFiles)
                        {
                            _logger.LogInformation("Changed test file {TestFile}", changedFile);
                        }
                    }
                }
            }
        }

        private sealed class ByMutantFilterType : IComparer<IMutantFilter>
        {
            public int Compare(IMutantFilter x, IMutantFilter y) => x?.Type.CompareTo(y?.Type) ?? -1;
        }
    }
}
