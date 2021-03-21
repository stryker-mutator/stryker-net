using Moq;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.TestRunners;
using System.Collections.Generic;
using Xunit;

namespace Stryker.Core.UnitTest.MutationTest
{
    public class MutationTestExecutorTests
    {
        [Fact]
        public void MutationTestExecutor_NoFailedTestShouldBeSurvived()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var mutant = new Mutant { Id = 1 };
            testRunnerMock.Setup(x => x.RunAll(It.IsAny<int>(), mutant, null)).Returns(new TestRunResult(true));

            var target = new MutationTestExecutor(testRunnerMock.Object);

            target.Test(new List<Mutant> { mutant }, 0, null);

            mutant.ResultStatus.ShouldBe(MutantStatus.Survived);
            testRunnerMock.Verify(x => x.RunAll(It.IsAny<int>(), mutant, null), Times.Once);
        }

        [Fact]
        public void MutationTestExecutor_FailedTestShouldBeKilled()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var mutant = new Mutant { Id = 1, CoveringTests = TestListDescription.EveryTest() };
            testRunnerMock.Setup(x => x.RunAll(It.IsAny<int>(), mutant, null)).Returns(new TestRunResult(false));

            var target = new MutationTestExecutor(testRunnerMock.Object);

            target.Test(new List<Mutant> { mutant }, 0, null);

            mutant.ResultStatus.ShouldBe(MutantStatus.Killed);
            testRunnerMock.Verify(x => x.RunAll(It.IsAny<int>(), mutant, null), Times.Once);
        }

        [Fact]
        public void MutationTestExecutor_TimeoutShouldBePassedToProcessTimeout()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var mutant = new Mutant { Id = 1, CoveringTests = TestListDescription.EveryTest() };
            testRunnerMock.Setup(x => x.RunAll(It.IsAny<int>(), mutant, null)).
                Returns(TestRunResult.TimedOut(TestListDescription.NoTest(), TestListDescription.NoTest(), TestListDescription.EveryTest(), ""));

            var target = new MutationTestExecutor(testRunnerMock.Object);

            target.Test(new List<Mutant> { mutant }, 1999, null);

            mutant.ResultStatus.ShouldBe(MutantStatus.Timeout);
            testRunnerMock.Verify(x => x.RunAll(1999, mutant, null), Times.Once);
        }
    }
}
