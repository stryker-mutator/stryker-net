using Stryker.Shared.Tests;

namespace Stryker.Shared.Mutants;

public interface IMutant : IReadOnlyMutant
{
    new int Id { get; set; }

    new IMutation Mutation { get; set; }

    new MutantStatus ResultStatus { get; set; }

    new string ResultStatusReason { get; set; }

    new ITestGuids CoveringTests { get; set; }

    new ITestGuids KillingTests { get; set; }

    new ITestGuids AssessingTests { get; set; }

    new bool IsStaticValue { get; set; }

    bool MustBeTestedInIsolation { get; set; }

    string DisplayName { get; }

    void AnalyzeTestRun(ITestGuids failedTests, ITestGuids resultRanTests, ITestGuids timedOutTests, bool sessionTimedOut);
}
