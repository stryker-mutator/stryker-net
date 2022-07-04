
namespace Stryker.Core.Mutants
{
    /// <summary>
    /// This interface should only contain readonly properties to ensure that others than the mutation test process cannot modify mutants.
    /// </summary>
    public interface IReadOnlyMutant
    {
        int Id { get; }
        Mutation Mutation { get; }
        MutantStatus ResultStatus { get; }
        string ResultStatusReason { get; }
        ITestGuids CoveringTests { get; }
        ITestGuids AssessingTests { get; }
        int? Line { get; }
        bool CountForStats { get; }
        bool IsStaticValue { get; }
    }

    /// <summary>
    /// Represents a single mutation on domain level
    /// </summary>
    public class Mutant : IReadOnlyMutant
    {
        public int Id { get; set; }

        public Mutation Mutation { get; set; }

        public MutantStatus ResultStatus { get; set; }

        public ITestGuids CoveringTests { get; set; } = TestsGuidList.NoTest();
        
        public ITestGuids AssessingTests { get; set; } = TestsGuidList.EveryTest();
        
        public string ResultStatusReason { get; set; }

        public bool CountForStats => ResultStatus != MutantStatus.CompileError && ResultStatus != MutantStatus.Ignored;

        public bool IsStaticValue { get; set; }

        public bool MustBeTestedInIsolation { get; set; }

        public string DisplayName => $"{Id}: {Mutation?.DisplayName}";

        public int? Line => Mutation?.OriginalNode?.GetLocation().GetLineSpan().StartLinePosition.Line + 1;

        public void AnalyzeTestRun(ITestGuids failedTests, ITestGuids resultRanTests, ITestGuids timedOutTests)
        {
            if (AssessingTests.ContainsAny(failedTests))
            {
                ResultStatus = MutantStatus.Killed;
            }
            else if (resultRanTests.IsEveryTest || (resultRanTests.IsEveryTest is not true && AssessingTests.IsIncludedIn(resultRanTests)))
            {
                ResultStatus = MutantStatus.Survived;
            }
            else if (AssessingTests.ContainsAny(timedOutTests))
            {
                ResultStatus = MutantStatus.Timeout;
            }
        }
    }
}
