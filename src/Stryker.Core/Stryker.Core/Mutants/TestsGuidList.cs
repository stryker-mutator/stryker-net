using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Mutants
{

    public class TestsGuidList : ITestListDescription
    {
        private readonly TestSet _testsBase;
        private readonly ISet<Guid> _testsGuid;
        public ICollection<TestDescription> Tests => _testsGuid?.Select(t => _testsBase[t]).ToHashSet();

        private static readonly TestsGuidList EveryTests;
        private static readonly TestsGuidList NoTestAtAll;

        static TestsGuidList()
        {
            EveryTests = new TestsGuidList();
            NoTestAtAll = new TestsGuidList(null, Array.Empty<Guid>());
        }

        private TestsGuidList()
        {
            _testsGuid = null;
        }

        public TestsGuidList(TestSet testsBase, IEnumerable<TestDescription> testDescriptions) : this(testsBase, testDescriptions?.Select(t => t.Id))
        {
        }

        public TestsGuidList(TestSet testsBase, IEnumerable<Guid> guids)
        {
            _testsBase = testsBase;
            _testsGuid = guids != null ? new HashSet<Guid>(guids) : new HashSet<Guid>();
        }

        public bool IsEveryTest => _testsGuid == null;

        public bool IsEmpty => _testsGuid != null && _testsGuid.Count == 0;

        public int Count => _testsGuid?.Count ?? 0;

        public TestsGuidList Merge(ITestGuids other)
        {
            return new TestsGuidList(_testsBase, _testsGuid.Union(other.GetGuids()));
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
