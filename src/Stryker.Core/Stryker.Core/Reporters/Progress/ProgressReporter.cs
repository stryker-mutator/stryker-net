using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents;
using System;
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

        public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent)
        {
        }

        public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested, IEnumerable<TestDescription> testDescriptions)
        {
            Console.WriteLine();
            _progressBarReporter.ReportInitialState(mutantsToBeTested.Count());
            _mutantsResultReporter.ReportInitialState();
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
            _progressBarReporter.ReportRunTest();
            _mutantsResultReporter.ReportMutantTestResult(result);
        }

        public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent)
        {
        }
    }
}