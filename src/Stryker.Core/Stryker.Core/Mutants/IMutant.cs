using Stryker.Configuration.Mutants;
using Stryker.Configuration.TestRunners;

namespace Stryker.Core.Mutants;
public interface IMutant
{
    ITestGuids AssessingTests { get; set; }
    bool CountForStats { get; }
    ITestGuids CoveringTests { get; set; }
    string DisplayName { get; }
    int Id { get; set; }
    bool IsStaticValue { get; set; }
    ITestGuids KillingTests { get; set; }
    bool MustBeTestedInIsolation { get; set; }
    Mutation Mutation { get; set; }
    MutantStatus ResultStatus { get; set; }
    string ResultStatusReason { get; set; }

    void AnalyzeTestRun(ITestGuids failedTests, ITestGuids resultRanTests, ITestGuids timedOutTests, bool sessionTimedOut);
}