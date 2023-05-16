using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;

namespace Stryker.Core.Reporters
{
    /// <summary>
    /// Broadcasts the report calls to all reporters in the list
    /// The order of the reporters is important, as the reporter invokes them in order
    /// </summary>
    public class BroadcastReporter : IReporter
    {
        private readonly object _mutex = new();
        public IEnumerable<IReporter> Reporters { get; }

        public BroadcastReporter(IEnumerable<IReporter> reporters) => Reporters = reporters;

        public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent, TestProjectsInfo testProjectsInfo)
        {
            foreach (var reporter in Reporters)
            {
                reporter.OnMutantsCreated(reportComponent, testProjectsInfo);
            }
            // todo: refactor to lifecycle event
            if ((reportComponent.Mutants ?? Enumerable.Empty<IReadOnlyMutant>()).Any())
            {
                new FilteredMutantsLogger().OnMutantsCreated(reportComponent);
            }
        }

        public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested)
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

        public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent, TestProjectsInfo testProjectsInfo)
        {
            // make sure all other console caches are flushed before writing final reports
            Thread.Sleep(TimeSpan.FromSeconds(1));

            foreach (var reporter in Reporters)
            {
                reporter.OnAllMutantsTested(reportComponent, testProjectsInfo);
            }
        }
    }
}
