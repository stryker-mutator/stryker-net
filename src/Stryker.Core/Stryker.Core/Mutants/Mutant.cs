using Microsoft.CodeAnalysis;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.TestRunners;

namespace Stryker.Core.Mutants;

/// <summary>
/// Represents a single mutation on domain level
/// </summary>
public class Mutant : IMutant
{
    public int Id { get; set; }

    public Mutation Mutation { get; set; }

    public MutantStatus ResultStatus { get; set; }

    public ITestGuids CoveringTests { get; set; } = TestGuidsList.NoTest();

    public ITestGuids KillingTests { get; set; } = TestGuidsList.NoTest();

    public ITestGuids AssessingTests { get; set; } = TestGuidsList.EveryTest();

    public string ResultStatusReason { get; set; }

    public bool CountForStats => ResultStatus != MutantStatus.CompileError && ResultStatus != MutantStatus.Ignored;

    public bool IsStaticValue { get; set; }

    public bool MustBeTestedInIsolation { get; set; }

    public string DisplayName => $"{Id}: {Mutation?.DisplayName}";

    public string ReplacementText { get; init; }

    public FileLinePositionSpan OriginalLocation { get; init; }

    public void AnalyzeTestRun(ITestGuids failedTests, ITestGuids resultRanTests, ITestGuids timedOutTests, bool sessionTimedOut)
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
