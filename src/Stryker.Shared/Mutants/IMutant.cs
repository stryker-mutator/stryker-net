using Stryker.Shared.Tests;

namespace Stryker.Shared.Mutants;

public interface IMutant : IReadOnlyMutant
{
    new int Id { get; set; }

    new IMutation Mutation { get; set; }

    new MutantStatus ResultStatus { get; set; }

    new string ResultStatusReason { get; set; }

    new ITestIdentifiers CoveringTests { get; set; }

    new ITestIdentifiers KillingTests { get; set; }

    new ITestIdentifiers AssessingTests { get; set; }

    new bool IsStaticValue { get; set; }

    bool MustBeTestedInIsolation { get; set; }

    string DisplayName { get; }

    void AnalyzeTestRun(ITestIdentifiers failedTests, ITestIdentifiers resultRanTests, ITestIdentifiers timedOutTests, bool sessionTimedOut);
}
