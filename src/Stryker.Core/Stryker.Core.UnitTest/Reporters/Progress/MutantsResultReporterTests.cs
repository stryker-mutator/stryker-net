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
                                                     _mutantTimeoutLogger.Object,
                                                     _mutantRuntimeErrorLogger.Object);
        }

        [Fact]
        public void ReportMutantTestResult_ShouldLogEachTimeKilledMutantIsReported()
        {
            var mutantTestResult = new Mutant()
            {
                ResultStatus = MutantStatus.Killed
            };

            for (int i = 0; i < 5; i++)
            {
                _mutantsResultReporter.ReportMutantTestResult(mutantTestResult);
                _mutantKilledLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(), It.Is<object[]>(y => y.Length == 1 && (int)y.First() == i + 1)));
            }
        }

        [Fact]
        public void ReportMutantTestResult_ShouldLogEachTimeSurvivedMutantIsReported()
        {
            var mutantTestResult = new Mutant()
            {
                ResultStatus = MutantStatus.Survived
            };

            for (int i = 0; i < 5; i++)
            {
                _mutantsResultReporter.ReportMutantTestResult(mutantTestResult);
                _mutantSurvivedLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(), It.Is<object[]>(y => y.Length == 1 && (int)y.First() == i + 1)));
            }
        }

        [Fact]
        public void ReportMutantTestResult_ShouldLogEachTimeTimeoutMutantIsReported()
        {
            var mutantTestResult = new Mutant()
            {
                ResultStatus = MutantStatus.Timeout
            };

            for (int i = 0; i < 5; i++)
            {
                _mutantsResultReporter.ReportMutantTestResult(mutantTestResult);
                _mutantTimeoutLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(), It.Is<object[]>(y => y.Length == 1 && (int)y.First() == i + 1)));
            }
        }

        [Fact]
        public void ReportMutantTestResult_ShouldLogEachTimeRuntimeErrorMutantIsReported()
        {
            var mutantTestResult = new Mutant()
            {
                ResultStatus = MutantStatus.RuntimeError
            };

            for (int i = 0; i < 5; i++)
            {
                _mutantsResultReporter.ReportMutantTestResult(mutantTestResult);
                _mutantRuntimeErrorLogger.Verify(x => x.ReplaceLog(It.IsAny<string>(), It.Is<object[]>(y => y.Length == 1 && (int)y.First() == i + 1)));
            }
        }
    }
}