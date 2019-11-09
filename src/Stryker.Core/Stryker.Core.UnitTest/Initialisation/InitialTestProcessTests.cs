using Moq;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using Stryker.Core.TestRunners;
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
            testRunnerMock.Setup(x => x.RunAll(It.IsAny<int?>(), null)).Returns(new TestRunResult { Success = false });
            testRunnerMock.Setup(x => x.CaptureCoverage(false, false))
                .Returns(new TestRunResult { Success = false });
            testRunnerMock.Setup(x => x.DiscoverNumberOfTests()).Returns(0);

            var exception = Assert.Throws<StrykerInputException>(() => _target.InitialTest(testRunnerMock.Object));
        }

        [Fact]
        public void InitialTestProcess_ShouldCalculateTestTimeout()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            testRunnerMock.Setup(x => x.RunAll(It.IsAny<int?>(), null)).Returns(new TestRunResult { Success = true });
            testRunnerMock.Setup(x => x.CaptureCoverage(false, false))
                .Returns(new TestRunResult { Success = true });
            testRunnerMock.Setup(x => x.DiscoverNumberOfTests()).Returns(0);

            var result = _target.InitialTest(testRunnerMock.Object);
        }
    }
}
