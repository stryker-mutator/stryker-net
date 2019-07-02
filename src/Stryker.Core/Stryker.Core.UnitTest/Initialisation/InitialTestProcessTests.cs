﻿using Moq;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.TestRunners;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class InitialTestProcessTests
    {
        public InitialTestProcess _target { get; set; }

        public InitialTestProcessTests()
        {
            _target = new InitialTestProcess();
        }

        [Fact]
        public void InitialTestProcess_ShouldThrowExceptionOnFail()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            testRunnerMock.Setup(x => x.RunAll(It.IsAny<int?>(), It.IsAny<int?>())).Returns(new TestRunResult { Success = false });
            testRunnerMock.Setup(x => x.CaptureCoverage())
                .Returns(new TestRunResult {Success = false});

            var exception = Assert.Throws<StrykerInputException>(() => _target.InitialTest(testRunnerMock.Object));
        }

        [Fact]
        public void InitialTestProcess_ShouldCalculateTestTimeout()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            testRunnerMock.Setup(x => x.RunAll(It.IsAny<int?>(), It.IsAny<int?>())).Returns(new TestRunResult { Success = true });
            testRunnerMock.Setup(x => x.CaptureCoverage())
                .Returns(new TestRunResult {Success = true});

            var result = _target.InitialTest(testRunnerMock.Object);
        }
    }
}
