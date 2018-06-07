using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using Stryker.Core.Testing;
using System;

namespace Stryker.Core.Reporters
{
    /// <summary>
    /// The default reporter, prints a simple progress and end result.
    /// </summary>
    public class ConsoleStatusReporter : IReporter
    {
        private IChalk _chalk { get; set; }

        public ConsoleStatusReporter(IChalk chalk = null)
        {
            _chalk = chalk ?? new Chalk();
        }

        public void OnMutantsCreated(IReadOnlyInputComponent inputComponent) { }

        public void OnMutantTested(IReadOnlyMutant result) { }

        public void OnAllMutantsTested(IReadOnlyInputComponent inputComponent) { }

        private void DisplayComponent(IReadOnlyInputComponent inputComponent) { }

        public void OnInitialisationStarted()
        {
            _chalk.Default($"Analyzing project{Environment.NewLine}");
        }

        public void OnInitialBuildStarted()
        {
            _chalk.Default($"Building project{Environment.NewLine}");
        }

        public void OnInitialTestRunStarted()
        {
            _chalk.Default($"Starting initial testrun{Environment.NewLine}");
        }

        public void OnInitialisationDone()
        {
            _chalk.Default($"Project OK{Environment.NewLine}Generating mutants{Environment.NewLine}");
        }
    }
}
