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
        string DisplayName { get; }
        string ResultStatusReason { get; }
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
        public TestListDescription CoveringTests { get; set; } = new TestListDescription();
        public string ResultStatusReason { get; set; }
        public bool MustRunAllTests { get; set; }
        public string DisplayName => $"{Id}: {Mutation?.DisplayName}";
        public bool IsStaticValue { get; set; }

        public void AnalyzeTestRun(IReadOnlyList<TestDescription> failedTests, TestListDescription resultRanTests)
        {
            if (failedTests.Any(t => MustRunAllTests ||  CoveringTests.Contains(t.Guid)))
            {
                // a test killed us
                ResultStatus = MutantStatus.Killed;
            }
            else if (resultRanTests.GetList().Any(t => CoveringTests.Contains(t.Guid)))
            {
                ResultStatus = MutantStatus.Survived;
            }
        }
    }
}