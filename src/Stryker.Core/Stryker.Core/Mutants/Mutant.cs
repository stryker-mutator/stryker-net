using System.Collections.Generic;

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
    }

    /// <summary>
    /// Represents a single mutation on domain level
    /// </summary>
    public class Mutant : IReadOnlyMutant
    {
        public int Id { get; set; }
        public Mutation Mutation { get; set; }
        public MutantStatus ResultStatus { get; set; }
        public ITestListDescription CoveringTests { get; set; } = new TestListDescription();
        public string ResultStatusReason { get; set; }
        public bool CountForStats => ResultStatus != MutantStatus.CompileError && ResultStatus != MutantStatus.Ignored;

        public bool MustRunAgainstAllTests
        {
            get => CoveringTests.IsEveryTest;
        }

        public string DisplayName => $"{Id}: {Mutation?.DisplayName}";
        public int? Line => Mutation?.OriginalNode?.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
        public bool IsStaticValue { get; set; }

        public void DeclareCoveringTests(ICollection<TestDescription> collection)
        {
            CoveringTests = new TestListDescription(collection);
        }

        public void ResetCoverage()
        {
            CoveringTests = TestListDescription.NoTest();
        }

        public void AnalyzeTestRun(ITestListDescription failedTests, ITestListDescription resultRanTests, ITestListDescription timedOutTests)
        {
            if (!failedTests.IsEmpty && MustRunAgainstAllTests || failedTests.ContainsAny(CoveringTests))
            {
                ResultStatus = MutantStatus.Killed;
            }
            else if (resultRanTests.IsEveryTest || (!MustRunAgainstAllTests && CoveringTests.IsIncluded(resultRanTests)))
            {
                ResultStatus = MutantStatus.Survived;
            }
            else if (!timedOutTests.IsEmpty && CoveringTests.IsEveryTest || CoveringTests.ContainsAny(timedOutTests))
            {
                ResultStatus = MutantStatus.Timeout;
            }
        }
    }
}
