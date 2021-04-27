using System;
using System.Collections.Generic;

namespace Stryker.Core.Mutants
{
    // represents a simple list of test identifier with some properties
    public interface ITestGuids
    {
        public IEnumerable<Guid> GetGuids();
        int Count { get; }
        bool IsEmpty { get; }
        bool IsEveryTest { get; }
    }

    // represents a test list
    public interface ITestListDescription : ITestGuids
    {
        ICollection<TestDescription> Tests { get; }
        bool Contains(Guid testId);
        bool ContainsAny(ITestGuids other);
        bool IsIncluded(ITestGuids test);
        TestsGuidList Merge(ITestGuids other);
    }

}
