using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Reporters.Progress
{
    public class ProgressReporter : IReporter
    {
        private readonly IProgressBarReporter _progressBarReporter;
        public ProgressReporter(IProgressBarReporter progressBarReporter)
        {
            _progressBarReporter = progressBarReporter;
        }

        public void OnMutantsCreated(IReadOnlyInputComponent reportComponent)
        {
        }

        public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested, IEnumerable<TestDescription> testDescriptions)
        {
            _progressBarReporter.ReportInitialState(mutantsToBeTested.Count());
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
            _progressBarReporter.ReportRunTest(result);
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent reportComponent)
        {
            _progressBarReporter.ReportFinalState();
        }
    }
}