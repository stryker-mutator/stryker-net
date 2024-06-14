using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Mutants;
using Stryker.Shared.Coverage;
using Stryker.Shared.Initialisation;
using Stryker.Shared.Logging;
using Stryker.Shared.Mutants;
using Stryker.Shared.Options;
using Stryker.Shared.Tests;

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

    public void DetermineTestCoverage(IProjectAndTests project, ITestRunner runner, IEnumerable<IMutant> mutants, ITestIdentifiers resultFailingTests)
    {
        if (!_options.OptimizationMode.HasFlag(OptimizationModes.SkipUncoveredMutants) &&
            !_options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest))
        {
            foreach (var mutant in mutants)
            {
                mutant.CoveringTests = TestIdentifiers.EveryTest();
                mutant.AssessingTests = TestIdentifiers.EveryTest();
            }
            return;
        }

        ParseCoverage(runner.CaptureCoverage(project), mutants, new TestIdentifiers(resultFailingTests.GetIdentifiers()));
    }

    private void ParseCoverage(IEnumerable<ICoverageRunResult> coverage, IEnumerable<IMutant> mutantsToScan, ITestIdentifiers failedTests)
    {
        var dubiousTests = new HashSet<Identifier>();
        var trustedTests = new HashSet<Identifier>();
        var testIds = new HashSet<Identifier>();

        var mutationToResultMap = new Dictionary<int, List<ICoverageRunResult>>();
        foreach (var coverageRunResult in coverage)
        {
            foreach (var i in coverageRunResult.MutationsCovered)
            {
                if (!mutationToResultMap.ContainsKey(i))
                {
                    mutationToResultMap[i] = [];
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


        var allTest = TestIdentifiers.EveryTest();
        var allTestsExceptTrusted = (trustedTests.Count == 0 && failedTests.Count == 0)? TestIdentifiers.EveryTest()
            : new TestIdentifiers(testIds.Except(trustedTests).ToHashSet()).Excluding(failedTests) as TestIdentifiers;

        if (!coverage.Any())
        {
            allTest = TestIdentifiers.NoTest();
            allTestsExceptTrusted = TestIdentifiers.NoTest();
        }
        foreach (var mutant in mutantsToScan)
        {
            CoverageForThisMutant(mutant, mutationToResultMap, allTest, allTestsExceptTrusted, new TestIdentifiers( dubiousTests), failedTests);
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
        mutant.MustBeTestedInIsolation = resultTingRequirements.HasFlag(MutationTestingRequirements.NeedEarlyActivation);
        if (resultTingRequirements.HasFlag(MutationTestingRequirements.AgainstAllTests))
        {
            mutant.CoveringTests = everytest;
            mutant.AssessingTests = TestIdentifiers.EveryTest();
        }
        else if (resultTingRequirements.HasFlag(MutationTestingRequirements.Static) || mutant.IsStaticValue)
        {
            // static mutations will be tested against every tests, except the one that are trusted not to cover it
            mutant.CoveringTests = allTestsGuidsExceptTrusted.Merge(testGuids);
            mutant.AssessingTests = (allTestsGuidsExceptTrusted.Merge(assessingTests) as TestIdentifiers).Excluding(failedTest);
            
            mutant.IsStaticValue = true;
        }
        else
        {
            mutant.CoveringTests = testGuids.Merge(dubiousTests);
            mutant.AssessingTests = (testGuids.Merge(dubiousTests) as TestIdentifiers).Excluding(failedTest);
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

    private static (MutationTestingRequirements, ITestIdentifiers) ParseResultForThisMutant(IReadOnlyDictionary<int, List<ICoverageRunResult>> mutationToResultMap, int mutantId)
    {
        var resultingRequirements = MutationTestingRequirements.None;
        if (!mutationToResultMap.ContainsKey(mutantId))
        {
            return (resultingRequirements, TestIdentifiers.NoTest()); 
        }
        var testIdentifiers = new List<Identifier>();
        foreach (var coverageRunResult in mutationToResultMap[mutantId])
        {
            testIdentifiers.Add(coverageRunResult.TestId);
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

        return (resultingRequirements, new TestIdentifiers(testIdentifiers));
    }
}
