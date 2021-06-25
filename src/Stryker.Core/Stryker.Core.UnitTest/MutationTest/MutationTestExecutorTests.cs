using System;
using Moq;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.TestRunners;
using System.Collections.Generic;
using Stryker.Core.Initialisation;
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
            testRunnerMock.Setup(x => x.TestMultipleMutants(It.IsAny<ITimeoutValueCalculator>(), It.IsAny<IReadOnlyList<Mutant>>(), null)).Returns(new TestRunResult(true));

            var target = new MutationTestExecutor(testRunnerMock.Object);

            target.Test(new List<Mutant> { mutant }, null, null);

            mutant.ResultStatus.ShouldBe(MutantStatus.Survived);
            testRunnerMock.Verify(x => x.TestMultipleMutants(It.IsAny<ITimeoutValueCalculator>(), It.IsAny<IReadOnlyList<Mutant>>(), null), Times.Once);
        }

        [Fact]
        public void MutationTestExecutor_FailedTestShouldBeKilled()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var mutant = new Mutant { Id = 1, CoveringTests = TestsGuidList.EveryTest() };
            testRunnerMock.Setup(x => x.TestMultipleMutants(null, It.IsAny<IReadOnlyList<Mutant>>(), null)).Returns(new TestRunResult(false));

            var target = new MutationTestExecutor(testRunnerMock.Object);

            target.Test(new List<Mutant> { mutant }, null, null);

            mutant.ResultStatus.ShouldBe(MutantStatus.Killed);
            testRunnerMock.Verify(x => x.TestMultipleMutants(null, It.IsAny<IReadOnlyList<Mutant>>(), null), Times.Once);
        }

        [Fact]
        public void MutationTestExecutor_TimeoutShouldBePassedToProcessTimeout()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var mutant = new Mutant { Id = 1, CoveringTests = TestsGuidList.EveryTest() };
            testRunnerMock.Setup(x => x.TestMultipleMutants(It.IsAny<ITimeoutValueCalculator>(), It.IsAny<IReadOnlyList<Mutant>>(), null)).
                Returns(TestRunResult.TimedOut(TestsGuidList.NoTest(), TestsGuidList.NoTest(), TestsGuidList.EveryTest(), "", TimeSpan.Zero));

            var target = new MutationTestExecutor(testRunnerMock.Object);

            var timeoutValueCalculator = new TimeoutValueCalculator(500);
            target.Test(new List<Mutant> { mutant }, timeoutValueCalculator, null);

            mutant.ResultStatus.ShouldBe(MutantStatus.Timeout);
            testRunnerMock.Verify(x => x.TestMultipleMutants(timeoutValueCalculator, It.IsAny<IReadOnlyList<Mutant>>(), null), Times.Once);
        }
    }
}
