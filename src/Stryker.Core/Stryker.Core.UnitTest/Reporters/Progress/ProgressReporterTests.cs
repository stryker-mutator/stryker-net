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
        private readonly Mock<IMutantsResultReporter> _mutantsResultReporter;
        private readonly Mock<IProgressBarReporter> _progressBarReporter;

        private readonly ProgressReporter _progressReporter;

        public ProgressReporterTests()
        {
            _mutantsResultReporter = new Mock<IMutantsResultReporter>();
            _progressBarReporter = new Mock<IProgressBarReporter>();

            _progressReporter = new ProgressReporter(_mutantsResultReporter.Object, _progressBarReporter.Object);
        }
        
        [Fact]
        public void ProgressReporter_ShouldCallBothReporters_OnReportInitialState()
        {
            var totalNumberOfTests = 10;

            _progressReporter.ReportInitialState(totalNumberOfTests);
            _mutantsResultReporter.Verify(x => x.ReportInitialState(), Times.Once);
            _progressBarReporter.Verify(x => x.ReportInitialState(totalNumberOfTests), Times.Once);
        }

        [Fact]
        public void ProgressReporter_ShouldCallBothReporters_OnReportRunTest()
        {
            var mutant = new Mutant();
            var testDuration = TimeSpan.FromSeconds(10);
            _progressReporter.ReportRunTest(testDuration, mutant);

            _mutantsResultReporter.Verify(x => x.ReportMutantTestResult(mutant), Times.Once);
            _progressBarReporter.Verify(x => x.ReportRunTest(testDuration), Times.Once);
        }
    }
}