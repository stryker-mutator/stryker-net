using Moq;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Testing;
using Stryker.Core.TestRunners;
using System;
using System.Collections.Generic;
using Stryker.Core.Logging.TotalNumberOfTests;
using Stryker.Core.Parsers;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class InitialTestProcessTests
    {
        public InitialTestProcess _target { get; set; }

        public InitialTestProcessTests()
        {
            var totalNumberOfTestsLogger = new Mock<ITotalNumberOfTestsLogger>();
            _target = new InitialTestProcess(totalNumberOfTestsLogger.Object);
        }

        [Fact]
        public void InitialTestProcess_ShouldThrowExceptionOnFail()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            testRunnerMock.Setup(x => x.RunAll(It.IsAny<int>())).Returns(new TestRunResult() { Success = false });

            var exception = Assert.Throws<InitialTestRunFailedException>(() => _target.InitialTest(testRunnerMock.Object));
        }

        [Fact]
        public void InitialTestProcess_ShouldCalculateTestTimeout()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            testRunnerMock.Setup(x => x.RunAll(It.IsAny<int>())).Returns(new TestRunResult() { Success = true });

            var result = _target.InitialTest(testRunnerMock.Object);
        }
    }
}
