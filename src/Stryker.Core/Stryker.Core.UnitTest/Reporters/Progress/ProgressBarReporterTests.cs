using Moq;
using Stryker.Core.Reporters.Progress;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters.Progress
{
    public class FixedClock : IStopWatchProvider
    {
        public void Start()
        {
        }

        public long GetElapsedMillisecond()
        {
            return 10L;
        }
    }
    
    public class ProgressBarReporterTests
    {
        private readonly Mock<IConsoleOneLineLogger> _testsProgressLogger;
        private readonly ProgressBarReporter _progressBarReporter;

        public ProgressBarReporterTests()
        {
            _testsProgressLogger = new Mock<IConsoleOneLineLogger>();
            _progressBarReporter = new ProgressBarReporter(_testsProgressLogger.Object, new FixedClock());
        }

        [Fact]
        public void ReportInitialState_ShouldReportTestProgressAs0PercentageDone_WhenTotalNumberOfTestsIsTwo()
        {
            _progressBarReporter.ReportInitialState(3);

            VerifyProgress(progressBar: "----------",
                tested: 0,
                total: 3,
                percentage: 0,
                estimate: "NA");
        }

        [Fact]
        public void ReportRunTest_ShouldReportTestProgressAs50PercentageDone_And_FirstTestExecutionTime_WhenHalfOfTestsAreDone()
        {
            _progressBarReporter.ReportInitialState(2);

            _progressBarReporter.ReportRunTest();

            VerifyProgress(progressBar: "█████-----",
                tested: 1,
                total: 2,
                percentage: 50,
                estimate: "~0m 00s");
        }

        [Fact]
        public void ReportRunTest_TestExecutionTimeInMinutes()
        {
            _progressBarReporter.ReportInitialState(10000);
            _progressBarReporter.ReportRunTest();

            VerifyProgress(progressBar: "----------",
                tested: 1,
                total: 10000,
                percentage: 0,
                estimate: "~1m 39s");
        }

        [Fact]
        public void ReportRunTest_TestExecutionTimeInHours()
        {
            _progressBarReporter.ReportInitialState(1000000);
            _progressBarReporter.ReportRunTest();

            VerifyProgress(progressBar: "----------",
                tested: 1,
                total: 1000000,
                percentage: 0,
                estimate: "~2h 46m");
        }

        [Fact]
        public void ReportRunTest_TestExecutionTimeInDays()
        {
            _progressBarReporter.ReportInitialState(100000000);

            _progressBarReporter.ReportRunTest();

            VerifyProgress(progressBar: "----------",
                tested: 1,
                total: 100000000,
                percentage: 0,
                estimate: "~11d 13h");
        }

        private void VerifyProgress(string progressBar, int tested, int total, int percentage, string estimate)
        {
            if (tested > 0)
            {
                _testsProgressLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(),
                It.Is<object[]>(loggerParams => loggerParams.SequenceEqual(new object[] { progressBar, tested, total, percentage, estimate }))));
            } else
            {
                _testsProgressLogger.Verify(x => x.StartLog(It.IsAny<string>(),
                It.Is<object[]>(loggerParams => loggerParams.SequenceEqual(new object[] { progressBar, tested, total, percentage, estimate }))));
            }
            
        }
    }
}