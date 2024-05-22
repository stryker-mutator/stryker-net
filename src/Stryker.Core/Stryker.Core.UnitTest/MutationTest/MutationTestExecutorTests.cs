using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Shouldly;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Testing;
using Stryker.Shared.Initialisation;
using Stryker.Shared.Mutants;
using Stryker.Shared.Tests;
using Stryker.TestRunner.VSTest;
using Xunit;

namespace Stryker.Core.UnitTest.MutationTest;

public class MutationTestExecutorTests : TestBase
{
    [Fact]
    public void MutationTestExecutor_NoFailedTestShouldBeSurvived()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var mutant = new Mutant { Id = 1 };
        testRunnerMock.Setup(x => x.TestMultipleMutants( It.IsAny<IProjectAndTests>(),It.IsAny<ITimeoutValueCalculator>(), It.IsAny<IReadOnlyList<IMutant>>(), null)).Returns(new TestRunResult(true));

        var target = new MutationTestExecutor(testRunnerMock.Object);

        target.Test(null, new List<IMutant> { mutant }, null, null);

        mutant.ResultStatus.ShouldBe(MutantStatus.Survived);
        testRunnerMock.Verify(x => x.TestMultipleMutants( It.IsAny<IProjectAndTests>(),It.IsAny<ITimeoutValueCalculator>(), It.IsAny<IReadOnlyList<IMutant>>(), null), Times.Once);
    }

    [Fact]
    public void MutationTestExecutor_FailedTestShouldBeKilled()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var mutant = new Mutant { Id = 1, CoveringTests = TestIdentifiers.EveryTest() };
        testRunnerMock.Setup(x => x.TestMultipleMutants( It.IsAny<IProjectAndTests>(),null, It.IsAny<IReadOnlyList<IMutant>>(), null)).Returns(new TestRunResult(false));

        var target = new MutationTestExecutor(testRunnerMock.Object);

        target.Test(null,new List<IMutant> { mutant }, null, null);

        mutant.ResultStatus.ShouldBe(MutantStatus.Killed);
        testRunnerMock.Verify(x => x.TestMultipleMutants( It.IsAny<IProjectAndTests>(),null, It.IsAny<IReadOnlyList<IMutant>>(), null), Times.Once);
    }

    [Fact]
    public void MutationTestExecutor_TimeoutShouldBePassedToProcessTimeout()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var mutant = new Mutant { Id = 1, CoveringTests = TestIdentifiers.EveryTest() };
        testRunnerMock.Setup(x => x.TestMultipleMutants( It.IsAny<IProjectAndTests>(),It.IsAny<ITimeoutValueCalculator>(), It.IsAny<IReadOnlyList<IMutant>>(), null)).
            Returns(TestRunResult.TimedOut(new List<VsTestDescription>(), TestIdentifiers.NoTest(), TestIdentifiers.NoTest(), TestIdentifiers.EveryTest(), "", Enumerable.Empty<string>(), TimeSpan.Zero));

        var target = new MutationTestExecutor(testRunnerMock.Object);

        var timeoutValueCalculator = new TimeoutValueCalculator(500);
        target.Test(null, new List<IMutant> { mutant }, timeoutValueCalculator, null);

        mutant.ResultStatus.ShouldBe(MutantStatus.Timeout);
        testRunnerMock.Verify(x => x.TestMultipleMutants( It.IsAny<IProjectAndTests>(), timeoutValueCalculator, It.IsAny<IReadOnlyList<IMutant>>(), null), Times.Once);
    }

    [Fact]
    public void MutationTestExecutor_ShouldSwitchToSingleModeOnDubiousTimeouts()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var mutant1 = new Mutant { Id = 1, CoveringTests = TestIdentifiers.EveryTest() };
        var mutant2 = new Mutant { Id = 2, CoveringTests = TestIdentifiers.EveryTest() };
        testRunnerMock.Setup(x => x.TestMultipleMutants( It.IsAny<IProjectAndTests>(),It.IsAny<ITimeoutValueCalculator>(), It.IsAny<IReadOnlyList<IMutant>>(), null)).
            Returns(TestRunResult.TimedOut(new List<VsTestDescription>(), TestIdentifiers.NoTest(), TestIdentifiers.NoTest(), TestIdentifiers.NoTest(), "", Enumerable.Empty<string>(), TimeSpan.Zero));

        var target = new MutationTestExecutor(testRunnerMock.Object);

        var timeoutValueCalculator = new TimeoutValueCalculator(500);
        target.Test(null, new List<IMutant> { mutant1, mutant2 }, timeoutValueCalculator, null);

        mutant1.ResultStatus.ShouldBe(MutantStatus.Timeout);
        mutant2.ResultStatus.ShouldBe(MutantStatus.Timeout);
        testRunnerMock.Verify(x => x.TestMultipleMutants( It.IsAny<IProjectAndTests>(),timeoutValueCalculator, It.IsAny<IReadOnlyList<IMutant>>(), null), Times.Exactly(3));
    }
}
