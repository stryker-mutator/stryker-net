using Moq;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters.Progress;
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
            var mutants = new Mutant[3] { new Mutant(), new Mutant(), new Mutant() };

            _progressReporter.OnStartMutantTestRun(mutants);
            _mutantsResultReporter.Verify(x => x.ReportInitialState(), Times.Once);
            _progressBarReporter.Verify(x => x.ReportInitialState(mutants.Length), Times.Once);
        }

        [Fact]
        public void ProgressReporter_ShouldCallBothReporters_OnReportRunTest()
        {
            var mutant = new Mutant();
            _progressReporter.OnMutantTested(mutant);

            _mutantsResultReporter.Verify(x => x.ReportMutantTestResult(mutant), Times.Once);
            _progressBarReporter.Verify(x => x.ReportRunTest(), Times.Once);
        }
    }
}