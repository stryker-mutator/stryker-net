using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moq;
using Shouldly;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.TestRunners;
using Stryker.Core.TestRunners.VsTest;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class InitialTestProcessTests : TestBase
    {
        private readonly InitialTestProcess _target;
        private readonly StrykerOptions _options;

        public InitialTestProcessTests()
        {
            _target = new InitialTestProcess();
            _options = new StrykerOptions
            {
                AdditionalTimeout = 0
            };
        }
   
        [Fact]
        public void InitialTestProcess_ShouldNotThrowIfAFewTestsFail()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var test1 = Guid.NewGuid();
            var testList = new List<Guid>(10)
            {
                test1
            };
            for (var i = testList.Count; i < testList.Capacity; i++)
            {
                testList.Add(Guid.NewGuid());
            }
            var ranTests = new TestGuidsList(testList);
            var failedTests = new TestGuidsList(test1);
            testRunnerMock.Setup(x => x.InitialTest(It.IsAny<IProjectAndTests>())).Returns(new TestRunResult(Enumerable.Empty<VsTestDescription>(), ranTests, failedTests, TestGuidsList.NoTest(), string.Empty, Enumerable.Empty<string>(), TimeSpan.Zero) );
            testRunnerMock.Setup(x => x.DiscoverTests(It.IsAny<string>())).Returns(true);
            testRunnerMock.Setup(x => x.GetTests(It.IsAny<IProjectAndTests>())).Returns(new TestSet());

            _target.InitialTest(_options, null, testRunnerMock.Object);

            testRunnerMock.Verify(p => p.InitialTest(It.IsAny<IProjectAndTests>()), Times.Once);
        }

        [Fact]
        public void InitialTestProcess_ShouldCalculateTestTimeout()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            testRunnerMock.Setup(x => x.InitialTest(It.IsAny<IProjectAndTests>())).Callback(() => Thread.Sleep(10)).Returns(new TestRunResult(true));
            testRunnerMock.Setup(x => x.DiscoverTests(It.IsAny<string>())).Returns(true);
            testRunnerMock.Setup(x => x.GetTests(It.IsAny<IProjectAndTests>())).Returns(new TestSet());
            var result = _target.InitialTest( _options, null, testRunnerMock.Object);

            result.TimeoutValueCalculator.DefaultTimeout.ShouldBeInRange(1, 200, "This test contains a Thread.Sleep to simulate time passing as this test is testing that a stopwatch is used correctly to measure time.\n If this test is failing for unclear reasons, perhaps the computer running the test is too slow causing the time estimation to be off");
        }
    }
}
