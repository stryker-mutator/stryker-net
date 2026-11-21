using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.Results;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class CoverageCaptureProgressTests
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);

    [TestMethod]
    public void Add_ShouldNotLog_BeforeTheIntervalHasPassed()
    {
        var time = new ManualTimeProvider();
        var logger = new ListLogger();
        var progress = new CoverageCaptureProgress(logger, 10, Interval, time);

        progress.Add(Result(CoverageConfidence.Normal));
        time.Advance(Interval - TimeSpan.FromSeconds(1));
        progress.Add(Result(CoverageConfidence.Normal));

        logger.Messages.ShouldBeEmpty();
    }

    [TestMethod]
    public void Add_ShouldLogCapturedAndDubiousCounts_OnceTheIntervalHasPassed()
    {
        var time = new ManualTimeProvider();
        var logger = new ListLogger();
        var progress = new CoverageCaptureProgress(logger, 10, Interval, time);

        progress.Add(Result(CoverageConfidence.Normal));
        progress.Add(Result(CoverageConfidence.Dubious));
        time.Advance(Interval);
        progress.Add(Result(CoverageConfidence.Normal));

        logger.Messages.ShouldBe(["Captured coverage for 3/10 tests (1 dubious)"]);
        progress.Dubious.ShouldBe(1);
    }

    [TestMethod]
    public void Add_ShouldLogAtMostOncePerInterval()
    {
        var time = new ManualTimeProvider();
        var logger = new ListLogger();
        var progress = new CoverageCaptureProgress(logger, 10, Interval, time);

        time.Advance(Interval);
        progress.Add(Result(CoverageConfidence.Normal));
        progress.Add(Result(CoverageConfidence.Normal));
        time.Advance(Interval);
        progress.Add(Result(CoverageConfidence.Dubious));

        logger.Messages.ShouldBe([
            "Captured coverage for 1/10 tests (0 dubious)",
            "Captured coverage for 3/10 tests (1 dubious)",
        ]);
    }

    private static ICoverageRunResult Result(CoverageConfidence confidence) =>
        CoverageRunResult.Create("test", confidence, [], [], []);
}
