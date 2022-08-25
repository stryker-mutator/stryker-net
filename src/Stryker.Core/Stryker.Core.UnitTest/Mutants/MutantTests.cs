using System;
using System.Collections.Generic;
using Moq;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.TestRunners;
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
        [InlineData(MutantStatus.NotRun, true)]
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
        public void ShouldSetTimedOutStateWhenSomeTestTimesOut()
        {
            var mutant = new Mutant
            {
                AssessingTests = TestGuidsList.EveryTest()
            };

            var result = new TestRunResults(TestGuidsList.EveryTest(),
                TestGuidsList.NoTest(),
                TestGuidsList.EveryTest(),
                TestGuidsList.NoTest());


            mutant.AnalyzeTestRun(result);

            mutant.ResultStatus.ShouldBe(MutantStatus.Timeout);
        }

        [Fact]
        public void ShouldSetTimedOutStateWhenSessionTimesOut()
        {
            var mutant = new Mutant
            {
                AssessingTests = TestGuidsList.EveryTest()
            };

            var result = TestRunResults.TimedOut(TestGuidsList.EveryTest(),
                TestGuidsList.NoTest(),
                TestGuidsList.EveryTest(),
                TestGuidsList.NoTest());


            mutant.AnalyzeTestRun(result);

            mutant.ResultStatus.ShouldBe(MutantStatus.Timeout);
        }
    }
}
