﻿using Stryker.Core.Mutants;

namespace Stryker.Core.Reporters.Progress
{
    public interface IMutantsResultReporter
    {
        void ReportInitialState();
        void ReportMutantTestResult(IReadOnlyMutant mutantTestResult);
    }

    public class MutantsResultReporter : IMutantsResultReporter
    {
        private readonly IConsoleOneLineLogger _mutantKilledLogger;
        private readonly IConsoleOneLineLogger _mutantSurvivedLogger;
        private readonly IConsoleOneLineLogger _mutantTimeoutLogger;

        private int _mutantsKilledCount;
        private int _mutantsSurvivedCount;
        private int _mutantsTimeoutCount;

        public MutantsResultReporter(
            IConsoleOneLineLogger mutantKilledLogger,
            IConsoleOneLineLogger mutantSurvivedLogger,
            IConsoleOneLineLogger mutantTimeoutLogger)
        {
            _mutantKilledLogger = mutantKilledLogger;
            _mutantSurvivedLogger = mutantSurvivedLogger;
            _mutantTimeoutLogger = mutantTimeoutLogger;
        }

        public void ReportInitialState()
        {
            _mutantKilledLogger.StartLog("Killed:   {0}", 0);
            _mutantSurvivedLogger.StartLog("Survived: {0}", 0);
            _mutantTimeoutLogger.StartLog("Timeout:  {0}", 0);
        }

        public void ReportMutantTestResult(IReadOnlyMutant mutantTestResult)
        {
            switch (mutantTestResult.ResultStatus)
            {
                case MutantStatus.Killed:
                    _mutantsKilledCount++;
                    _mutantKilledLogger.ReplaceLog("Killed:   {0}", _mutantsKilledCount);
                    break;
                case MutantStatus.Survived:
                    _mutantsSurvivedCount++;
                    _mutantSurvivedLogger.ReplaceLog("Survived: {0}", _mutantsSurvivedCount);
                    break;
                case MutantStatus.Timeout:
                    _mutantsTimeoutCount++;
                    _mutantTimeoutLogger.ReplaceLog("Timeout:  {0}", _mutantsTimeoutCount);
                    break;
            };
        }
    }
}