using Stryker.Abstractions.Testing;

namespace Stryker.Abstractions;

public interface IMutant : IReadOnlyMutant
{
    new int Id { get; set; }
    new Mutation Mutation { get; set; }
    new MutantStatus ResultStatus { get; set; }
    new string ResultStatusReason { get; set; }
    new ITestIdentifiers CoveringTests { get; set; }
    new ITestIdentifiers KillingTests { get; set; }
    new ITestIdentifiers AssessingTests { get; set; }
    new bool CountForStats { get; }
    new bool IsStaticValue { get; set; }

    string DisplayName { get; }
    bool MustBeTestedInIsolation { get; set; }

    void AnalyzeTestRun(ITestIdentifiers failedTests, ITestIdentifiers resultRanTests, ITestIdentifiers timedOutTests, bool sessionTimedOut);
}
