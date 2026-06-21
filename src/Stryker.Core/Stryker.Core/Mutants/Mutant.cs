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

    public bool CountForStats => ResultStatus is not (MutantStatus.CompileError or MutantStatus.RuntimeError or MutantStatus.Ignored);

    public bool IsStaticValue { get; set; }

    public bool MustBeTestedInIsolation { get; set; }

    public string DisplayName => $"{Id}: {Mutation?.DisplayName}";

    public void AnalyzeTestRun(ITestIdentifiers failedTests, ITestIdentifiers resultRanTests, ITestIdentifiers timedOutTests, bool sessionTimedOut, bool sessionRuntimeError)
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
        else if (sessionRuntimeError)
        {
            // The test host crashed (e.g. a mutation caused a fatal fault). The mutant cannot be
            // conclusively tested, so it is a runtime error (excluded from the mutation score).
            ResultStatus = MutantStatus.RuntimeError;
            ResultStatusReason = "The test host crashed or became unreachable while testing this mutant";
        }
        else if (resultRanTests.IsEveryTest || !resultRanTests.IsEveryTest && AssessingTests.IsIncludedIn(resultRanTests))
        {
            ResultStatus = MutantStatus.Survived;
        }
    }
}
