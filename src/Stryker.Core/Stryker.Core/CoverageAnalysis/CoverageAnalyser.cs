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
            var dubiousTests = new HashSet<Guid>();
            var trustedTests = new HashSet<Guid>();
            var testIds = coverage.Select(c => c.TestId).ToList();

            var mutationToResultMap = new Dictionary<int, List<CoverageRunResult>>();
            foreach (var coverageRunResult in coverage)
            {
                foreach (var i in coverageRunResult.MutationsCovered)
                {
                    if (!mutationToResultMap.ContainsKey(i))
                    {
                        mutationToResultMap[i] = new List<CoverageRunResult>();
                    }
                    mutationToResultMap[i].Add(coverageRunResult);
                }

                switch (coverageRunResult.Confidence)
                {
                    case CoverageConfidence.Dubious:
                        dubiousTests.Add(coverageRunResult.TestId);
                        break;
                    case CoverageConfidence.Exact:
                        trustedTests.Add(coverageRunResult.TestId);
                        break;
                }
            }

            var allTestsExceptTrusted = testIds.Except(trustedTests).ToHashSet();
            foreach (var mutant in mutantsToScan)
            {
                CoverageForThisMutant(mutant, mutationToResultMap, allTestsExceptTrusted, dubiousTests);
            }
        }

        private void CoverageForThisMutant(Mutant mutant, IReadOnlyDictionary<int, List<CoverageRunResult>> mutationToResultMap,
            IEnumerable<Guid> allTestsGuidsExceptTrusted, IEnumerable<Guid> dubiousTests)
        {
            var testGuids = new List<Guid>();
            var mutantId = mutant.Id;
            var resultTingRequirements = PareResultForThisMutant(mutationToResultMap, mutantId, testGuids);

            mutant.MustBeTestedInIsolation = resultTingRequirements.HasFlag(MutationTestingRequirements.NeedEarlyActivation);
            if (resultTingRequirements.HasFlag(MutationTestingRequirements.AgainstAllTests))
            {
                mutant.CoveringTests = TestsGuidList.EveryTest();
                _logger.LogDebug(
                    $"Mutant {mutant.Id} will be tested against all tests.");
            }
            else if (resultTingRequirements.HasFlag(MutationTestingRequirements.Static))
            {
                // static mutations will be tested against every tests, except the one that are trusted not to cover it
                mutant.CoveringTests = new TestsGuidList(allTestsGuidsExceptTrusted.Union(testGuids).Distinct());
                mutant.IsStaticValue = true;
                _logger.LogDebug(
                    $"Mutant {mutant.Id} will be tested against most tests ({mutant.CoveringTests.Count}).");
            }
            else
            {
                mutant.CoveringTests = new TestsGuidList(testGuids.Union(dubiousTests));
                if (mutant.CoveringTests.IsEmpty)
                {
                    _logger.LogInformation(
                        $"Mutant {mutant.Id} is not covered by any test.");
                }
                else
                {
                    _logger.LogDebug(
                        $"Mutant {mutant.Id} will be tested against ({mutant.CoveringTests.Count}) tests.");
                }
            }
        }

        private static MutationTestingRequirements PareResultForThisMutant(IReadOnlyDictionary<int, List<CoverageRunResult>> mutationToResultMap, int mutantId,
            ICollection<Guid> testGuids)
        {
            var resultTingRequirements = MutationTestingRequirements.None;
            if (!mutationToResultMap.ContainsKey(mutantId))
            {
                return resultTingRequirements;
            }
            foreach (var coverageRunResult in mutationToResultMap[mutantId])
            {
                testGuids.Add(coverageRunResult.TestId);
                // did this test covered the mutation via some static context
                var mutationTestingRequirement = coverageRunResult[mutantId];
                if (!resultTingRequirements.HasFlag(MutationTestingRequirements.Static)
                    && (mutationTestingRequirement.HasFlag(MutationTestingRequirements.Static)
                        || mutationTestingRequirement.HasFlag(MutationTestingRequirements.CoveredOutsideTest)))
                {
                    resultTingRequirements |= MutationTestingRequirements.Static;
                }

                // did this mutation requires to be tested alone
                if (!resultTingRequirements.HasFlag(MutationTestingRequirements.NeedEarlyActivation) &&
                    mutationTestingRequirement.HasFlag(MutationTestingRequirements.NeedEarlyActivation))
                {
                    resultTingRequirements |= MutationTestingRequirements.NeedEarlyActivation;
                }

                if (!mutationTestingRequirement.HasFlag(MutationTestingRequirements.AgainstAllTests) &&
                    coverageRunResult.Confidence == CoverageConfidence.UnexpectedCase)
                {
                    resultTingRequirements |= MutationTestingRequirements.AgainstAllTests;
                }
            }

            return resultTingRequirements;
        }
    }
}
