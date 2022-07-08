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
        public void ShouldSetTimedoutState()
        {
            var failedTestsMock = new Mock<ITestGuids>();
            var resultTestsMock = new Mock<ITestGuids>();
            var timedoutTestsMock = new Mock<ITestGuids>();
            var coveringTestsMock = new Mock<ITestGuids>();

            failedTestsMock.Setup(x => x.IsEmpty).Returns(true);
            timedoutTestsMock.Setup(x => x.IsEmpty).Returns(false);
            coveringTestsMock.Setup(x => x.IsEveryTest).Returns(true);

            var mutant = new Mutant();

            mutant.AnalyzeTestRun(failedTestsMock.Object, resultTestsMock.Object, timedoutTestsMock.Object);

            mutant.ResultStatus.ShouldBe(MutantStatus.Timeout);
        }
    }
}
