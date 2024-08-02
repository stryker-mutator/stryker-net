using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Configuration.Mutants;

namespace Stryker.Configuration.UnitTest.Mutants
{
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
