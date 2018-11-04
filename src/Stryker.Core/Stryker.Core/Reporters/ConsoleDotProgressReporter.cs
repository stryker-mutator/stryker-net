using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using Stryker.Core.Testing;
using System;
using System.Collections.Generic;

namespace Stryker.Core.Reporters
{
    /// <summary>
    /// The default reporter, prints a simple progress by printing dots
    /// </summary>
    public class ConsoleDotProgressReporter : IReporter
    {
        private IChalk _chalk { get; set; }

        public ConsoleDotProgressReporter(IChalk chalk = null)
        {
            _chalk = chalk ?? new Chalk();
        }

        public void OnMutantsCreated(IReadOnlyInputComponent inputComponent) { }
        public void OnStartMutantTestRun(IEnumerable<Mutant> mutantsToBeTested)
        {
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
            switch (result.ResultStatus) {
                case MutantStatus.Killed:
                    _chalk.Default(".");
                    break;
                case MutantStatus.Survived:
                    _chalk.Red("S");
                    break;
                case MutantStatus.Timeout:
                    _chalk.Default("T");
                    break;
            };
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent inputComponent) { }
    }
}
