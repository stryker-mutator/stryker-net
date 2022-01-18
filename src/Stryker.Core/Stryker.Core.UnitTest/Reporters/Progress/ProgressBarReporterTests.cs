using Moq;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters.Progress;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters.Progress
{
    public class FixedClock : TestBase, IStopWatchProvider
    {
        public void Start()
        {
        }

        public void Stop()
        {
        }

        public long GetElapsedMillisecond()
        {
            return 10L;
        }
    }

    public class ProgressBarReporterTests
    {
        [Fact]
        public void ReportInitialState_ShouldReportTestProgressAs0PercentageDone_WhenTotalNumberOfTestsIsTwo()
        {
            var progressBarMock = new Mock<IProgressBar>(MockBehavior.Strict);
            progressBarMock.Setup(x => x.Start(It.IsAny<int>(), It.IsAny<string>()));

            var progressBarReporter = new ProgressBarReporter(progressBarMock.Object, new FixedClock());

            progressBarReporter.ReportInitialState(3);

            progressBarMock.Verify(x => x.Start(
                It.Is<int>(a => a == 3),
                It.Is<string>(b => b == "│ Testing mutant 0 / 3 │ K 0 │ S 0 │ T 0 │ NA │")
            ));
        }

        [Theory]
        [InlineData(MutantStatus.Killed)]
        [InlineData(MutantStatus.Survived)]
        [InlineData(MutantStatus.Timeout)]
        public void ReportRunTest_ShouldReportTestProgressAs50PercentageDone_And_FirstTestExecutionTime_WhenHalfOfTestsAreDone(MutantStatus status)
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

            string expected = string.Empty;
            switch (status)
            {
                case MutantStatus.Killed:
                    expected = "│ Testing mutant 1 / 2 │ K 1 │ S 0 │ T 0 │ ~0m 00s │";
                    break;
                case MutantStatus.Survived:
                    expected = "│ Testing mutant 1 / 2 │ K 0 │ S 1 │ T 0 │ ~0m 00s │";
                    break;
                case MutantStatus.Timeout:
                    expected = "│ Testing mutant 1 / 2 │ K 0 │ S 0 │ T 1 │ ~0m 00s │";
                    break;
            }

            progressBarMock.Verify(x => x.Tick(
                It.Is<string>(b => b == expected)
            ));
        }

        [Theory]
        [InlineData(MutantStatus.Killed)]
        [InlineData(MutantStatus.Survived)]
        [InlineData(MutantStatus.Timeout)]
        public void ReportRunTest_TestExecutionTimeInMinutes(MutantStatus status)
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

            string expected = string.Empty;
            switch (status)
            {
                case MutantStatus.Killed:
                    expected = "│ Testing mutant 1 / 10000 │ K 1 │ S 0 │ T 0 │ ~1m 39s │";
                    break;
                case MutantStatus.Survived:
                    expected = "│ Testing mutant 1 / 10000 │ K 0 │ S 1 │ T 0 │ ~1m 39s │";
                    break;
                case MutantStatus.Timeout:
                    expected = "│ Testing mutant 1 / 10000 │ K 0 │ S 0 │ T 1 │ ~1m 39s │";
                    break;
            }

            progressBarMock.Verify(x => x.Tick(
                It.Is<string>(b => b == expected)
            ));
        }

        [Theory]
        [InlineData(MutantStatus.Killed)]
        [InlineData(MutantStatus.Survived)]
        [InlineData(MutantStatus.Timeout)]
        public void ReportRunTest_TestExecutionTimeInHours(MutantStatus status)
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

            string expected = string.Empty;
            switch (status)
            {
                case MutantStatus.Killed:
                    expected = "│ Testing mutant 1 / 1000000 │ K 1 │ S 0 │ T 0 │ ~2h 46m │";
                    break;
                case MutantStatus.Survived:
                    expected = "│ Testing mutant 1 / 1000000 │ K 0 │ S 1 │ T 0 │ ~2h 46m │";
                    break;
                case MutantStatus.Timeout:
                    expected = "│ Testing mutant 1 / 1000000 │ K 0 │ S 0 │ T 1 │ ~2h 46m │";
                    break;
            }
        }

        [Theory]
        [InlineData(MutantStatus.Killed)]
        [InlineData(MutantStatus.Survived)]
        [InlineData(MutantStatus.Timeout)]
        public void ReportRunTest_TestExecutionTimeInDays(MutantStatus status)
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

            string expected = string.Empty;
            switch (status)
            {
                case MutantStatus.Killed:
                    expected = "│ Testing mutant 1 / 100000000 │ K 1 │ S 0 │ T 0 │ ~11d 13h │";
                    break;
                case MutantStatus.Survived:
                    expected = "│ Testing mutant 1 / 100000000 │ K 0 │ S 1 │ T 0 │ ~11d 13h │";
                    break;
                case MutantStatus.Timeout:
                    expected = "│ Testing mutant 1 / 100000000 │ K 0 │ S 0 │ T 1 │ ~11d 13h │";
                    break;
            }
        }


        [Fact]
        public void ProgressBarSmokeCheck()
        {
            var progress = new ProgressBar();
            progress.Start(0, "test");
            progress.Tick("next");
            progress.Stop();

            progress.Dispose();
        }

        private void VerifyProgress(string progressBar, int tested, int total, int percentage, string estimate)
        {
            if (tested > 0)
            {
                //_testsProgressLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(),
               // It.Is<object[]>(loggerParams => loggerParams.SequenceEqual(new object[] { progressBar, tested, total, percentage, estimate }))));
            } else
            {
                //_testsProgressLogger.Verify(x => x.StartLog(It.IsAny<string>(),
                //It.Is<object[]>(loggerParams => loggerParams.SequenceEqual(new object[] { progressBar, tested, total, percentage, estimate }))));
            }

        }
    }
}
