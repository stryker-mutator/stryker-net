using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.Tests;

namespace Stryker.Core.CoverageAnalysis;

public class CoverageAnalyser : ICoverageAnalyser
{
    private readonly ILogger<CoverageAnalyser> _logger;

    public CoverageAnalyser(ILogger<CoverageAnalyser> logger) => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public void DetermineTestCoverage(IStrykerOptions options, IProjectAndTests project, ITestRunner runner, IEnumerable<IMutant> mutants,
        ITestIdentifiers resultFailingTests)
    {
        if (!options.OptimizationMode.HasFlag(OptimizationModes.SkipUncoveredMutants) &&
            !options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest))
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
        // if no test covers any mutant, we assume that the coverage capture failed, and we disable coverage based optimization
        if (coverage.All(c => c.MutationsCovered.Count == 0))
        {
            _logger.LogError("It looks like the test coverage capture failed. Disable coverage based optimisation.");
            AssumeAllTestsAreNeeded(mutantsToScan);
            return;
        }

        var dubiousTests = new HashSet<string>();
        var trustedTests = new HashSet<string>();
        var testIds = new HashSet<string>();

        var mutationToResultMap = MutationToResultMap(coverage, failedTests, testIds, dubiousTests, trustedTests);

        ITestIdentifierSet allTest;
        ITestIdentifierSet allTestsExceptTrusted;
        if (!coverage.Any())
        {
            allTest = TestIdentifierList.NoTest();
            allTestsExceptTrusted = TestIdentifierList.NoTest();
        }
        else
        {
            allTest = TestIdentifierList.EveryTest();
            if (trustedTests.Count == 0 && failedTests.IsEmpty)
            {
                allTestsExceptTrusted = TestIdentifierList.EveryTest();
            }
            else
            {
                testIds.ExceptWith(trustedTests);
                testIds.ExceptWith(failedTests.GetIdentifiers());
                allTestsExceptTrusted = new TestIdentifierList(testIds);
            }
        }

        var dubiousTestsList = new TestIdentifierList(dubiousTests);
        foreach (var mutant in mutantsToScan)
        {
            CoverageForThisMutant(mutant, mutationToResultMap,allTest, allTestsExceptTrusted,
                dubiousTestsList, failedTests);
        }
    }

    private static Dictionary<int, List<ICoverageRunResult>> MutationToResultMap(IEnumerable<ICoverageRunResult> coverage, TestIdentifierList failedTests, HashSet<string> testIds,
        HashSet<string> dubiousTests, HashSet<string> trustedTests)
    {
        var mutationToResultMap = new Dictionary<int, List<ICoverageRunResult>>();
        foreach (var coverageRunResult in coverage)
        {
            foreach (var i in coverageRunResult.MutationsCovered)
            {
                if (!mutationToResultMap.TryGetValue(i, out var value))
                {
                    value = [];
                    mutationToResultMap[i] = value;
                }

                value.Add(coverageRunResult);
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

        return mutationToResultMap;
    }

    private void CoverageForThisMutant(IMutant mutant,
        IReadOnlyDictionary<int, List<ICoverageRunResult>> mutationToResultMap,
        ITestIdentifierSet allTests,
        ITestIdentifierSet allTestsExceptTrusted,
        ITestIdentifierSet dubiousTests,
        ITestIdentifierSet failedTest)
    {
        var mutantId = mutant.Id;
        var (resultTingRequirements, testGuids) = ParseResultForThisMutant(mutationToResultMap, mutantId);

        mutant.MustBeTestedInIsolation =
            resultTingRequirements.HasFlag(MutationTestingRequirements.NeedEarlyActivation);
        if (resultTingRequirements.HasFlag(MutationTestingRequirements.AgainstAllTests))
        {
            mutant.CoveringTests = TestIdentifierList.EveryTest();
            mutant.AssessingTests = allTests;
        }
        else if (resultTingRequirements.HasFlag(MutationTestingRequirements.Static) || mutant.IsStaticValue)
        {
            // static mutations will be tested against every test, except the one that are trusted not to cover it
            mutant.CoveringTests = allTestsExceptTrusted.Merge(testGuids);
            mutant.AssessingTests = new TestIdentifierList(mutant.CoveringTests).Excluding(failedTest);
            mutant.IsStaticValue = true;
        }
        else
        {
            mutant.CoveringTests = new TestIdentifierList(testGuids).Merge(dubiousTests);
            mutant.AssessingTests = new TestIdentifierList(mutant.CoveringTests).Excluding(failedTest);
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
        if (!mutationToResultMap.TryGetValue(mutantId, out var results))
        {
            return (resultingRequirements, TestIdentifierList.NoTest());
        }

        var testGuids = new List<string>();
        foreach (var coverageRunResult in results)
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
