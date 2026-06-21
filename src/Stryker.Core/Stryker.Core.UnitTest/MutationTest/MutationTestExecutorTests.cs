using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Testing;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;
using Stryker.TestRunner.VsTest;

namespace Stryker.Core.UnitTest.MutationTest;

[TestClass]
public class MutationTestExecutorTests : TestBase
{
    [TestMethod]
    public async Task MutationTestExecutor_NoFailedTestShouldBeSurvived()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var mutant = new Mutant { Id = 1 };
        testRunnerMock.Setup(x => x.TestMultipleMutantsAsync(It.IsAny<IProjectAndTests>(), It.IsAny<ITimeoutValueCalculator>(), It.IsAny<IReadOnlyList<IMutant>>(), null)).Returns(Task.FromResult(new TestRunResult(true) as ITestRunResult));

        var loggerMock = new Mock<ILogger<MutationTestExecutor>>();
        var target = new MutationTestExecutor(loggerMock.Object);
        target.TestRunner = testRunnerMock.Object;

        await target.TestAsync(null, new List<IMutant> { mutant }, null, null);

        mutant.ResultStatus.ShouldBe(MutantStatus.Survived);
        testRunnerMock.Verify(x => x.TestMultipleMutantsAsync(It.IsAny<IProjectAndTests>(), It.IsAny<ITimeoutValueCalculator>(), It.IsAny<IReadOnlyList<IMutant>>(), null), Times.Once);
    }

    [TestMethod]
    public async Task MutationTestExecutor_FailedTestShouldBeKilled()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var mutant = new Mutant { Id = 1, CoveringTests = TestIdentifierList.EveryTest() };
        testRunnerMock.Setup(x => x.TestMultipleMutantsAsync(It.IsAny<IProjectAndTests>(), null, It.IsAny<IReadOnlyList<IMutant>>(), null)).Returns(Task.FromResult(new TestRunResult(false) as ITestRunResult));

        var loggerMock = new Mock<ILogger<MutationTestExecutor>>();
        var target = new MutationTestExecutor(loggerMock.Object);
        target.TestRunner = testRunnerMock.Object;

        await target.TestAsync(null, new List<IMutant> { mutant }, null, null);

        mutant.ResultStatus.ShouldBe(MutantStatus.Killed);
        testRunnerMock.Verify(x => x.TestMultipleMutantsAsync(It.IsAny<IProjectAndTests>(), null, It.IsAny<IReadOnlyList<IMutant>>(), null), Times.Once);
    }

    [TestMethod]
    public async Task MutationTestExecutor_TimeoutShouldBePassedToProcessTimeout()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var mutant = new Mutant { Id = 1, CoveringTests = TestIdentifierList.EveryTest() };
        testRunnerMock.Setup(x => x.TestMultipleMutantsAsync(It.IsAny<IProjectAndTests>(), It.IsAny<ITimeoutValueCalculator>(), It.IsAny<IReadOnlyList<IMutant>>(), null)).
            Returns(Task.FromResult(TestRunResult.TimedOut(new List<VsTestDescription>(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), TestIdentifierList.EveryTest(), "", Enumerable.Empty<string>(), TimeSpan.Zero) as ITestRunResult));

        var loggerMock = new Mock<ILogger<MutationTestExecutor>>();
        var target = new MutationTestExecutor(loggerMock.Object);
        target.TestRunner = testRunnerMock.Object;

        var timeoutValueCalculator = new TimeoutValueCalculator(500);
        await target.TestAsync(null, new List<IMutant> { mutant }, timeoutValueCalculator, null);

        mutant.ResultStatus.ShouldBe(MutantStatus.Timeout);
        testRunnerMock.Verify(x => x.TestMultipleMutantsAsync(It.IsAny<IProjectAndTests>(), timeoutValueCalculator, It.IsAny<IReadOnlyList<IMutant>>(), null), Times.Once);
    }

    [TestMethod]
    public async Task MutationTestExecutor_RuntimeErrorShouldBeReportedAsRuntimeError()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var mutant = new Mutant { Id = 1, CoveringTests = TestIdentifierList.EveryTest() };
        testRunnerMock.Setup(x => x.TestMultipleMutantsAsync(It.IsAny<IProjectAndTests>(), It.IsAny<ITimeoutValueCalculator>(), It.IsAny<IReadOnlyList<IMutant>>(), null)).
            Returns(Task.FromResult(TestRunResult.RuntimeError(new List<VsTestDescription>(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), "the test host crashed", Enumerable.Empty<string>(), TimeSpan.Zero) as ITestRunResult));

        var loggerMock = new Mock<ILogger<MutationTestExecutor>>();
        var target = new MutationTestExecutor(loggerMock.Object);
        target.TestRunner = testRunnerMock.Object;

        await target.TestAsync(null, new List<IMutant> { mutant }, null, null);

        mutant.ResultStatus.ShouldBe(MutantStatus.RuntimeError);
        mutant.ResultStatusReason.ShouldNotBeNullOrEmpty();
        testRunnerMock.Verify(x => x.TestMultipleMutantsAsync(It.IsAny<IProjectAndTests>(), It.IsAny<ITimeoutValueCalculator>(), It.IsAny<IReadOnlyList<IMutant>>(), null), Times.Once);
        // a crash is not a test failure: it must not be logged as an error
        loggerMock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Never);
    }

    [TestMethod]
    public async Task MutationTestExecutor_ShouldSwitchToSingleModeOnRuntimeError()
    {
        // A crash in a batch can't be attributed to one mutant, so the batch is rerun one by one
        // to isolate the culprit (mirrors the dubious-timeout behaviour).
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var mutant1 = new Mutant { Id = 1, CoveringTests = TestIdentifierList.EveryTest() };
        var mutant2 = new Mutant { Id = 2, CoveringTests = TestIdentifierList.EveryTest() };
        testRunnerMock.Setup(x => x.TestMultipleMutantsAsync(It.IsAny<IProjectAndTests>(), It.IsAny<ITimeoutValueCalculator>(), It.IsAny<IReadOnlyList<IMutant>>(), null)).
            Returns(Task.FromResult(TestRunResult.RuntimeError(new List<VsTestDescription>(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), "the test host crashed", Enumerable.Empty<string>(), TimeSpan.Zero) as ITestRunResult));

        var loggerMock = new Mock<ILogger<MutationTestExecutor>>();
        var target = new MutationTestExecutor(loggerMock.Object);
        target.TestRunner = testRunnerMock.Object;

        await target.TestAsync(null, new List<IMutant> { mutant1, mutant2 }, null, null);

        mutant1.ResultStatus.ShouldBe(MutantStatus.RuntimeError);
        mutant2.ResultStatus.ShouldBe(MutantStatus.RuntimeError);
        testRunnerMock.Verify(x => x.TestMultipleMutantsAsync(It.IsAny<IProjectAndTests>(), It.IsAny<ITimeoutValueCalculator>(), It.IsAny<IReadOnlyList<IMutant>>(), null), Times.Exactly(3));
    }

    [TestMethod]
    public async Task MutationTestExecutor_ShouldSwitchToSingleModeOnDubiousTimeouts()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var mutant1 = new Mutant { Id = 1, CoveringTests = TestIdentifierList.EveryTest() };
        var mutant2 = new Mutant { Id = 2, CoveringTests = TestIdentifierList.EveryTest() };
        testRunnerMock.Setup(x => x.TestMultipleMutantsAsync(It.IsAny<IProjectAndTests>(), It.IsAny<ITimeoutValueCalculator>(), It.IsAny<IReadOnlyList<IMutant>>(), null)).
            Returns(Task.FromResult(TestRunResult.TimedOut(new List<VsTestDescription>(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), "", Enumerable.Empty<string>(), TimeSpan.Zero) as ITestRunResult));

        var loggerMock = new Mock<ILogger<MutationTestExecutor>>();
        var target = new MutationTestExecutor(loggerMock.Object);
        target.TestRunner = testRunnerMock.Object;

        var timeoutValueCalculator = new TimeoutValueCalculator(500);
        await target.TestAsync(null, new List<IMutant> { mutant1, mutant2 }, timeoutValueCalculator, null);

        mutant1.ResultStatus.ShouldBe(MutantStatus.Timeout);
        mutant2.ResultStatus.ShouldBe(MutantStatus.Timeout);
        testRunnerMock.Verify(x => x.TestMultipleMutantsAsync(It.IsAny<IProjectAndTests>(), timeoutValueCalculator, It.IsAny<IReadOnlyList<IMutant>>(), null), Times.Exactly(3));
    }
}
