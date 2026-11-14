using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.Core.CoverageAnalysis;
using Stryker.Core.Mutants;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;

namespace Stryker.Core.UnitTest.MutationTest;

[TestClass]
public class CoverageAnalyserTests : TestBase
{
    [TestMethod]
    public void DetermineTestCoverage_ShouldSumOnlyAssessingTestHits()
    {
        var coverage = new ICoverageRunResult[]
        {
            Coverage("assessing-a", 8),
            Coverage("assessing-b", 7),
            Coverage("failed", 100)
        };
        var mutant = new Mutant { Id = 1 };

        AnalyzeCoverage(coverage, mutant, OptimizationModes.CoverageBasedTest, new TestIdentifierList("failed"));

        mutant.HitLimit.ShouldBe(1500L);
            mutant.HitCount.ShouldBe(15L);
        mutant.AssessingTests.GetIdentifiers().ShouldBe(new[] { "assessing-a", "assessing-b" });
    }

    [TestMethod]
    public void DetermineTestCoverage_ShouldApplyMinimumHitLimit()
    {
        var mutant = new Mutant { Id = 1 };

        AnalyzeCoverage([Coverage("test", 1)], mutant, OptimizationModes.CoverageBasedTest, TestIdentifierList.NoTest());

        mutant.HitLimit.ShouldBe(1000L);
        mutant.HitCount.ShouldBe(1L);
    }

    [TestMethod]
    public void DetermineTestCoverage_ShouldDisableHitLimitWhenCountsAreUnavailable()
    {
        var mutant = new Mutant { Id = 1 };
        var coverage = new[]
        {
            CoverageRunResult.Create("test", CoverageConfidence.Normal, [1], [], [])
        };

        AnalyzeCoverage(coverage, mutant, OptimizationModes.CoverageBasedTest, TestIdentifierList.NoTest());

        mutant.HitLimit.ShouldBeNull();
        mutant.HitCount.ShouldBeNull();
        mutant.HitCount.ShouldBeNull();
    }

    [TestMethod]
    public void DetermineTestCoverage_ShouldDisableHitLimitWhenCoverageAnalysisIsOff()
    {
        var mutant = new Mutant { Id = 1 };

        AnalyzeCoverage([], mutant, OptimizationModes.None, TestIdentifierList.NoTest());

        mutant.HitLimit.ShouldBeNull();
    }

    private static void AnalyzeCoverage(IEnumerable<ICoverageRunResult> coverage, IMutant mutant,
        OptimizationModes optimizationMode, ITestIdentifiers failedTests)
    {
        var runner = new Mock<ITestRunner>();
        runner.Setup(testRunner => testRunner.CaptureCoverage(It.IsAny<IProjectAndTests>())).Returns(coverage);
        var options = new Mock<IStrykerOptions>();
        options.SetupGet(strykerOptions => strykerOptions.OptimizationMode).Returns(optimizationMode);

        var analyser = new CoverageAnalyser(TestLoggerFactory.CreateLogger<CoverageAnalyser>());
        analyser.DetermineTestCoverage(options.Object, Mock.Of<IProjectAndTests>(), runner.Object, [mutant], failedTests);
    }

    private static CoverageRunResult Coverage(string testId, int hitCount) =>
        CoverageRunResult.Create(testId, CoverageConfidence.Exact, [1], [], [], new Dictionary<int, int> { [1] = hitCount });
}