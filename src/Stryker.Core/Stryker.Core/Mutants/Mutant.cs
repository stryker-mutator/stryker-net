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
        ITestListDescription CoveringTests { get; }
        int? Line { get; }
        bool CountForStats { get; }
        bool MustRunAgainstAllTests { get; }
        bool IsStaticValue { get; }
        public bool MustBeTestedInIsolation { get; }
        public string Location { get; }
    }

    /// <summary>
    /// Represents a single mutation on domain level
    /// </summary>
    public class Mutant : IReadOnlyMutant
    {
        public int Id { get; set; }
        public Mutation Mutation { get; set; }
        public MutantStatus ResultStatus { get; set; }
        public ITestListDescription CoveringTests { get; set; } = TestsGuidList.EveryTest();
        public bool MustRunAgainstAllTests => CoveringTests.IsEveryTest;
        public ITestListDescription KillingTests { get; set; } = TestsGuidList.NoTest();
        public string ResultStatusReason { get; set; }
        public bool CountForStats => ResultStatus != MutantStatus.CompileError && ResultStatus != MutantStatus.Ignored;
        public bool MustRunAgainstAllTests => CoveringTests.IsEveryTest;
        public string DisplayName => $"{Id}: {Mutation?.DisplayName}";
        public string Location
        {
            get
            {
                var location = Mutation?.OriginalNode?.GetLocation().GetMappedLineSpan();
                return location == null ? "Unknown location." : $"{location.Value.Path} line {location.Value.StartLinePosition.Line}:{location.Value.StartLinePosition.Character}";
            }
        }

        public int? Line => Mutation?.OriginalNode?.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
        public bool IsStaticValue { get; set; }
        public bool CannotDetermineCoverage { get; set; }
        public bool MustBeTestedInIsolation { get; set; }
        public void ResetCoverage() => CoveringTests = TestsGuidList.NoTest();
        public void AnalyzeTestRun(ITestGuids failedTests, ITestGuids resultRanTests, ITestGuids timedOutTests)
        {
            if (CoveringTests.ContainsAny(failedTests))
            {
                ResultStatus = MutantStatus.Killed;
                KillingTests = CoveringTests.Intersect(failedTests);
            }
            else if (resultRanTests.IsEveryTest || (MustRunAgainstAllTests is not true && CoveringTests.IsIncluded(resultRanTests)))
            {
                ResultStatus = MutantStatus.Survived;
            }
            else if (CoveringTests.ContainsAny(timedOutTests))
            {
                ResultStatus = MutantStatus.Timeout;
            }
        }
    }
}
