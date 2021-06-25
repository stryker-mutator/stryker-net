using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Mutants
{

    public class TestsGuidList : ITestListDescription
    {
        private readonly ISet<Guid> _testsGuid;

        private static readonly TestsGuidList EveryTests;
        private static readonly TestsGuidList NoTestAtAll;

        static TestsGuidList()
        {
            EveryTests = new TestsGuidList();
            NoTestAtAll = new TestsGuidList(Array.Empty<Guid>());
        }

        private TestsGuidList()
        {
            _testsGuid = null;
        }

        public TestsGuidList(IEnumerable<TestDescription> testDescriptions) : this(testDescriptions?.Select(t => t.Id))
        {
        }

        public TestsGuidList(IEnumerable<Guid> guids)
        {
            _testsGuid = guids != null ? new HashSet<Guid>(guids) : new HashSet<Guid>();
        }

        public TestsGuidList(params Guid[] guids) : this((IEnumerable<Guid>)guids)
        {
        }

        public bool IsEveryTest => _testsGuid == null;

        public bool IsEmpty => _testsGuid != null && _testsGuid.Count == 0;

        public int Count => _testsGuid?.Count ?? 0;

        public TestsGuidList Merge(ITestGuids other)
        {
            return new TestsGuidList(_testsGuid.Union(other.GetGuids()));
        }

        public bool Contains(Guid testId)
        {
            return IsEveryTest || _testsGuid.Contains(testId);
        }

        public bool IsIncluded(ITestGuids test)
        {
            return test.IsEveryTest || _testsGuid.IsSubsetOf(test.GetGuids());
        }

        public static TestsGuidList EveryTest()
        {
            return EveryTests;
        }

        public static TestsGuidList NoTest()
        {
            return NoTestAtAll;
        }

        public IEnumerable<Guid> GetGuids()
        {
            return _testsGuid;
        }

        public bool ContainsAny(ITestGuids other)
        {
            if (other.IsEmpty || IsEmpty)
            {
                return false;
            }
            if (IsEveryTest)
            {
                return true;
            }
            if (other.IsEveryTest)
            {
                return true;
            }
            return _testsGuid.Overlaps(other.GetGuids());
        }
    }
}
