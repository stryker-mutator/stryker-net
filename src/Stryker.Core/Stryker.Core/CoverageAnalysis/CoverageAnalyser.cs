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
            var coveringMap = new Dictionary<int, ISet<Guid>>();
            var trustedCoveringMap = new Dictionary<int, ISet<Guid>>();
            foreach (var id in mutantsToScan.Select( m=> m.Id))
            {
                coveringMap[id] = new HashSet<Guid>();
                trustedCoveringMap[id] = new HashSet<Guid>();
            }
            var staticMutants = new HashSet<int>();
            var dubiousTests = new List<Guid>();
            var leakedMutations = new HashSet<int>();
            var testIds = coverage.Select(c => c.TestId).ToList();
            // process coverage results
            TransformResult(coverage, dubiousTests, trustedCoveringMap, coveringMap, staticMutants, leakedMutations);
            // now declare coverage info for each mutant
            foreach (var mutant in mutantsToScan)
            {
                CoverageForThisMutant(mutant, staticMutants, leakedMutations, testIds, trustedCoveringMap, coveringMap, dubiousTests);
            }
        }

        private void TransformResult(IEnumerable<CoverageRunResult> coverageRunResults, ICollection<Guid> guids, IReadOnlyDictionary<int, ISet<Guid>> dictionary, IReadOnlyDictionary<int, ISet<Guid>> coveringMap1,
            ISet<int> staticMutantSet, ISet<int> leakedMutationSet)
        {
            foreach (var coverageRunResult in coverageRunResults)
            {
                var id = coverageRunResult.TestId;
                switch (coverageRunResult.Confidence)
                {
                    // track non normal coverage result for second pass
                    case CoverageConfidence.Dubious:
                        guids.Add(id);
                        break;
                    case CoverageConfidence.Exact:
                        // track coverage
                        foreach (var mutation in coverageRunResult.CoveredMutations.Union(coverageRunResult
                                     .DetectedStaticMutations))
                        {
                            dictionary[mutation].Add(id);
                        }
                        break;
                    default:
                        // track coverage
                        foreach (var mutation in coverageRunResult.CoveredMutations.Union(coverageRunResult
                                     .DetectedStaticMutations))
                        {
                            coveringMap1[mutation].Add(id);
                        }
                        break;
                }

                // store static mutants
                staticMutantSet.UnionWith(coverageRunResult.DetectedStaticMutations);
                // store leaked mutations
                leakedMutationSet.UnionWith(coverageRunResult.LeakedMutations);
            }
        }

        private void CoverageForThisMutant(Mutant mutant, IReadOnlySet<int> staticMutants, IReadOnlySet<int> leakedMutations, IEnumerable<Guid> testIds,
            IReadOnlyDictionary<int, ISet<Guid>> trustedCoveringMap, IReadOnlyDictionary<int, ISet<Guid>> coveringMap, IReadOnlyCollection<Guid> dubiousTests)
        {
            var mutantId = mutant.Id;
            if (mutant.IsStaticValue || staticMutants.Contains(mutantId))
            {
                // mutant is static
                mutant.IsStaticValue |= staticMutants.Contains(mutantId);
                // this mutant is part of a static context, it must be run against all tests
                // except the one we trust for not covering it
                mutant.CoveringTests = new TestsGuidList(testIds.Where(tId => !trustedCoveringMap[mutantId].Contains(tId)));
                _logger.LogDebug(
                    $"Mutant {mutant.Id} will be tested against all tests because it is used in a static context.");
            }
            
            else if (leakedMutations.Contains(mutantId))
            {
                // mutant has been covered between tests
                mutant.IsStaticValue = true;
                // this mutant appears outside any test => we are not sure which test covers it, we need to run all tests
                // except the one we trust for not covering it
                mutant.CoveringTests = new TestsGuidList(testIds.Where(tId => !trustedCoveringMap[mutantId].Contains(tId)));
                _logger.LogDebug(
                    $"Mutant {mutant.Id} will be tested against all tests because it is covered outside tests.");
            }
            else if (coveringMap[mutantId].Count == 0 && dubiousTests.Count == 0 &&
                     trustedCoveringMap[mutantId].Count == 0)
            {
                // mutant is not covered
                mutant.CoveringTests = TestsGuidList.NoTest();
                mutant.ResultStatus = MutantStatus.NoCoverage;
                mutant.ResultStatusReason = "Mutant has no test coverage";
                _logger.LogDebug($"Mutant {mutant.Id} is not covered by any test.");
            }
            else if (!_options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest))
            {
                // no coverage base test: every covered mutant is tested against all tests
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
