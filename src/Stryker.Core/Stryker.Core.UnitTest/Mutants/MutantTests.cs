using System;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Shared.Mutants;
using Stryker.Shared.Tests;
using Xunit;

namespace Stryker.Core.UnitTest;

public class MutantTests : TestBase
{
    [Fact]
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

    [Theory]
    [InlineData(MutantStatus.CompileError, false)]
    [InlineData(MutantStatus.Ignored, false)]
    [InlineData(MutantStatus.Killed, true)]
    [InlineData(MutantStatus.NoCoverage, true)]
    [InlineData(MutantStatus.Pending, true)]
    [InlineData(MutantStatus.Survived, true)]
    [InlineData(MutantStatus.Timeout, true)]
    public void ShouldCountForStats(MutantStatus status, bool doesCount)
    {
        var mutant = new Mutant
        {
            ResultStatus = status
        };

        mutant.CountForStats.ShouldBe(doesCount);
    }

    [Fact]
    public void ShouldSetKilledStateWhenAssesingTestFailed()
    {
        var failingTest = Guid.NewGuid();
        var succeedingTest = Guid.NewGuid();
        var mutant = new Mutant
        {
            AssessingTests = new TestIdentifiers(new[] { failingTest })
        };

        mutant.AnalyzeTestRun(new TestIdentifiers(new[] { failingTest }),
            new TestIdentifiers(new[] { succeedingTest }),
            TestIdentifiers.NoTest(),
            false);

        mutant.ResultStatus.ShouldBe(MutantStatus.Killed);
        var killingTest = mutant.KillingTests.GetIdentifiers().ShouldHaveSingleItem();
        killingTest.ShouldBe(Identifier.Create(failingTest));
    }

    [Fact]
    public void ShouldSetSurvivedWhenNonAssesingTestFailed()
    {
        var failingTest = Guid.NewGuid();
        var succeedingTest = Guid.NewGuid();
        var mutant = new Mutant
        {
            AssessingTests = new TestIdentifiers(new[] { succeedingTest })
        };

        mutant.AnalyzeTestRun(new TestIdentifiers(new[] { failingTest }),
            new TestIdentifiers(new[] { succeedingTest }),
            TestIdentifiers.NoTest(),
            false);

        mutant.ResultStatus.ShouldBe(MutantStatus.Survived);
        mutant.KillingTests.GetIdentifiers().ShouldBeEmpty();
    }

    [Fact]
    public void ShouldSetSurvivedWhenNoTestSucceeds()
    {
        var succeedingTest = Guid.NewGuid();
        var mutant = new Mutant
        {
            AssessingTests = new TestIdentifiers(new[] { succeedingTest })
        };

        mutant.AnalyzeTestRun(TestIdentifiers.NoTest(),
            new TestIdentifiers(new[] { succeedingTest }),
            TestIdentifiers.NoTest(),
            false);

        mutant.ResultStatus.ShouldBe(MutantStatus.Survived);
        mutant.KillingTests.GetIdentifiers().ShouldBeEmpty();
    }

    [Fact]
    public void ShouldSetTimedOutStateWhenSomeTestTimesOut()
    {
        var mutant = new Mutant
        {
            AssessingTests = TestIdentifiers.EveryTest()
        };

        mutant.AnalyzeTestRun(TestIdentifiers.NoTest(),
            TestIdentifiers.EveryTest(),
            TestIdentifiers.EveryTest(),
            false);

        mutant.ResultStatus.ShouldBe(MutantStatus.Timeout);
    }

    [Fact]
    public void ShouldSetTimedOutStateWhenSessionTimesOut()
    {
        var mutant = new Mutant
        {
            AssessingTests = TestIdentifiers.EveryTest()
        };

        mutant.AnalyzeTestRun(TestIdentifiers.NoTest(),
            TestIdentifiers.NoTest(),
            TestIdentifiers.NoTest(),
            true);

        mutant.ResultStatus.ShouldBe(MutantStatus.Timeout);
    }
}
