﻿using Stryker.Core.Initialisation.ProjectComponent;
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
        private readonly object _mutex = new object();
        public IEnumerable<IReporter> Reporters { get; }

        public BroadcastReporter(IEnumerable<IReporter> reporters)
        {
            Reporters = reporters;
        }

        public void OnMutantsCreated(IReadOnlyInputComponent inputComponent)
        {
            foreach (var reporter in Reporters)
            {
                reporter.OnMutantsCreated(inputComponent);
            }
        }

        public void OnStartMutantTestRun(IEnumerable<Mutant> mutantsToBeTested)
        {
            foreach (var reporter in Reporters)
            {
                reporter.OnStartMutantTestRun(mutantsToBeTested);
            }
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
            lock (_mutex)
            {
                foreach (var reporter in Reporters)
                {
                    reporter.OnMutantTested(result);
                }
            }
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent inputComponent)
        {
            foreach (var reporter in Reporters)
            {
                reporter.OnAllMutantsTested(inputComponent);
            }
        }
    }
}
