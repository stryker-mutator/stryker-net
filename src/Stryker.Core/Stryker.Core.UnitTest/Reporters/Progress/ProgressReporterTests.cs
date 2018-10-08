using Moq;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters.Progress;
using System;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters.Progress
{
    public class ProgressReporterTests
    {
        private readonly Mock<IConsoleOneLineLogger> _testsProgressLogger;
        private readonly Mock<IConsoleOneLineLogger> _mutantKilledLogger;
        private readonly Mock<IConsoleOneLineLogger> _mutantSurvivedLogger;
        private readonly Mock<IConsoleOneLineLogger> _mutantTimeoutLogger;
        private readonly Mock<IConsoleOneLineLogger> _mutantRuntimeErrorLogger;

        private readonly ProgressReporter _progressReporter;

        public ProgressReporterTests()
        {
            _testsProgressLogger = new Mock<IConsoleOneLineLogger>();
            _mutantKilledLogger = new Mock<IConsoleOneLineLogger>();
            _mutantSurvivedLogger = new Mock<IConsoleOneLineLogger>();
            _mutantTimeoutLogger = new Mock<IConsoleOneLineLogger>();
            _mutantRuntimeErrorLogger = new Mock<IConsoleOneLineLogger>();

            _progressReporter = new ProgressReporter(_testsProgressLogger.Object,
                                                     _mutantKilledLogger.Object,
                                                     _mutantSurvivedLogger.Object,
                                                     _mutantTimeoutLogger.Object,
                                                     _mutantRuntimeErrorLogger.Object);
        }

        [Fact]
        public void ProgressReporter_ShouldLogEachTimeKilledMutantIsReported()
        {
            var mutantTestResult = new Mutant()
            {
                ResultStatus = MutantStatus.Killed
            };

            for (int i = 0; i < 5; i++)
            {
                _progressReporter.ReportRunTest(TimeSpan.MaxValue, mutantTestResult);
                _mutantKilledLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(), It.Is<object[]>(y => y.Length == 1 && (int)y.First() == i + 1)));
            }
        }

        [Fact]
        public void ProgressReporter_ShouldLogEachTimeSurvivedMutantIsReported()
        {
            var mutantTestResult = new Mutant()
            {
                ResultStatus = MutantStatus.Survived
            };

            for (int i = 0; i < 5; i++)
            {
                _progressReporter.ReportRunTest(TimeSpan.MaxValue, mutantTestResult);
                _mutantSurvivedLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(), It.Is<object[]>(y => y.Length == 1 && (int)y.First() == i + 1)));
            }
        }

        [Fact]
        public void ProgressReporter_ShouldLogEachTimeTimeoutMutantIsReported()
        {
            var mutantTestResult = new Mutant()
            {
                ResultStatus = MutantStatus.Timeout
            };

            for (int i = 0; i < 5; i++)
            {
                _progressReporter.ReportRunTest(TimeSpan.MaxValue, mutantTestResult);
                _mutantTimeoutLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(), It.Is<object[]>(y => y.Length == 1 && (int)y.First() == i + 1)));
            }
        }

        [Fact]
        public void ProgressReporter_ShouldLogEachTimeRuntimeErrorMutantIsReported()
        {
            var mutantTestResult = new Mutant()
            {
                ResultStatus = MutantStatus.RuntimeError
            };

            for (int i = 0; i < 5; i++)
            {
                _progressReporter.ReportRunTest(TimeSpan.MaxValue, mutantTestResult);
                _mutantRuntimeErrorLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(), It.Is<object[]>(y => y.Length == 1 && (int)y.First() == i + 1)));
            }
        }

        [Fact]
        public void ProgressReporter_ShouldReportTestProgressAs100PercentageDone_WhenTotalNumberOfTestsIsZero()
        {
            _progressReporter.StartReporting(0);

            _progressReporter.ReportRunTest(TimeSpan.FromSeconds(10), new Mutant());
            _testsProgressLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(),
                                                          It.Is<object[]>(
                                                              y => (int)y.ElementAt(1) == 0 &&
                                                                   (int)y.ElementAt(2) == 0 &&
                                                                   (int)y.ElementAt(3) == 100 &&
                                                                   ((string)y.ElementAt(4)).Contains("0 s"))));
        }

        [Fact]
        public void ProgressReporter_ShouldReportTestProgressAs50PercentageDone_And_FirstTestExecutionTime_WhenHalfOfTestsAreDone()
        {
            _progressReporter.StartReporting(2);

            _progressReporter.ReportRunTest(TimeSpan.FromSeconds(10), new Mutant());

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