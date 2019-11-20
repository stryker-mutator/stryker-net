using System;
using System.Collections.Generic;
using Moq;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.TestRunners;
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
            testRunnerMock.Setup(x => x.RunAll(It.IsAny<int>(), mutant)).Returns(new TestRunResult (true ));
            
            var target = new MutationTestExecutor(testRunnerMock.Object);

            target.Test(new List<Mutant>{mutant}, 0);

            mutant.ResultStatus.ShouldBe(MutantStatus.Survived);
            testRunnerMock.Verify(x => x.RunAll(It.IsAny<int>(), mutant), Times.Once);
        }

        [Fact]
        public void MutationTestExecutor_FailedTestShouldBeKilled()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var mutant = new Mutant { Id = 1 };
            testRunnerMock.Setup(x => x.RunAll(It.IsAny<int>(), mutant)).Returns(new TestRunResult(false));
            
            var target = new MutationTestExecutor(testRunnerMock.Object);

            target.Test(new List<Mutant>{mutant}, 0);

            mutant.ResultStatus.ShouldBe(MutantStatus.Killed);
            testRunnerMock.Verify(x => x.RunAll(It.IsAny<int>(), mutant), Times.Once);
        }

        [Fact]
        public void MutationTestExecutor_TimeoutShouldBePassedToProcessTimeout()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var mutant = new Mutant { Id = 1 };
            testRunnerMock.Setup(x => x.RunAll(It.IsAny<int>(), mutant)).
                Returns(TestRunResult.TimedOut(new TestListDescription(null), new TestListDescription(null), ""));

            var target = new MutationTestExecutor(testRunnerMock.Object);

            target.Test(new List<Mutant>{mutant}, 1999);

            mutant.ResultStatus.ShouldBe(MutantStatus.Timeout);
            testRunnerMock.Verify(x => x.RunAll(1999, mutant), Times.Once);
        }
    }
}
