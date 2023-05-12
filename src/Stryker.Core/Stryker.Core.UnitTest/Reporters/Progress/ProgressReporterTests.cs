namespace Stryker.Core.UnitTest.Reporters.Progress;
using Moq;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters.Progress;
using Xunit;

public class ProgressReporterTests : TestBase
{
    private readonly Mock<IProgressBarReporter> _progressBarReporter;

    private readonly ProgressReporter _progressReporter;

    public ProgressReporterTests()
    {
        _progressBarReporter = new Mock<IProgressBarReporter>();

        _progressReporter = new ProgressReporter(_progressBarReporter.Object);
    }

    [Fact]
    public void ProgressReporter_ShouldCallBothReporters_OnReportInitialState()
    {
        var mutants = new Mutant[3] { new Mutant(), new Mutant(), new Mutant() };

        _progressReporter.OnStartMutantTestRun(mutants);
        _progressBarReporter.Verify(x => x.ReportInitialState(mutants.Length), Times.Once);
    }

    [Fact]
    public void ProgressReporter_ShouldCallBothReporters_OnReportRunTest()
    {
        var mutant = new Mutant();
        _progressReporter.OnMutantTested(mutant);

        _progressBarReporter.Verify(x => x.ReportRunTest(mutant), Times.Once);
    }
}
