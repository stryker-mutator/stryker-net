using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Reporters
{
    /// <summary>
    /// Broadcasts the report calls to all reporters in the list
    /// The order of the reporters is important, as the reporter invokes them in order
    /// </summary>
    public class BroadcastReporter : IReporter
    {
        private IEnumerable<IReporter> _reporters { get; set; }

        public BroadcastReporter(IEnumerable<IReporter> reporters)
        {
            _reporters = reporters;
        }

        public void OnMutantsCreated(IReadOnlyInputComponent inputComponent)
        {
            foreach(var reporter in _reporters)
            {
                reporter.OnMutantsCreated(inputComponent);
            }
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
            foreach (var reporter in _reporters)
            {
                reporter.OnMutantTested(result);
            }
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent inputComponent)
        {
            foreach (var reporter in _reporters)
            {
                reporter.OnAllMutantsTested(inputComponent);
            }
        }


        public void OnInitialisationStarted()
        {
            foreach (var reporter in _reporters)
            {
                reporter.OnInitialisationStarted();
            }
        }

        public void OnInitialBuildStarted()
        {
            foreach (var reporter in _reporters)
            {
                reporter.OnInitialBuildStarted();
            }
        }

        public void OnInitialTestRunStarted()
        {
            foreach (var reporter in _reporters)
            {
                reporter.OnInitialTestRunStarted();
            }
        }

        public void OnInitialisationDone()
        {
            foreach (var reporter in _reporters)
            {
                reporter.OnInitialisationDone();
            }
        }
    }
}
