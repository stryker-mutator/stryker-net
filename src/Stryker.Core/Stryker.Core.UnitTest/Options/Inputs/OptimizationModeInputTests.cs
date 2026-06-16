using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Stryker.Abstractions.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Abstractions.Options;
using Stryker.Configuration.Options.Inputs;
using RunnerKind = Stryker.Abstractions.Options.TestRunner;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class OptimizationModeInputTests : TestBase
{
    private readonly Mock<ILogger<CoverageAnalysisInput>> _loggerMock = new();

    private const string PromotionWarning =
        "The Microsoft Test Platform runner captures per-test coverage in isolation; 'perTest' "
        + "(process reuse) is not yet available and has been upgraded to 'perTestInIsolation'. "
        + "Process reuse for MTP is planned as a follow-up.";

    [TestMethod]
    [DataRow(null)]
    [DataRow("perTest")]
    public void ShouldPromotePerTestToIsolationForMtp(string value)
    {
        var result = new CoverageAnalysisInput { SuppliedInput = value }
            .Validate(RunnerKind.MicrosoftTestPlatform, _loggerMock.Object);

        result.HasFlag(OptimizationModes.CoverageBasedTest).ShouldBeTrue();
        result.HasFlag(OptimizationModes.CaptureCoveragePerTest).ShouldBeTrue();
    }

    [TestMethod]
    public void ShouldWarnWhenMtpPromotesExplicitPerTest()
    {
        new CoverageAnalysisInput { SuppliedInput = "perTest" }
            .Validate(RunnerKind.MicrosoftTestPlatform, _loggerMock.Object);

        _loggerMock.Verify(LogLevel.Warning, PromotionWarning, Times.Once);
    }

    [TestMethod]
    public void ShouldNotWarnWhenMtpUsesDefault()
    {
        new CoverageAnalysisInput { SuppliedInput = null }
            .Validate(RunnerKind.MicrosoftTestPlatform, _loggerMock.Object);

        _loggerMock.Verify(LogLevel.Warning, PromotionWarning, Times.Never);
    }

    [TestMethod]
    public void ShouldNotPromoteForVsTest()
    {
        var result = new CoverageAnalysisInput { SuppliedInput = "perTest" }
            .Validate(RunnerKind.VsTest, _loggerMock.Object);

        result.HasFlag(OptimizationModes.CaptureCoveragePerTest).ShouldBeFalse();
        _loggerMock.Verify(LogLevel.Warning, PromotionWarning, Times.Never);
    }

    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new CoverageAnalysisInput();
        target.HelpText.ShouldBe(@"Use coverage info to speed up execution. Possible values are: off, perTest, all, perTestInIsolation.
	- off: Coverage data is not captured. Every mutant is tested against all test. Slowest, use in case of doubt.
	- perTest: Capture mutations covered by each test. Mutations are tested against covering tests (or flagged NoCoverage if no test cover them). Fastest option.
	- all: Capture the list of mutations covered by some test. Test only these mutations, other are flagged as NoCoverage. Fast option.
	- perTestInIsolation: 'perTest' but coverage of each test is captured in isolation. Increase coverage accuracy at the expense of a slow init phase.
 | default: 'perTest'");
    }

    [TestMethod]
    [DataRow(null, OptimizationModes.CoverageBasedTest)]
    [DataRow("perTestinisolation", OptimizationModes.CoverageBasedTest, OptimizationModes.CaptureCoveragePerTest)]
    [DataRow("perTest", OptimizationModes.CoverageBasedTest)]
    [DataRow("all", OptimizationModes.SkipUncoveredMutants)]
    [DataRow("off", OptimizationModes.None)]
    public void ShouldSetFlags(string value, params OptimizationModes[] expectedFlags)
    {
        var target = new CoverageAnalysisInput { SuppliedInput = value };

        var result = target.Validate();

        foreach (var flag in expectedFlags)
        {
            result.HasFlag(flag).ShouldBeTrue();
        }
    }

    [TestMethod]
    public void ShouldThrowOnInvalidOptimizationMode()
    {
        var target = new CoverageAnalysisInput { SuppliedInput = "gibberish" };

        var exception = Should.Throw<InputException>(() => target.Validate());

        exception.Message.ShouldMatch("Incorrect coverageAnalysis option \\(gibberish\\)\\. The options are \\[.+\\]\\.");

        exception.ToString().ShouldContain("[off, perTest, all, perTestInIsolation].");
    }
}
