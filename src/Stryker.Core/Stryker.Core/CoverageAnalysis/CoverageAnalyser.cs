using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation;
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

        public void DetermineTestCoverage(IProjectAndTests project, ITestRunner runner, IEnumerable<Mutant> mutants, ITestGuids resultFailingTests)
        {
            if (!_options.OptimizationMode.HasFlag(OptimizationModes.SkipUncoveredMutants) &&
                !_options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest))
            {
                foreach (var mutant in mutants)
                {
                    mutant.CoveringTests = TestGuidsList.EveryTest();
                    mutant.AssessingTests = TestGuidsList.EveryTest();
                }
                return;
            }

            ParseCoverage(runner.CaptureCoverage(project), mutants, new TestGuidsList(resultFailingTests.GetGuids()));
        }

        private void ParseCoverage(IEnumerable<CoverageRunResult> coverage, IEnumerable<Mutant> mutantsToScan, TestGuidsList failedTests)
        {
            var dubiousTests = new HashSet<Guid>();
            var trustedTests = new HashSet<Guid>();
            var testIds = new HashSet<Guid>();

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

                if (failedTests.Contains(coverageRunResult.TestId))
                {
                    // exclude failing tests from the list of all tests
                    continue;
                }
                testIds.Add(coverageRunResult.TestId);
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

            var allTestsExceptTrusted = (trustedTests.Count == 0 && failedTests.Count == 0)? TestGuidsList.EveryTest()
                : new TestGuidsList(testIds.Except(trustedTests).ToHashSet()).Excluding(failedTests);
            
            foreach (var mutant in mutantsToScan)
            {
                CoverageForThisMutant(mutant, mutationToResultMap, allTestsExceptTrusted, new TestGuidsList( dubiousTests), failedTests);
            }
        }

        private void CoverageForThisMutant(Mutant mutant,
            IReadOnlyDictionary<int, List<CoverageRunResult>> mutationToResultMap,
            TestGuidsList allTestsGuidsExceptTrusted,
            TestGuidsList dubiousTests,
            TestGuidsList failedTest)
        {
            var mutantId = mutant.Id;
            var (resultTingRequirements, testGuids) = ParseResultForThisMutant(mutationToResultMap, mutantId);

            var assessingTests = testGuids.Excluding(failedTest);
            mutant.MustBeTestedInIsolation = resultTingRequirements.HasFlag(MutationTestingRequirements.NeedEarlyActivation);
            if (resultTingRequirements.HasFlag(MutationTestingRequirements.AgainstAllTests))
            {
                mutant.CoveringTests = TestGuidsList.EveryTest();
                mutant.AssessingTests = TestGuidsList.EveryTest();
                _logger.LogDebug(
                    $"Mutant {mutant.Id} will be tested against all tests.");
            }
            else if (resultTingRequirements.HasFlag(MutationTestingRequirements.Static) || mutant.IsStaticValue)
            {
                // static mutations will be tested against every tests, except the one that are trusted not to cover it
                mutant.CoveringTests = allTestsGuidsExceptTrusted.Merge(testGuids);
                mutant.AssessingTests = allTestsGuidsExceptTrusted.Merge(assessingTests).Excluding(failedTest);
                
                mutant.IsStaticValue = true;
                _logger.LogDebug(
                    $"Mutant {mutant.Id} will be tested against most tests ({mutant.AssessingTests.Count}).");
            }
            else
            {
                mutant.CoveringTests = testGuids.Merge(dubiousTests);
                mutant.AssessingTests = testGuids.Merge(dubiousTests).Excluding(failedTest);
            }
            // assess status according to actual coverage
            if (mutant.CoveringTests.IsEmpty && mutant.ResultStatus == MutantStatus.Pending)
            {
                mutant.ResultStatus = MutantStatus.NoCoverage;
                mutant.ResultStatusReason = "Not covered by any test.";
                _logger.LogDebug($"Mutant {mutant.Id} is not covered by any test.");
            }
            else if (mutant.AssessingTests.IsEmpty && mutant.ResultStatus == MutantStatus.Pending)
            {
                mutant.ResultStatus = MutantStatus.Survived;
                mutant.ResultStatusReason = "Only covered by already failing tests.";
                _logger.LogInformation(
                    $"Mutant {mutant.Id} is only covered by failing tests.");
            }
            else
            {
                _logger.LogDebug(
                    $"Mutant {mutant.Id} will be tested against ({(mutant.AssessingTests.IsEveryTest ? "all" : mutant.AssessingTests.Count)}) tests.");
            }
        }

        private static (MutationTestingRequirements, TestGuidsList) ParseResultForThisMutant(IReadOnlyDictionary<int, List<CoverageRunResult>> mutationToResultMap, int mutantId)
        {
            var resultingRequirements = MutationTestingRequirements.None;
            if (!mutationToResultMap.ContainsKey(mutantId))
            {
                return (resultingRequirements, TestGuidsList.NoTest()); 
            }
            var testGuids = new List<Guid>();
            foreach (var coverageRunResult in mutationToResultMap[mutantId])
            {
                testGuids.Add(coverageRunResult.TestId);
                // did this test covered the mutation via some static context?
                var mutationTestingRequirement = coverageRunResult[mutantId];
                if (!resultingRequirements.HasFlag(MutationTestingRequirements.Static)
                    && (mutationTestingRequirement.HasFlag(MutationTestingRequirements.Static)
                        || mutationTestingRequirement.HasFlag(MutationTestingRequirements.CoveredOutsideTest)))
                {
                    resultingRequirements |= MutationTestingRequirements.Static;
                }

                // does this mutation require to be tested alone?
                if (!resultingRequirements.HasFlag(MutationTestingRequirements.NeedEarlyActivation) &&
                    mutationTestingRequirement.HasFlag(MutationTestingRequirements.NeedEarlyActivation))
                {
                    resultingRequirements |= MutationTestingRequirements.NeedEarlyActivation;
                }

                // does this mutation require to be tested against all tests?
                if (!mutationTestingRequirement.HasFlag(MutationTestingRequirements.AgainstAllTests) &&
                    coverageRunResult.Confidence == CoverageConfidence.UnexpectedCase)
                {
                    resultingRequirements |= MutationTestingRequirements.AgainstAllTests;
                }
            }

            return (resultingRequirements, new TestGuidsList(testGuids));
        }
    }
}
