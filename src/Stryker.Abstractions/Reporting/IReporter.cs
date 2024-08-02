using Stryker.Configuration.ProjectComponents;
using Stryker.Configuration.ProjectComponents.TestProjects;

namespace Stryker.Configuration.Reporting;
{
    public interface IReporter
    {
        // Will get called when the project has been mutated
        void OnMutantsCreated(IReadOnlyProjectComponent reportComponent, TestProjectsInfo testProjectsInfo);
        // Will get called on start before first mutation is tested
        void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested);
        // Will get called when a mutation has been tested
        void OnMutantTested(IReadOnlyMutant result);
        // Will get called when all mutations have been tested
        void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent, TestProjectsInfo testProjectsInfo);
    }
}
