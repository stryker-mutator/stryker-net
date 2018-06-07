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
            testRunnerMock.Setup(x => x.SetActiveMutation(It.IsAny<int>()));
            testRunnerMock.Setup(x => x.RunAll()).Returns(new TestRunResult() { Success = true });

            var mutant = new Mutant() { Id = 1 };
            var target = new MutationTestExecutor(testRunnerMock.Object);

            target.Test(mutant);

            mutant.ResultStatus.ShouldBe(MutantStatus.Survived);
            testRunnerMock.Verify(x => x.SetActiveMutation(1), Times.Once);
            testRunnerMock.Verify(x => x.RunAll(), Times.Once);
        }

        [Fact]
        public void MutationTestExecutor_FailedTestShouldBeKilled()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            testRunnerMock.Setup(x => x.SetActiveMutation(It.IsAny<int>()));
            testRunnerMock.Setup(x => x.RunAll()).Returns(new TestRunResult() { Success = false });

            var mutant = new Mutant() { Id = 1 };
            var target = new MutationTestExecutor(testRunnerMock.Object);

            target.Test(mutant);

            mutant.ResultStatus.ShouldBe(MutantStatus.Killed);
            testRunnerMock.Verify(x => x.SetActiveMutation(1), Times.Once);
            testRunnerMock.Verify(x => x.RunAll(), Times.Once);
        }
    }
}
