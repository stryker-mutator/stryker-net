using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Reporters.Progress
{
    public class ProgressReporter : IReporter
    {
        private readonly IMutantsResultReporter _mutantsResultReporter;
        private readonly IProgressBarReporter _progressBarReporter;
        public ProgressReporter(IMutantsResultReporter mutantsResultReporter, IProgressBarReporter progressBarReporter)
        {
            _mutantsResultReporter = mutantsResultReporter;
            _progressBarReporter = progressBarReporter;
        }

        public void OnMutantsCreated(IReadOnlyInputComponent reportComponent)
        {
        }

        public void OnStartMutantTestRun(IEnumerable<Mutant> mutantsToBeTested)
        {
            _progressBarReporter.ReportInitialState(mutantsToBeTested.Count());
            _mutantsResultReporter.ReportInitialState();
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
            _progressBarReporter.ReportRunTest();
            _mutantsResultReporter.ReportMutantTestResult(result);
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent reportComponent)
        {
        }
    }
}