using Moq;
using Stryker.Core.Reporters.Progress;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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
        public void ReportRunTest_ShouldReportTestProgressAs100PercentageDone_WhenTotalNumberOfTestsIsZero()
        {
            _progressBarReporter.ReportInitialState(0);

            _progressBarReporter.ReportRunTest();
            _testsProgressLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(),
                                                          It.Is<object[]>(
                                                              y => (int)y.ElementAt(1) == 0 &&
                                                                   (int)y.ElementAt(2) == 0 &&
                                                                   (int)y.ElementAt(3) == 100)));
        }

        [Fact]
        public void ReportInitialState_ShouldReportTestProgressAs0PercentageDone_WhenTotalNumberOfTestsIsTwo()
        {
            _progressBarReporter.ReportInitialState(2);

            _testsProgressLogger.Verify(x => x.StartLog(It.IsAny<string>(),
                                                          It.Is<object[]>(
                                                              y => (int)y.ElementAt(1) == 0 &&
                                                                   (int)y.ElementAt(2) == 2 &&
                                                                   (int)y.ElementAt(3) == 0 &&
                                                                   ((string)y.ElementAt(4)).Equals("NA"))));
        }

        [Fact]
        public void ReportRunTest_ShouldReportTestProgressAs50PercentageDone_And_FirstTestExecutionTime_WhenHalfOfTestsAreDone()
        {
            _progressBarReporter.ReportInitialState(2);

            _progressBarReporter.ReportRunTest();

            _testsProgressLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(),
                                                          It.Is<object[]>(
                                                              y => ((string)y.ElementAt(0)).EndsWith("-----") &&
                                                                   (int)y.ElementAt(1) == 1 &&
                                                                   (int)y.ElementAt(2) == 2 &&
                                                                   (int)y.ElementAt(3) == 50 &&
                                                                   ((string)y.ElementAt(4)).Equals("~0m 00s"))));
        }

        [Fact]
        public void ReportRunTest_ShouldReportTestProgressAs50PercentageDone_And_FirstTestExecutionTimeInMinutes()
        {
            _progressBarReporter.ReportInitialState(10000);
            _progressBarReporter.ReportRunTest();

            _testsProgressLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(),
                It.Is<object[]>(
                    y => ((string)y.ElementAt(0)).EndsWith("-----") &&
                         (int)y.ElementAt(1) == 1 &&
                         (int)y.ElementAt(2) == 10000 &&
                         (int)y.ElementAt(3) == 0 && 
                         ((string)y.ElementAt(4)).Equals("~1m 39s"))));
        }
        [Fact]
        public void ReportRunTest_ShouldReportTestProgressAs50PercentageDone_And_FirstTestExecutionTimeInHours()
        {
            _progressBarReporter.ReportInitialState(1000000);
            _progressBarReporter.ReportRunTest();

            _testsProgressLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(),
                It.Is<object[]>(
                    y => ((string)y.ElementAt(0)).EndsWith("-----") &&
                         (int)y.ElementAt(1) == 1 &&
                         (int)y.ElementAt(2) == 1000000 &&
                         (int)y.ElementAt(3) == 0 && 
                         ((string)y.ElementAt(4)).Equals("~2h 46m"))));
        }

        [Fact]
        public void ReportRunTest_ShouldReportTestProgressAs50PercentageDone_And_FirstTestExecutionTimeInDays()
        {
            _progressBarReporter.ReportInitialState(100000000);
            _progressBarReporter.ReportRunTest();

            _testsProgressLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(),
                It.Is<object[]>(
                    y => ((string)y.ElementAt(0)).EndsWith("-----") &&
                         (int)y.ElementAt(1) == 1 &&
                         (int)y.ElementAt(2) == 100000000 &&
                         (int)y.ElementAt(3) == 0 && 
                         ((string)y.ElementAt(4)).Equals("~11d 13h"))));
        }
    }
}