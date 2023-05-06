using System.Collections.Generic;
using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;

namespace Stryker.Core.Reporters;

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
