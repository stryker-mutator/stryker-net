using System.Collections.Generic;
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
        TestListDescription CoveringTests { get; }
        string LongName { get; }
        string DisplayName { get; }
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
        public TestListDescription CoveringTests { get; set; } = new TestListDescription();
        public string ResultStatusReason { get; set; }

        public bool MustRunAgainstAllTests
        {
            get => CoveringTests.IsEveryTest || _mustRunAllTests;
            set => _mustRunAllTests = value;
        }

        public string DisplayName => $"{Id}: {Mutation?.DisplayName}";

        public string LongName =>
            $"{Mutation?.DisplayName} on line {Mutation?.OriginalNode?.GetLocation().GetLineSpan().StartLinePosition.Line + 1}: '{Mutation?.OriginalNode}' ==> '{Mutation?.ReplacementNode}'";
        public bool IsStaticValue { get; set; }

        public void AnalyzeTestRun(TestListDescription failedTests, TestListDescription resultRanTests)
        {
            if (failedTests.GetList().Any(t =>IsStaticValue ||  CoveringTests.Contains(t.Guid)))
            {
                // a test killed us
                ResultStatus = MutantStatus.Killed;
            }
            else if (resultRanTests.IsEveryTest || (!CoveringTests.IsEveryTest && CoveringTests.GetList().All(x => resultRanTests.Contains(x.Guid))))
            {
                ResultStatus = MutantStatus.Survived;
            }
        }
    }
}