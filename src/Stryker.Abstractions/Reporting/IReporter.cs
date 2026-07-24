using System.Collections.Generic;
using Stryker.Abstractions.ProjectComponents;

namespace Stryker.Abstractions.Reporting;

public interface IReporter
{
    // Will get called when the project has been mutated
    void OnMutantsCreated(IReadOnlyProjectComponent reportComponent, ITestProjectsInfo testProjectsInfo);
    // Will get called on start before first mutation is tested
    void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested);
    // Will get called when a mutation has been tested
    void OnMutantTested(IReadOnlyMutant result);
    // Will get called when all mutations have been tested
    void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent, ITestProjectsInfo testProjectsInfo);
    // Will get called when coverage analysis starts, before the tests are run
    void OnCoverageAnalysisStarted(int totalTests);
    // Will get called as tests are run to capture coverage
    void OnCoverageAnalysisProgress(int testsCompleted, int totalTests);
    // Will get called when coverage analysis is complete
    void OnCoverageAnalysisCompleted();
}
