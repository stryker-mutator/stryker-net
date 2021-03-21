using System.Collections.Generic;
using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.TestRunners;
using System.Threading;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class InitialTestProcessTests
    {
        private readonly InitialTestProcess _target;

        public InitialTestProcessTests()
        {
            _target = new InitialTestProcess();
        }

        [Fact]
        public void InitialTestProcess_ShouldThrowExceptionOnFail()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            testRunnerMock.Setup(x => x.InitialTest()).Returns(new TestRunResult(false) );
            testRunnerMock.Setup(x => x.CaptureCoverage( It.IsAny<List<Mutant>>(),false, false))
                .Returns(new TestRunResult(true));
            testRunnerMock.Setup(x => x.DiscoverNumberOfTests()).Returns(1);

            var exception = Assert.Throws<StrykerInputException>(() => _target.InitialTest(testRunnerMock.Object));
        }

        [Fact]
        public void InitialTestProcess_ShouldCalculateTestTimeout()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            testRunnerMock.Setup(x => x.InitialTest()).Callback(() => Thread.Sleep(2)).Returns(new TestRunResult(true));
            testRunnerMock.Setup(x => x.CaptureCoverage(It.IsAny<List<Mutant>>(),false, false))
                .Returns(new TestRunResult(true));
            testRunnerMock.Setup(x => x.DiscoverNumberOfTests()).Returns(2);

            var result = _target.InitialTest(testRunnerMock.Object);

            result.ShouldBeInRange(1, 200, "This test contains a Thread.Sleep to simulate time passing as this test is testing that a stopwatch is used correctly to measure time.\n If this test is failing for unclear reasons, perhaps the computer running the test is too slow causing the time estimation to be off");
        }
    }
}
