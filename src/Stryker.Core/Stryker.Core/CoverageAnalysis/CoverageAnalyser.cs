using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Initialisation;
using Stryker.Abstractions.Logging;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions;
using Stryker.Abstractions.TestRunners;
using Stryker.Core.TestRunners;
using Stryker.Core.Mutants;
using Stryker.Abstractions.Options;

namespace Stryker.Core.CoverageAnalysis;

public class CoverageAnalyser : ICoverageAnalyser
{
    private readonly ILogger<CoverageAnalyser> _logger;
    private readonly IStrykerOptions _options;

    public CoverageAnalyser(IStrykerOptions options)
    {
        _options = options;
        _logger = ApplicationLogging.LoggerFactory.CreateLogger<CoverageAnalyser>();
    }

    public void DetermineTestCoverage(IProjectAndTests project, ITestRunner runner, IEnumerable<IMutant> mutants,
        ITestGuids resultFailingTests)
    {
        if (!_options.OptimizationMode.HasFlag(OptimizationModes.SkipUncoveredMutants) &&
            !_options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest))
        {
            AssumeAllTestsAreNeeded(mutants);

            return;
        }

        ParseCoverage(runner.CaptureCoverage(project), mutants, new TestGuidsList(resultFailingTests.GetGuids()));
    }

    private static void AssumeAllTestsAreNeeded(IEnumerable<IMutant> mutants)
    {
        foreach (var mutant in mutants)
        {
            mutant.CoveringTests = TestGuidsList.EveryTest();
            mutant.AssessingTests = TestGuidsList.EveryTest();
        }
    }

    private void ParseCoverage(IEnumerable<CoverageRunResult> coverage, IEnumerable<IMutant> mutantsToScan,
        TestGuidsList failedTests)
    {
        if (coverage.Sum(c => c.MutationsCovered.Count) == 0)
        {
            _logger.LogError("It looks like the test coverage capture failed. Disable coverage based optimisation.");
            AssumeAllTestsAreNeeded(mutantsToScan);
            return;
        }

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


        var allTest = TestGuidsList.EveryTest();
        var allTestsExceptTrusted = trustedTests.Count == 0 && failedTests.Count == 0
            ? TestGuidsList.EveryTest()
            : new TestGuidsList(testIds.Except(trustedTests).ToHashSet()).Excluding(failedTests);

        if (!coverage.Any())
        {
            allTest = TestGuidsList.NoTest();
            allTestsExceptTrusted = TestGuidsList.NoTest();
        }

        foreach (var mutant in mutantsToScan)
        {
            CoverageForThisMutant(mutant, mutationToResultMap, allTest, allTestsExceptTrusted,
                new TestGuidsList(dubiousTests), failedTests);
        }
    }

    private void CoverageForThisMutant(IMutant mutant,
        IReadOnlyDictionary<int, List<CoverageRunResult>> mutationToResultMap,
        TestGuidsList everytest,
        TestGuidsList allTestsGuidsExceptTrusted,
        TestGuidsList dubiousTests,
        TestGuidsList failedTest)
    {
        var mutantId = mutant.Id;
        var (resultTingRequirements, testGuids) = ParseResultForThisMutant(mutationToResultMap, mutantId);

        var assessingTests = testGuids.Excluding(failedTest);
        mutant.MustBeTestedInIsolation =
            resultTingRequirements.HasFlag(MutationTestingRequirements.NeedEarlyActivation);
        if (resultTingRequirements.HasFlag(MutationTestingRequirements.AgainstAllTests))
        {
            mutant.CoveringTests = everytest;
            mutant.AssessingTests = TestGuidsList.EveryTest();
        }
        else if (resultTingRequirements.HasFlag(MutationTestingRequirements.Static) || mutant.IsStaticValue)
        {
            // static mutations will be tested against every tests, except the one that are trusted not to cover it
            mutant.CoveringTests = allTestsGuidsExceptTrusted.Merge(testGuids);
            mutant.AssessingTests = allTestsGuidsExceptTrusted.Merge(assessingTests).Excluding(failedTest);

            mutant.IsStaticValue = true;
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
            _logger.LogDebug("Mutant {MutantId} is not covered by any test.", mutant.Id);
        }
        else if (mutant.AssessingTests.IsEmpty && mutant.ResultStatus == MutantStatus.Pending)
        {
            mutant.ResultStatus = MutantStatus.Survived;
            mutant.ResultStatusReason = "Only covered by already failing tests.";
            _logger.LogInformation(
                "Mutant {MutantId} is only covered by failing tests.", mutant.Id);
        }
        else
        {
            _logger.LogDebug(
                "Mutant {MutantId} will be tested against ({TestCases}) tests.", mutant.Id,
                mutant.AssessingTests.IsEveryTest ? "all" : mutant.AssessingTests.Count);
        }
    }

    private static (MutationTestingRequirements, TestGuidsList) ParseResultForThisMutant(
        IReadOnlyDictionary<int, List<CoverageRunResult>> mutationToResultMap, int mutantId)
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
