using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Mutants
{

    public class TestsGuidList : ITestGuids
    {
        private readonly ISet<Guid> _testsGuid;

        private static readonly TestsGuidList everyTests = new();
        private static readonly TestsGuidList noTestAtAll = new(Array.Empty<Guid>());

        private TestsGuidList() => _testsGuid = null;

        public TestsGuidList(IEnumerable<TestDescription> testDescriptions) : this(testDescriptions?.Select(t => t.Id))
        {
        }

        public TestsGuidList(IEnumerable<Guid> guids) => _testsGuid = guids != null ? new HashSet<Guid>(guids) : null;

        public TestsGuidList(params Guid[] guids) : this((IEnumerable<Guid>)guids)
        {
        }

        public bool IsEmpty => _testsGuid is { Count: 0 };
        public bool IsEveryTest => _testsGuid == null;
        
        public int Count => _testsGuid?.Count ?? 0;

        public ITestGuids Merge(ITestGuids other) => new TestsGuidList(_testsGuid == null ? other.GetGuids() : _testsGuid.Union(other.GetGuids()));

        public bool Contains(Guid testId) => IsEveryTest || _testsGuid.Contains(testId);

        public bool IsIncludedIn(ITestGuids test) => test.IsEveryTest || _testsGuid.IsSubsetOf(test.GetGuids());

        public static TestsGuidList EveryTest() => everyTests;

        public static TestsGuidList NoTest() => noTestAtAll;

        public IEnumerable<Guid> GetGuids() => _testsGuid;

        public bool ContainsAny(ITestGuids other)
        {
            if (other.IsEmpty || IsEmpty)
            {
                return false;
            }
            if (IsEveryTest || other.IsEveryTest)
            {
                return true;
            }
            return _testsGuid.Overlaps(other.GetGuids());
        }
    }
}
