using Moq;
using Shouldly;
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

        public long GetElapsedMillisecond() => 10L;
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

        [Fact]
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

        [Theory]
        [InlineData(MutantStatus.Killed, "│ Testing mutant 1 / 2 │ K 1 │ S 0 │ T 0 │ ~0m 00s │")]
        [InlineData(MutantStatus.Survived, "│ Testing mutant 1 / 2 │ K 0 │ S 1 │ T 0 │ ~0m 00s │")]
        [InlineData(MutantStatus.Timeout, "│ Testing mutant 1 / 2 │ K 0 │ S 0 │ T 1 │ ~0m 00s │")]
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

        [Theory]
        [InlineData(MutantStatus.Killed, "│ Testing mutant 1 / 10000 │ K 1 │ S 0 │ T 0 │ ~1m 39s │")]
        [InlineData(MutantStatus.Survived, "│ Testing mutant 1 / 10000 │ K 0 │ S 1 │ T 0 │ ~1m 39s │")]
        [InlineData(MutantStatus.Timeout, "│ Testing mutant 1 / 10000 │ K 0 │ S 0 │ T 1 │ ~1m 39s │")]
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

        [Theory]
        [InlineData(MutantStatus.Killed, "│ Testing mutant 1 / 1000000 │ K 1 │ S 0 │ T 0 │ ~2h 46m │")]
        [InlineData(MutantStatus.Survived, "│ Testing mutant 1 / 1000000 │ K 0 │ S 1 │ T 0 │ ~2h 46m │")]
        [InlineData(MutantStatus.Timeout, "│ Testing mutant 1 / 1000000 │ K 0 │ S 0 │ T 1 │ ~2h 46m │")]
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

        [Theory]
        [InlineData(MutantStatus.Killed, "│ Testing mutant 1 / 100000000 │ K 1 │ S 0 │ T 0 │ ~11d 13h │")]
        [InlineData(MutantStatus.Survived, "│ Testing mutant 1 / 100000000 │ K 0 │ S 1 │ T 0 │ ~11d 13h │")]
        [InlineData(MutantStatus.Timeout, "│ Testing mutant 1 / 100000000 │ K 0 │ S 0 │ T 1 │ ~11d 13h │")]
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


        [Fact]
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
    }
}
