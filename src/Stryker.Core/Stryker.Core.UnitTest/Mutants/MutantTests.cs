using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Moq;
using Shouldly;
using Stryker.Core.Mutants;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutants
{
    public class MutantTests
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

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        public void ShouldSetTimedoutState(bool coversEveryTest, bool coveringTestsContainTimedoutTests)
        {
            var failedTestsMock = new Mock<ITestListDescription>();
            var resultTestsMock = new Mock<ITestListDescription>();
            var timedoutTestsMock = new Mock<ITestListDescription>();
            var coveringTestsMock = new Mock<ITestListDescription>();

            failedTestsMock.Setup(x => x.IsEmpty).Returns(true);
            timedoutTestsMock.Setup(x => x.IsEmpty).Returns(false);
            coveringTestsMock.Setup(x => x.GetList()).Returns(new List<TestDescription>() { new TestDescription(Guid.NewGuid(), "test", null)});
            coveringTestsMock.Setup(x => x.IsEveryTest).Returns(coversEveryTest);
            coveringTestsMock.Setup(x => x.ContainsAny(It.IsAny<IReadOnlyList<TestDescription>>())).Returns(coveringTestsContainTimedoutTests);

            var mutant = new Mutant();

            mutant.AnalyzeTestRun(failedTestsMock.Object, resultTestsMock.Object, timedoutTestsMock.Object);

            mutant.ResultStatus.ShouldBe(MutantStatus.Timeout);
        }
    }
}
