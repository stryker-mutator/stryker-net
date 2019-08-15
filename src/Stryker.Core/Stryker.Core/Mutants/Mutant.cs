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
        IDictionary<string, bool> CoveringTests { get; }
        string ResultStatusReason { get; }
        bool IsStaticValue { get; }
        bool MustRunAllTests { get; }
    }
    
    /// <summary>
    /// Represents a single mutation on domain level
    /// </summary>
    public class Mutant : IReadOnlyMutant
    {
        public int Id { get; set; }
        public Mutation Mutation { get; set; }
        public MutantStatus ResultStatus { get; set; }
        public IDictionary<string, bool> CoveringTests { get; set; } = new Dictionary<string, bool>();
        public string ResultStatusReason { get; set; }
        public bool MustRunAllTests { get; set; }
        public string DisplayName => $"{Id}: {Mutation?.DisplayName}";
        public bool IsStaticValue { get; set; }
        public bool IsTestedBy(string testId) => CoveringTests.ContainsKey(testId);
    }
}