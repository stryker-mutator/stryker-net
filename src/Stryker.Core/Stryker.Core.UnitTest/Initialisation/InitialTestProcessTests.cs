using System;
using System.Collections.Generic;
using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.TestRunners;
using System.Threading;
using Stryker.Core.Options;
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
        public void InitialTestProcess_ShouldThrowExceptionOnFail()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var failedTest = Guid.NewGuid();
            var successfulTest = Guid.NewGuid();
            var ranTests = new TestGuidsList(failedTest, successfulTest);
            var failedTests = new TestGuidsList(failedTest);
            testRunnerMock.Setup(x => x.InitialTest()).Returns(new TestRunResult(ranTests, failedTests, TestGuidsList.NoTest(), string.Empty, TimeSpan.Zero) );
            testRunnerMock.Setup(x => x.DiscoverTests()).Returns(new TestSet());

            Assert.Throws<InputException>(() => _target.InitialTest(_options, testRunnerMock.Object));
        }

        [Fact]
        public void InitialTestProcess_ShouldNotThrowIfAFewTestsFail()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var test1 = Guid.NewGuid();
            var testList = new List<Guid>(10);
            testList.Add(test1);
            for (int i = testList.Count; i < testList.Capacity; i++)
            {
                testList.Add(Guid.NewGuid());
            }
            var ranTests = new TestGuidsList(testList);
            var failedTests = new TestGuidsList(test1);
            testRunnerMock.Setup(x => x.InitialTest()).Returns(new TestRunResult(ranTests, failedTests, TestGuidsList.NoTest(), string.Empty, TimeSpan.Zero) );
            testRunnerMock.Setup(x => x.DiscoverTests()).Returns(new TestSet());

            _target.InitialTest(_options, testRunnerMock.Object);
        }

        [Fact]
        public void InitialTestProcess_ShouldCalculateTestTimeout()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            testRunnerMock.Setup(x => x.InitialTest()).Callback(() => Thread.Sleep(2)).Returns(new TestRunResult(true));
            testRunnerMock.Setup(x => x.DiscoverTests()).Returns(new TestSet());

            var result = _target.InitialTest(_options, testRunnerMock.Object);
            
            result.TimeoutValueCalculator.DefaultTimeout.ShouldBeInRange(1, 200, "This test contains a Thread.Sleep to simulate time passing as this test is testing that a stopwatch is used correctly to measure time.\n If this test is failing for unclear reasons, perhaps the computer running the test is too slow causing the time estimation to be off");
        }
    }
}
