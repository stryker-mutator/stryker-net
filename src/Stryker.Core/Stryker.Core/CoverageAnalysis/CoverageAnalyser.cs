using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.TestRunners;

namespace Stryker.Core.CoverageAnalysis
{
    public class CoverageAnalyser : ICoverageAnalyser
    {
        private readonly ILogger<CoverageAnalyser> _logger;
        private readonly StrykerOptions _options;

        public CoverageAnalyser(StrykerOptions options)
        {
            _options = options;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<CoverageAnalyser>();
        }

        public void DetermineTestCoverage(ITestRunner runner, IEnumerable<Mutant> mutants)
        {
            if (!_options.OptimizationMode.HasFlag(OptimizationModes.SkipUncoveredMutants) &&
                !_options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest))
            {
                foreach (var mutant in mutants)
                {
                    mutant.CoveringTests = TestsGuidList.EveryTest();
                }
                return;
            }

            ParseCoverage(runner.CaptureCoverage(), mutants);
        }

        private void ParseCoverage(IEnumerable<CoverageRunResult> coverage, IEnumerable<Mutant> mutantsToScan)
        {
            //mutantsToScan.ForEach( m => m.ResetCoverage());
            var coveringMap = new Dictionary<int, ISet<Guid>>();
            var trustedCoveringMap = new Dictionary<int, ISet<Guid>>();
            foreach (var mutant in mutantsToScan)
            {
                coveringMap[mutant.Id] = new HashSet<Guid>();
                trustedCoveringMap[mutant.Id] = new HashSet<Guid>();
            }
            var staticMutants = new HashSet<int>();
            var dubiousTests = new List<Guid>();
            var leakedMutations = new HashSet<int>();
            var testIds = coverage.Select(c => c.TestId).ToList();
            // process coverage results
            foreach (var coverageRunResult in coverage)
            {
                var id = coverageRunResult.TestId;
                switch (coverageRunResult.Confidence)
                {
                    // track non normal coverage result for second pass
                    case CoverageConfidence.Dubious:
                        dubiousTests.Add(id);
                        break;
                    case CoverageConfidence.Exact:
                        // track coverage
                        foreach (var mutation in coverageRunResult.CoveredMutations.Union(coverageRunResult.DetectedStaticMutations))
                        {
                            trustedCoveringMap[mutation].Add(id);
                        }
                        break;
                    case CoverageConfidence.Normal:
                        // track coverage
                        foreach (var mutation in coverageRunResult.CoveredMutations.Union(coverageRunResult.DetectedStaticMutations))
                        {
                            coveringMap[mutation].Add(id);
                        }
                        break;
                }

                // store static mutants
                staticMutants.UnionWith(coverageRunResult.DetectedStaticMutations);
                // store leaked mutations
                leakedMutations.UnionWith(coverageRunResult.LeakedMutations);
            }
            // now declare coverage info
            foreach (var mutant in mutantsToScan)
            {
                var mutantId = mutant.Id;
                if (mutant.IsStaticValue || staticMutants.Contains(mutantId) || leakedMutations.Contains(mutantId))
                {
                    mutant.IsStaticValue |= staticMutants.Contains(mutantId);
                    // this mutant is part of a static context, it must be run against all tests
                    // except the one we trust for not covering it
                    mutant.CoveringTests = new TestsGuidList(testIds.Where(tId => !trustedCoveringMap[mutantId].Contains(tId)));
                    _logger.LogDebug($"Mutant {mutant.Id} will be tested against all tests because it is used in a static context.");
                }
                else if (coveringMap[mutantId].Count == 0 && dubiousTests.Count == 0 &&
                         trustedCoveringMap[mutantId].Count == 0)
                {
                    mutant.CoveringTests = TestsGuidList.NoTest();
                    mutant.ResultStatus = MutantStatus.NoCoverage;
                    mutant.ResultStatusReason = "Mutant has no test coverage";
                    _logger.LogDebug($"Mutant {mutant.Id} is not covered by any test.");
                }
                else if (!_options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest))
                {
                    mutant.CoveringTests = TestsGuidList.EveryTest();
                    _logger.LogDebug($"Mutant {mutant.Id} will be tested against all tests.");
                }
                else
                {
                    // this mutant is covered by tests covering it officially plus the dubious tests, just in case
                    mutant.CoveringTests = new TestsGuidList(dubiousTests.Union(coveringMap[mutantId])
                        .Union(trustedCoveringMap[mutantId]));
                    _logger.LogDebug($"Mutant {mutant.Id} is covered by {mutant.CoveringTests.Count} tests.");
                    _logger.LogTrace($"Tests are : {string.Join(',', mutant.CoveringTests.GetGuids())}.");
                }
            }
        }
    }
}
