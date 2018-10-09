using System;
using System.Linq;
using Moq;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters.Progress;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters.Progress
{
    public class ProgressBarReporterTests
    {
        private readonly Mock<IConsoleOneLineLogger> _testsProgressLogger;
        private readonly ProgressBarReporter _progressBarReporter;
        public ProgressBarReporterTests()
        {
            _testsProgressLogger = new Mock<IConsoleOneLineLogger>();
            _progressBarReporter = new ProgressBarReporter(_testsProgressLogger.Object);
        }

        [Fact]
        public void ReportRunTest_ShouldReportTestProgressAs100PercentageDone_WhenTotalNumberOfTestsIsZero()
        {
            _progressBarReporter.ReportInitialState(0);

            _progressBarReporter.ReportRunTest(TimeSpan.FromSeconds(10));

            _testsProgressLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(),
                                                          It.Is<object[]>(
                                                              y => (int)y.ElementAt(1) == 0 &&
                                                                   (int)y.ElementAt(2) == 0 &&
                                                                   (int)y.ElementAt(3) == 100 &&
                                                                   ((string)y.ElementAt(4)).Contains("0 s"))));
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
                                                                   ((string)y.ElementAt(4)).Contains("0 s"))));
        }

        [Fact]
        public void ReportRunTest_ShouldReportTestProgressAs50PercentageDone_And_FirstTestExecutionTime_WhenHalfOfTestsAreDone()
        {
            _progressBarReporter.ReportInitialState(2);

            _progressBarReporter.ReportRunTest(TimeSpan.FromSeconds(10));

            _testsProgressLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(),
                                                          It.Is<object[]>(
                                                              y => ((string)y.ElementAt(0)).EndsWith("-----") &&
                                                                   (int)y.ElementAt(1) == 1 &&
                                                                   (int)y.ElementAt(2) == 2 &&
                                                                   (int)y.ElementAt(3) == 50 &&
                                                                   (string)y.ElementAt(4) == "10 s")));
        }
    }
}