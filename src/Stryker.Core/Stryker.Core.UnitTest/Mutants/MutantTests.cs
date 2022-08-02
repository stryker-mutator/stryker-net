using System;
using System.Collections.Generic;
using Moq;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
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
        public void ShouldSetTimedOutState()
        {
            var failedTestsMock = new Mock<ITestGuids>();
            var timedOutTestsMock = new Mock<ITestGuids>();
            var coveringTestsMock = new Mock<ITestGuids>();

            failedTestsMock.Setup(x => x.IsEmpty).Returns(true);
            timedOutTestsMock.Setup(x => x.IsEmpty).Returns(false);
            coveringTestsMock.Setup(x => x.IsEveryTest).Returns(true);

            var mutant = new Mutant();
            var empty = new HashSet<Guid>();
            var results = new TestRunResults( null, empty, null, null);
            mutant.AnalyzeTestRun(results);

            mutant.ResultStatus.ShouldBe(MutantStatus.Timeout);
        }
    }
}
