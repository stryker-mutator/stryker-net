using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters.Progress;

namespace Stryker.Core.UnitTest.Reporters.Progress;

[TestClass]
public class ProgressBarReporterTests
{
    [TestMethod]
    public void ReportInitialState_ShouldReportTestProgressAs0PercentageDone_WhenTotalNumberOfTestsIsTwo()
    {
        var progressBarMock = new Mock<IProgressBar>(MockBehavior.Strict);
        progressBarMock.Setup(x => x.Start(It.IsAny<int>(), It.IsAny<string>()));

        var progressBarReporter = new ProgressBarReporter(progressBarMock.Object, new FixedClock());

        progressBarReporter.ReportInitialState(3);

        progressBarMock.Verify(x => x.Start(
            It.Is<int>(a => a == 3),
            It.Is<string>(b => b == "│ Testing mutant 0 / 3 │ K 0 │ S 0 │ T 0 │ E 0 │ NA │")
        ));
    }

    [TestMethod]
    public void ShouldSupportWhenNoMutants()
    {
        var progressBarMock = new ProgressBar();

        var progressBarReporter = new ProgressBarReporter(progressBarMock, new FixedClock());
        // the progress bar was never initialized
        progressBarMock.Ticks().ShouldBe(-1);
        progressBarReporter.ReportFinalState();
        // the progress bar was never initialized
        progressBarMock.Ticks().ShouldBe(-1);
    }

    [TestMethod]
    [DataRow(MutantStatus.Killed, "│ Testing mutant 1 / 2 │ K 1 │ S 0 │ T 0 │ E 0 │ ~0m 00s │")]
    [DataRow(MutantStatus.Survived, "│ Testing mutant 1 / 2 │ K 0 │ S 1 │ T 0 │ E 0 │ ~0m 00s │")]
    [DataRow(MutantStatus.Timeout, "│ Testing mutant 1 / 2 │ K 0 │ S 0 │ T 1 │ E 0 │ ~0m 00s │")]
    [DataRow(MutantStatus.RuntimeError, "│ Testing mutant 1 / 2 │ K 0 │ S 0 │ T 0 │ E 1 │ ~0m 00s │")]
    public void ReportRunTest_ShouldReportTestProgressAs50PercentageDone_And_FirstTestExecutionTime_WhenHalfOfTestsAreDone(MutantStatus status, string expected)
    {
        var progressBarMock = new Mock<IProgressBar>(MockBehavior.Strict);
        progressBarMock.Setup(x => x.Start(It.IsAny<int>(), It.IsAny<string>()));
        progressBarMock.Setup(x => x.Tick(It.IsAny<string>()));

        var progressBarReporter = new ProgressBarReporter(progressBarMock.Object, new FixedClock());

        var mutantTestResult = new Mutant()
        {
            ResultStatus = status
        };

        progressBarReporter.ReportInitialState(2);
        progressBarReporter.ReportRunTest(mutantTestResult);

        progressBarMock.Verify(x => x.Tick(
            It.Is<string>(b => b == expected)
        ));
    }

    [TestMethod]
    [DataRow(MutantStatus.Killed, "│ Testing mutant 1 / 10000 │ K 1 │ S 0 │ T 0 │ E 0 │ ~1m 39s │")]
    [DataRow(MutantStatus.Survived, "│ Testing mutant 1 / 10000 │ K 0 │ S 1 │ T 0 │ E 0 │ ~1m 39s │")]
    [DataRow(MutantStatus.Timeout, "│ Testing mutant 1 / 10000 │ K 0 │ S 0 │ T 1 │ E 0 │ ~1m 39s │")]
    [DataRow(MutantStatus.RuntimeError, "│ Testing mutant 1 / 10000 │ K 0 │ S 0 │ T 0 │ E 1 │ ~1m 39s │")]
    public void ReportRunTest_TestExecutionTimeInMinutes(MutantStatus status, string expected)
    {
        var progressBarMock = new Mock<IProgressBar>(MockBehavior.Strict);
        progressBarMock.Setup(x => x.Start(It.IsAny<int>(), It.IsAny<string>()));
        progressBarMock.Setup(x => x.Tick(It.IsAny<string>()));

        var progressBarReporter = new ProgressBarReporter(progressBarMock.Object, new FixedClock());

        var mutantTestResult = new Mutant()
        {
            ResultStatus = status
        };

        progressBarReporter.ReportInitialState(10000);
        progressBarReporter.ReportRunTest(mutantTestResult);

        progressBarMock.Verify(x => x.Tick(
            It.Is<string>(b => b == expected)
        ));
    }

    [TestMethod]
    [DataRow(MutantStatus.Killed, "│ Testing mutant 1 / 1000000 │ K 1 │ S 0 │ T 0 │ E 0 │ ~2h 46m │")]
    [DataRow(MutantStatus.Survived, "│ Testing mutant 1 / 1000000 │ K 0 │ S 1 │ T 0 │ E 0 │ ~2h 46m │")]
    [DataRow(MutantStatus.Timeout, "│ Testing mutant 1 / 1000000 │ K 0 │ S 0 │ T 1 │ E 0 │ ~2h 46m │")]
    [DataRow(MutantStatus.RuntimeError, "│ Testing mutant 1 / 1000000 │ K 0 │ S 0 │ T 0 │ E 1 │ ~2h 46m │")]
    public void ReportRunTest_TestExecutionTimeInHours(MutantStatus status, string expected)
    {
        var progressBarMock = new Mock<IProgressBar>(MockBehavior.Strict);
        progressBarMock.Setup(x => x.Start(It.IsAny<int>(), It.IsAny<string>()));
        progressBarMock.Setup(x => x.Tick(It.IsAny<string>()));

        var progressBarReporter = new ProgressBarReporter(progressBarMock.Object, new FixedClock());

        var mutantTestResult = new Mutant()
        {
            ResultStatus = status
        };

        progressBarReporter.ReportInitialState(1000000);
        progressBarReporter.ReportRunTest(mutantTestResult);

        progressBarMock.Verify(x => x.Tick(
            It.Is<string>(b => b == expected)));
    }

    [TestMethod]
    [DataRow(MutantStatus.Killed, "│ Testing mutant 1 / 100000000 │ K 1 │ S 0 │ T 0 │ E 0 │ ~11d 13h │")]
    [DataRow(MutantStatus.Survived, "│ Testing mutant 1 / 100000000 │ K 0 │ S 1 │ T 0 │ E 0 │ ~11d 13h │")]
    [DataRow(MutantStatus.Timeout, "│ Testing mutant 1 / 100000000 │ K 0 │ S 0 │ T 1 │ E 0 │ ~11d 13h │")]
    [DataRow(MutantStatus.RuntimeError, "│ Testing mutant 1 / 100000000 │ K 0 │ S 0 │ T 0 │ E 1 │ ~11d 13h │")]
    public void ReportRunTest_TestExecutionTimeInDays(MutantStatus status, string expected)
    {
        var progressBarMock = new Mock<IProgressBar>(MockBehavior.Strict);
        progressBarMock.Setup(x => x.Start(It.IsAny<int>(), It.IsAny<string>()));
        progressBarMock.Setup(x => x.Tick(It.IsAny<string>()));

        var progressBarReporter = new ProgressBarReporter(progressBarMock.Object, new FixedClock());

        var mutantTestResult = new Mutant()
        {
            ResultStatus = status
        };

        progressBarReporter.ReportInitialState(100000000);
        progressBarReporter.ReportRunTest(mutantTestResult);

        progressBarMock.Verify(x => x.Tick(
            It.Is<string>(b => b == expected)
        ));
    }


    [TestMethod]
    public void ProgressBarSmokeCheck()
    {
        var progress = new ProgressBar();
        progress.Start(0, "test");
        progress.Tick("next");
        progress.Ticks().ShouldBe(1);
        progress.Stop();

        progress.Dispose();
        progress.Ticks().ShouldBe(-1);
    }

    [TestMethod]
    public void CoverageTestsProcessed_ShouldBeZero_Initially()
    {
        var progressBarMock = new Mock<IProgressBar>(MockBehavior.Strict);
        progressBarMock.Setup(x => x.Start(It.IsAny<int>(), It.IsAny<string>()));

        var progressBarReporter = new ProgressBarReporter(progressBarMock.Object, new FixedClock());

        progressBarReporter.CoverageTestsProcessed.ShouldBe(0);
    }

    [TestMethod]
    public void ReportCoverageAnalysisProgress_ShouldUpdateCounter()
    {
        var progressBarMock = new Mock<IProgressBar>(MockBehavior.Strict);
        progressBarMock.Setup(x => x.Start(It.IsAny<int>(), It.IsAny<string>()));

        var progressBarReporter = new ProgressBarReporter(progressBarMock.Object, new FixedClock());

        progressBarReporter.ReportCoverageAnalysisStarted(100);
        progressBarReporter.CoverageTestsProcessed.ShouldBe(0);

        progressBarReporter.ReportCoverageAnalysisProgress(50);
        progressBarReporter.CoverageTestsProcessed.ShouldBe(50);

        progressBarReporter.ReportCoverageAnalysisCompleted();
    }

    [TestMethod]
    public void ReportCoverageAnalysisProgress_ShouldDoNothing_WhenNotStarted()
    {
        var progressBarMock = new Mock<IProgressBar>(MockBehavior.Strict);
        progressBarMock.Setup(x => x.Start(It.IsAny<int>(), It.IsAny<string>()));

        var progressBarReporter = new ProgressBarReporter(progressBarMock.Object, new FixedClock());

        progressBarReporter.ReportCoverageAnalysisProgress(50);
        progressBarReporter.CoverageTestsProcessed.ShouldBe(0);

        progressBarReporter.ReportCoverageAnalysisCompleted();
    }

    [TestMethod]
    public void ReportCoverageAnalysisCompleted_ShouldShowFinalProgress()
    {
        var progressBarMock = new Mock<IProgressBar>(MockBehavior.Strict);
        progressBarMock.Setup(x => x.Start(It.IsAny<int>(), It.IsAny<string>()));

        var progressBarReporter = new ProgressBarReporter(progressBarMock.Object, new FixedClock());

        progressBarReporter.ReportCoverageAnalysisStarted(200);
        progressBarReporter.ReportCoverageAnalysisProgress(150);

        progressBarReporter.CoverageTestsProcessed.ShouldBe(150);
        progressBarReporter.ReportCoverageAnalysisCompleted();
    }
}

public class FixedClock : TestBase, IStopWatchProvider
{
    public void Start()
    {
    }

    public void Stop()
    {
    }

    public long GetElapsedMillisecond() => 10L;
}
