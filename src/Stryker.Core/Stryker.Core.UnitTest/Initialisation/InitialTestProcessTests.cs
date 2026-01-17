using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Testing;
using Stryker.Core.Initialisation;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;
using Stryker.TestRunner.VsTest;
using Stryker.Utilities;

namespace Stryker.Core.UnitTest.Initialisation;

[TestClass]
public class InitialTestProcessTests : TestBase
{
    private readonly InitialTestProcess _target;
    private readonly StrykerOptions _options;

    public InitialTestProcessTests()
    {
        _target = new InitialTestProcess(TestLoggerFactory.CreateLogger<InitialTestProcess>());
        _options = new StrykerOptions
        {
            AdditionalTimeout = 0
        };
    }

    [TestMethod]
    public void InitialTestProcess_ShouldNotThrowIfAFewTestsFail()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var test1 = "test1";
        var testList = new List<string>(10)
        {
            test1
        };
        for (var i = testList.Count; i < testList.Capacity; i++)
        {
            testList.Add("test"+i);
        }
        var ranTests = new TestIdentifierList(testList);
        var failedTests = new TestIdentifierList(test1);
        testRunnerMock.Setup(x => x.InitialTestAsync(It.IsAny<IProjectAndTests>())).Returns(new TestRunResult(Enumerable.Empty<VsTestDescription>(), ranTests, failedTests, TestIdentifierList.NoTest(), string.Empty, Enumerable.Empty<string>(), TimeSpan.Zero));
        testRunnerMock.Setup(x => x.DiscoverTestsAsync(It.IsAny<string>())).Returns(true);
        testRunnerMock.Setup(x => x.GetTests(It.IsAny<IProjectAndTests>())).Returns(new TestSet());

        _target.InitialTest(_options, null, testRunnerMock.Object);

        testRunnerMock.Verify(p => p.InitialTestAsync(It.IsAny<IProjectAndTests>()), Times.Once);
    }

    [TestMethod]
    public void InitialTestProcess_ShouldCalculateTestTimeout()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        testRunnerMock.Setup(x => x.InitialTestAsync(It.IsAny<IProjectAndTests>())).Callback(() => Thread.Sleep(10)).Returns(new TestRunResult(true));
        testRunnerMock.Setup(x => x.DiscoverTestsAsync(It.IsAny<string>())).Returns(true);
        testRunnerMock.Setup(x => x.GetTests(It.IsAny<IProjectAndTests>())).Returns(new TestSet());
        var result = _target.InitialTest(_options, null, testRunnerMock.Object);

        result.TimeoutValueCalculator.DefaultTimeout.ShouldBeInRange(1, 200, "This test contains a Thread.Sleep to simulate time passing as this test is testing that a stopwatch is used correctly to measure time.\n If this test is failing for unclear reasons, perhaps the computer running the test is too slow causing the time estimation to be off");
    }
}
