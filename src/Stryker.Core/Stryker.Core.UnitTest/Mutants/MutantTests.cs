using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Core.Mutants;
using Stryker.TestRunner.Tests;

namespace Stryker.Core.UnitTest.Mutants;

[TestClass]
public class MutantTests : TestBase
{
    [TestMethod]
    public void ShouldHaveDisplayName()
    {
        var mutant = new Mutant
        {
            Id = 1,
            Mutation = new Mutation
            {
                DisplayName = "test mutation"
            }
        };

        mutant.DisplayName.ShouldBe("1: test mutation");
    }

    [TestMethod]
    [DataRow(MutantStatus.CompileError, false)]
    [DataRow(MutantStatus.Ignored, false)]
    [DataRow(MutantStatus.Killed, true)]
    [DataRow(MutantStatus.NoCoverage, true)]
    [DataRow(MutantStatus.Pending, true)]
    [DataRow(MutantStatus.Survived, true)]
    [DataRow(MutantStatus.Timeout, true)]
    public void ShouldCountForStats(MutantStatus status, bool doesCount)
    {
        var mutant = new Mutant
        {
            ResultStatus = status
        };

        mutant.CountForStats.ShouldBe(doesCount);
    }

    [TestMethod]
    public void ShouldSetKilledStateWhenAssesingTestFailed()
    {
        var failingTest = Guid.NewGuid().ToString();
        var succeedingTest = Guid.NewGuid().ToString();
        var mutant = new Mutant
        {
            AssessingTests = new TestIdentifierList(new[] { failingTest })
        };

        mutant.AnalyzeTestRun(new TestIdentifierList(new[] { failingTest }),
            new TestIdentifierList(new[] { succeedingTest }),
            TestIdentifierList.NoTest(),
            false);

        mutant.ResultStatus.ShouldBe(MutantStatus.Killed);
        var killingTest = mutant.KillingTests.GetIdentifiers().ShouldHaveSingleItem();
        killingTest.ShouldBe(failingTest);
    }

    [TestMethod]
    public void ShouldSetSurvivedWhenNonAssesingTestFailed()
    {
        var failingTest = Guid.NewGuid().ToString();
        var succeedingTest = Guid.NewGuid().ToString();
        var mutant = new Mutant
        {
            AssessingTests = new TestIdentifierList(new[] { succeedingTest })
        };

        mutant.AnalyzeTestRun(new TestIdentifierList(new[] { failingTest }),
            new TestIdentifierList(new[] { succeedingTest }),
            TestIdentifierList.NoTest(),
            false);

        mutant.ResultStatus.ShouldBe(MutantStatus.Survived);
        mutant.KillingTests.GetIdentifiers().ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldSetSurvivedWhenNoTestSucceeds()
    {
        var succeedingTest = Guid.NewGuid().ToString();
        var mutant = new Mutant
        {
            AssessingTests = new TestIdentifierList(new[] { succeedingTest })
        };

        mutant.AnalyzeTestRun(TestIdentifierList.NoTest(),
            new TestIdentifierList(new[] { succeedingTest }),
            TestIdentifierList.NoTest(),
            false);

        mutant.ResultStatus.ShouldBe(MutantStatus.Survived);
        mutant.KillingTests.GetIdentifiers().ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldSetTimedOutStateWhenSomeTestTimesOut()
    {
        var mutant = new Mutant
        {
            AssessingTests = TestIdentifierList.EveryTest()
        };

        mutant.AnalyzeTestRun(TestIdentifierList.NoTest(),
            TestIdentifierList.EveryTest(),
            TestIdentifierList.EveryTest(),
            false);

        mutant.ResultStatus.ShouldBe(MutantStatus.Timeout);
    }

    [TestMethod]
    public void ShouldSetTimedOutStateWhenSessionTimesOut()
    {
        var mutant = new Mutant
        {
            AssessingTests = TestIdentifierList.EveryTest()
        };

        mutant.AnalyzeTestRun(TestIdentifierList.NoTest(),
            TestIdentifierList.NoTest(),
            TestIdentifierList.NoTest(),
            true);

        mutant.ResultStatus.ShouldBe(MutantStatus.Timeout);
    }
}
