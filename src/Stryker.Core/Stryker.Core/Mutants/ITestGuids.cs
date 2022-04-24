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
        ITestGuids Merge(ITestGuids otherTimedOutTests);
    }

    // represents a test list
    public interface ITestListDescription : ITestGuids
    {
        bool ContainsAny(ITestGuids other);
        bool IsIncluded(ITestGuids test);
        TestsGuidList Merge(ITestGuids other);
        ITestListDescription Intersect(ITestGuids failedTests);
    }

}
