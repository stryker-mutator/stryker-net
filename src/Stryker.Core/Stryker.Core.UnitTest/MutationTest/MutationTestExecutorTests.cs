using System;
using System.Collections.Generic;
using System.Linq;
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
    public void MutationTestExecutor_NoFailedTestShouldBeSurvived()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var mutant = new Mutant { Id = 1 };
        testRunnerMock.Setup(x => x.TestMultipleMutants(It.IsAny<IProjectAndTests>(), It.IsAny<ITimeoutValueCalculator>(), It.IsAny<IReadOnlyList<IMutant>>(), null)).Returns(new TestRunResult(true));

        var loggerMock = new Mock<ILogger<MutationTestExecutor>>();
        var target = new MutationTestExecutor(loggerMock.Object);
        target.TestRunner = testRunnerMock.Object;

        target.Test(null, new List<IMutant> { mutant }, null, null);

        mutant.ResultStatus.ShouldBe(MutantStatus.Survived);
        testRunnerMock.Verify(x => x.TestMultipleMutants(It.IsAny<IProjectAndTests>(), It.IsAny<ITimeoutValueCalculator>(), It.IsAny<IReadOnlyList<IMutant>>(), null), Times.Once);
    }

    [TestMethod]
    public void MutationTestExecutor_FailedTestShouldBeKilled()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var mutant = new Mutant { Id = 1, CoveringTests = TestIdentifierList.EveryTest() };
        testRunnerMock.Setup(x => x.TestMultipleMutants(It.IsAny<IProjectAndTests>(), null, It.IsAny<IReadOnlyList<IMutant>>(), null)).Returns(new TestRunResult(false));

        var loggerMock = new Mock<ILogger<MutationTestExecutor>>();
        var target = new MutationTestExecutor(loggerMock.Object);
        target.TestRunner = testRunnerMock.Object;

        target.Test(null, new List<IMutant> { mutant }, null, null);

        mutant.ResultStatus.ShouldBe(MutantStatus.Killed);
        testRunnerMock.Verify(x => x.TestMultipleMutants(It.IsAny<IProjectAndTests>(), null, It.IsAny<IReadOnlyList<IMutant>>(), null), Times.Once);
    }

    [TestMethod]
    public void MutationTestExecutor_TimeoutShouldBePassedToProcessTimeout()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var mutant = new Mutant { Id = 1, CoveringTests = TestIdentifierList.EveryTest() };
        testRunnerMock.Setup(x => x.TestMultipleMutants(It.IsAny<IProjectAndTests>(), It.IsAny<ITimeoutValueCalculator>(), It.IsAny<IReadOnlyList<IMutant>>(), null)).
            Returns(TestRunResult.TimedOut(new List<VsTestDescription>(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), TestIdentifierList.EveryTest(), "", Enumerable.Empty<string>(), TimeSpan.Zero));

        var loggerMock = new Mock<ILogger<MutationTestExecutor>>();
        var target = new MutationTestExecutor(loggerMock.Object);
        target.TestRunner = testRunnerMock.Object;

        var timeoutValueCalculator = new TimeoutValueCalculator(500);
        target.Test(null, new List<IMutant> { mutant }, timeoutValueCalculator, null);

        mutant.ResultStatus.ShouldBe(MutantStatus.Timeout);
        testRunnerMock.Verify(x => x.TestMultipleMutants(It.IsAny<IProjectAndTests>(), timeoutValueCalculator, It.IsAny<IReadOnlyList<IMutant>>(), null), Times.Once);
    }

    [TestMethod]
    public void MutationTestExecutor_ShouldSwitchToSingleModeOnDubiousTimeouts()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var mutant1 = new Mutant { Id = 1, CoveringTests = TestIdentifierList.EveryTest() };
        var mutant2 = new Mutant { Id = 2, CoveringTests = TestIdentifierList.EveryTest() };
        testRunnerMock.Setup(x => x.TestMultipleMutants(It.IsAny<IProjectAndTests>(), It.IsAny<ITimeoutValueCalculator>(), It.IsAny<IReadOnlyList<IMutant>>(), null)).
            Returns(TestRunResult.TimedOut(new List<VsTestDescription>(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), "", Enumerable.Empty<string>(), TimeSpan.Zero));

        var loggerMock = new Mock<ILogger<MutationTestExecutor>>();
        var target = new MutationTestExecutor(loggerMock.Object);
        target.TestRunner = testRunnerMock.Object;

        var timeoutValueCalculator = new TimeoutValueCalculator(500);
        target.Test(null, new List<IMutant> { mutant1, mutant2 }, timeoutValueCalculator, null);

        mutant1.ResultStatus.ShouldBe(MutantStatus.Timeout);
        mutant2.ResultStatus.ShouldBe(MutantStatus.Timeout);
        testRunnerMock.Verify(x => x.TestMultipleMutants(It.IsAny<IProjectAndTests>(), timeoutValueCalculator, It.IsAny<IReadOnlyList<IMutant>>(), null), Times.Exactly(3));
    }
}
