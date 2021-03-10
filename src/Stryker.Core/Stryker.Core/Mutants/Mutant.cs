using System.Linq;

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
        string LongName { get; }
        int? Line { get; }
        bool CountForStats { get; }
        bool MustRunAgainstAllTests { get; }
    }

    /// <summary>
    /// Represents a single mutation on domain level
    /// </summary>
    public class Mutant : IReadOnlyMutant
    {
        private bool _mustRunAllTests;
        public int Id { get; set; }
        public Mutation Mutation { get; set; }
        public MutantStatus ResultStatus { get; set; }
        public ITestListDescription CoveringTests { get; private set; } = new TestListDescription();
        public string ResultStatusReason { get; set; }
        public bool CountForStats => ResultStatus != MutantStatus.CompileError && ResultStatus != MutantStatus.Ignored;

        public bool MustRunAgainstAllTests
        {
            get => CoveringTests.IsEveryTest || _mustRunAllTests;
            set => _mustRunAllTests = value;
        }

        public string DisplayName => $"{Id}: {Mutation?.DisplayName}";

        public string LongName =>
            $"{Mutation?.DisplayName} on line {Line}: '{Mutation?.OriginalNode}' ==> '{Mutation?.ReplacementNode}'";

        public int? Line => Mutation?.OriginalNode?.GetLocation().GetLineSpan().StartLinePosition.Line + 1;

        public void DeclareCoveringTest(TestDescription test)
        {
            if (test.IsAllTests)
            {
                CoveringTests = TestListDescription.EveryTest();
                return;
            }
            CoveringTests = CoveringTests.Add(test);
        }

        public void ResetCoverage()
        {
            CoveringTests = TestListDescription.NoTest();
        }

        public bool IsStaticValue { get; set; }

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
