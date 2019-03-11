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
        private readonly Mock<IConsoleOneLineLogger> _mutantRuntimeErrorLogger;

        private readonly MutantsResultReporter _mutantsResultReporter;

        public MutantsResultReporterTests()
        {
            _mutantKilledLogger = new Mock<IConsoleOneLineLogger>();
            _mutantSurvivedLogger = new Mock<IConsoleOneLineLogger>();
            _mutantTimeoutLogger = new Mock<IConsoleOneLineLogger>();
            _mutantRuntimeErrorLogger = new Mock<IConsoleOneLineLogger>();

            _mutantsResultReporter = new MutantsResultReporter(_mutantKilledLogger.Object,
                                                     _mutantSurvivedLogger.Object,
                                                     _mutantTimeoutLogger.Object);
        }

        [Theory]
        [InlineData(MutantStatus.Killed)]
        [InlineData(MutantStatus.Survived)]
        [InlineData(MutantStatus.Timeout)]
        public void ReportMutantTestResult_ShouldLogEachTimeKilledMutantIsReported(MutantStatus status)
        {
            var mutantTestResult = new Mutant()
            {
                ResultStatus = status
            };

            for (int i = 0; i < 5; i++)
            {
                _mutantsResultReporter.ReportMutantTestResult(mutantTestResult);

                // Verify the right oneline logger is called for each status
                switch (status)
                {
                    case MutantStatus.Killed:
                        _mutantKilledLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(), It.Is<object[]>(y => y.Length == 1 && (int)y.First() == i + 1)));
                        break;
                    case MutantStatus.Survived:
                        _mutantSurvivedLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(), It.Is<object[]>(y => y.Length == 1 && (int)y.First() == i + 1)));
                        break;
                    case MutantStatus.Timeout:
                        _mutantTimeoutLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(), It.Is<object[]>(y => y.Length == 1 && (int)y.First() == i + 1)));
                        break;
                }
            }
        }
    }
}