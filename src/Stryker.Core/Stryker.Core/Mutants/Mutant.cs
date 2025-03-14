using Stryker.Abstractions;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.Tests;

namespace Stryker.Core.Mutants;

/// <summary>
/// Represents a single mutation on domain level
/// </summary>
public class Mutant : IMutant
{
    public int Id { get; set; }

    public Mutation Mutation { get; set; }

    public MutantStatus ResultStatus { get; set; }

    public ITestIdentifiers CoveringTests { get; set; } = TestIdentifierList.NoTest();

    public ITestIdentifiers KillingTests { get; set; } = TestIdentifierList.NoTest();

    public ITestIdentifiers AssessingTests { get; set; } = TestIdentifierList.EveryTest();

    public string ResultStatusReason { get; set; }

    public bool CountForStats => ResultStatus != MutantStatus.CompileError && ResultStatus != MutantStatus.Ignored;

    public bool IsStaticValue { get; set; }

    public bool MustBeTestedInIsolation { get; set; }

    public string DisplayName => $"{Id}: {Mutation?.DisplayName}";

    public void AnalyzeTestRun(ITestIdentifiers failedTests, ITestIdentifiers resultRanTests, ITestIdentifiers timedOutTests, bool sessionTimedOut)
    {
        if (AssessingTests.ContainsAny(failedTests))
        {
            ResultStatus = MutantStatus.Killed;
            KillingTests = AssessingTests.Intersect(failedTests);
        }
        else if (AssessingTests.ContainsAny(timedOutTests) || sessionTimedOut)
        {
            ResultStatus = MutantStatus.Timeout;
        }
        else if (resultRanTests.IsEveryTest || !resultRanTests.IsEveryTest && AssessingTests.IsIncludedIn(resultRanTests))
        {
            ResultStatus = MutantStatus.Survived;
        }
    }
}
