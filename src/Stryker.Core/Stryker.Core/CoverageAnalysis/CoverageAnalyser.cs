using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.Tests;
using Stryker.Utilities.Logging;

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
        ITestIdentifiers resultFailingTests)
    {
        if (!_options.OptimizationMode.HasFlag(OptimizationModes.SkipUncoveredMutants) &&
            !_options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest))
        {
            AssumeAllTestsAreNeeded(mutants);

            return;
        }

        ParseCoverage(runner.CaptureCoverage(project), mutants, new TestIdentifierList(resultFailingTests.GetIdentifiers()));
    }

    private static void AssumeAllTestsAreNeeded(IEnumerable<IMutant> mutants)
    {
        foreach (var mutant in mutants)
        {
            mutant.CoveringTests = TestIdentifierList.EveryTest();
            mutant.AssessingTests = TestIdentifierList.EveryTest();
        }
    }

    private void ParseCoverage(IEnumerable<ICoverageRunResult> coverage, IEnumerable<IMutant> mutantsToScan,
        TestIdentifierList failedTests)
    {
        if (coverage.Sum(c => c.MutationsCovered.Count) == 0)
        {
            _logger.LogError("It looks like the test coverage capture failed. Disable coverage based optimisation.");
            AssumeAllTestsAreNeeded(mutantsToScan);
            return;
        }

        var dubiousTests = new HashSet<string>();
        var trustedTests = new HashSet<string>();
        var testIds = new HashSet<string>();

        var mutationToResultMap = new Dictionary<int, List<ICoverageRunResult>>();
        foreach (var coverageRunResult in coverage)
        {
            foreach (var i in coverageRunResult.MutationsCovered)
            {
                if (!mutationToResultMap.ContainsKey(i))
                {
                    mutationToResultMap[i] = new List<ICoverageRunResult>();
                }

                mutationToResultMap[i].Add(coverageRunResult);
            }

            if (failedTests.GetIdentifiers().Contains(coverageRunResult.TestId))
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


        var allTest = TestIdentifierList.EveryTest();
        var allTestsExceptTrusted = trustedTests.Count == 0 && failedTests.Count == 0
            ? TestIdentifierList.EveryTest()
            : new TestIdentifierList(testIds.Except(trustedTests).ToHashSet()).Excluding(failedTests);

        if (!coverage.Any())
        {
            allTest = TestIdentifierList.NoTest();
            allTestsExceptTrusted = TestIdentifierList.NoTest();
        }

        foreach (var mutant in mutantsToScan)
        {
            CoverageForThisMutant(mutant, mutationToResultMap, allTest, allTestsExceptTrusted,
                new TestIdentifierList(dubiousTests), failedTests);
        }
    }

    private void CoverageForThisMutant(IMutant mutant,
        IReadOnlyDictionary<int, List<ICoverageRunResult>> mutationToResultMap,
        ITestIdentifiers everytest,
        ITestIdentifiers allTestsGuidsExceptTrusted,
        ITestIdentifiers dubiousTests,
        ITestIdentifiers failedTest)
    {
        var mutantId = mutant.Id;
        var (resultTingRequirements, testGuids) = ParseResultForThisMutant(mutationToResultMap, mutantId);

        var assessingTests = testGuids.Excluding(failedTest);
        mutant.MustBeTestedInIsolation =
            resultTingRequirements.HasFlag(MutationTestingRequirements.NeedEarlyActivation);
        if (resultTingRequirements.HasFlag(MutationTestingRequirements.AgainstAllTests))
        {
            mutant.CoveringTests = everytest;
            mutant.AssessingTests = TestIdentifierList.EveryTest();
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

    private static (MutationTestingRequirements, ITestIdentifiers) ParseResultForThisMutant(
        IReadOnlyDictionary<int, List<ICoverageRunResult>> mutationToResultMap, int mutantId)
    {
        var resultingRequirements = MutationTestingRequirements.None;
        if (!mutationToResultMap.ContainsKey(mutantId))
        {
            return (resultingRequirements, TestIdentifierList.NoTest());
        }

        var testGuids = new List<string>();
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

        return (resultingRequirements, new TestIdentifierList(testGuids));
    }
}
