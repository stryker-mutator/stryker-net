using System;
using Moq;
using Shouldly;
using Stryker.Core.Mutants;
using Xunit;

namespace Stryker.Core.UnitTest.Mutants
{
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
                AssessingTests = new TestGuidsList(new[] { failingTest })
            };

            mutant.AnalyzeTestRun(new TestGuidsList(new[] { failingTest }),
                new TestGuidsList(new[] { succeedingTest }),
                TestGuidsList.NoTest(),
                false);

            mutant.ResultStatus.ShouldBe(MutantStatus.Killed);
            var killingTest = mutant.KillingTests.GetGuids().ShouldHaveSingleItem();
            killingTest.ShouldBe(failingTest);
        }

        [Fact]
        public void ShouldSetSurvivedWhenNonAssesingTestFailed()
        {
            var failingTest = Guid.NewGuid();
            var succeedingTest = Guid.NewGuid();
            var mutant = new Mutant
            {
                AssessingTests = new TestGuidsList(new[] { succeedingTest })
            };

            mutant.AnalyzeTestRun(new TestGuidsList(new[] { failingTest }),
                new TestGuidsList(new[] { succeedingTest }),
                TestGuidsList.NoTest(),
                false);

            mutant.ResultStatus.ShouldBe(MutantStatus.Survived);
            mutant.KillingTests.GetGuids().ShouldBeEmpty();
        }

        [Fact]
        public void ShouldSetSurvivedWhenNoTestSucceeds()
        {
            var succeedingTest = Guid.NewGuid();
            var mutant = new Mutant
            {
                AssessingTests = new TestGuidsList(new[] { succeedingTest })
            };

            mutant.AnalyzeTestRun(TestGuidsList.NoTest(),
                new TestGuidsList(new[] { succeedingTest }),
                TestGuidsList.NoTest(),
                false);

            mutant.ResultStatus.ShouldBe(MutantStatus.Survived);
            mutant.KillingTests.GetGuids().ShouldBeEmpty();
        }

        [Fact]
        public void ShouldSetTimedOutStateWhenSomeTestTimesOut()
        {
            var mutant = new Mutant
            {
                AssessingTests = TestGuidsList.EveryTest()
            };

            mutant.AnalyzeTestRun(TestGuidsList.NoTest(),
                TestGuidsList.EveryTest(),
                TestGuidsList.EveryTest(),
                false);

            mutant.ResultStatus.ShouldBe(MutantStatus.Timeout);
        }

        [Fact]
        public void ShouldSetTimedOutStateWhenSessionTimesOut()
        {
            var mutant = new Mutant
            {
                AssessingTests = TestGuidsList.EveryTest()
            };

            mutant.AnalyzeTestRun(TestGuidsList.NoTest(),
                TestGuidsList.NoTest(),
                TestGuidsList.NoTest(),
                true);

            mutant.ResultStatus.ShouldBe(MutantStatus.Timeout);
        }
    }
}
