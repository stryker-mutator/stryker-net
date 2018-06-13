using Moq;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Testing;
using Stryker.Core.TestRunners;
using System;
using System.Collections.Generic;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class InitialTestProcessTests
    {
        [Fact]
        public void InitialTestProcess_ShouldThrowExceptionOnFail()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            testRunnerMock.Setup(x => x.RunAll(It.IsAny<int>())).Returns(new TestRunResult() { Success = false });

            var target = new InitialTestProcess();

            var exception = Assert.Throws<InitialTestRunFailedException>(() => target.InitialTest(testRunnerMock.Object));
        }

        [Fact]
        public void InitialTestProcess_ShouldCalculateTestTimeout()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            testRunnerMock.Setup(x => x.RunAll(It.IsAny<int>())).Returns(new TestRunResult() { Success = true });

            var target = new InitialTestProcess();

            var result = target.InitialTest(testRunnerMock.Object);

            ;
        }
    }
}
