using Moq;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters.Progress;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters.Progress
{
    public class MutantsResultReporterTests
    {
        private readonly Mock<IConsoleOneLineLogger> _mutantKilledLogger;
        private readonly Mock<IConsoleOneLineLogger> _mutantSurvivedLogger;
        private readonly Mock<IConsoleOneLineLogger> _mutantTimeoutLogger;

        private readonly MutantsResultReporter _target;

        public MutantsResultReporterTests()
        {
            _mutantKilledLogger = new Mock<IConsoleOneLineLogger>();
            _mutantSurvivedLogger = new Mock<IConsoleOneLineLogger>();
            _mutantTimeoutLogger = new Mock<IConsoleOneLineLogger>();

            _target = new MutantsResultReporter(_mutantKilledLogger.Object,
                                                _mutantSurvivedLogger.Object,
                                                _mutantTimeoutLogger.Object);
        }

        [Theory]
        [InlineData(MutantStatus.Killed)]
        [InlineData(MutantStatus.Survived)]
        [InlineData(MutantStatus.Timeout)]
        public void ShouldLogEachTimeKilledMutantIsReported(MutantStatus status)
        {
            var mutantTestResult = new Mutant()
            {
                ResultStatus = status
            };

            for (int i = 0; i < 5; i++)
            {
                _target.ReportMutantTestResult(mutantTestResult);

                // Verify the right oneline logger is called for each status
                switch (status)
                {
                    case MutantStatus.Killed:
                        _mutantKilledLogger.Verify(x => x.ReplaceLog("Killed:   {0}", It.Is<object[]>(y => y.Length == 1 && (int)y.First() == i + 1)));
                        break;
                    case MutantStatus.Survived:
                        _mutantSurvivedLogger.Verify(x => x.ReplaceLog("Survived: {0}", It.Is<object[]>(y => y.Length == 1 && (int)y.First() == i + 1)));
                        break;
                    case MutantStatus.Timeout:
                        _mutantTimeoutLogger.Verify(x => x.ReplaceLog("Timeout:  {0}", It.Is<object[]>(y => y.Length == 1 && (int)y.First() == i + 1)));
                        break;
                }
            }
        }

        [Fact]
        public void ShouldLogInitialState()
        {
            _target.ReportInitialState();

            // Verify the right oneline logger is called for each status
            _mutantKilledLogger.Verify(x => x.StartLog("Killed:   {0}", It.Is<object[]>(y => y.Length == 1 && (int)y.First() == 0)));
            _mutantSurvivedLogger.Verify(x => x.StartLog("Survived: {0}", It.Is<object[]>(y => y.Length == 1 && (int)y.First() == 0)));
            _mutantTimeoutLogger.Verify(x => x.StartLog("Timeout:  {0}", It.Is<object[]>(y => y.Length == 1 && (int)y.First() == 0)));
        }
    }
}