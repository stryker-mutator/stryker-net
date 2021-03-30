using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Mutants
{
    public interface ITestListDescription
    {
        ISet<TestDescription> Tests { get; }
        int Count { get; }
        bool IsEmpty { get; }
        bool IsEveryTest { get; }
        public TestsGuidList Merge(ITestListDescription other);
        bool Contains(TestDescription test);
        bool Contains(Guid testId);
        bool IsIncluded(ITestListDescription test);
        bool ContainsAny(IReadOnlyList<TestDescription> usedTests);
        bool ContainsAny(ITestListDescription other);
        public IEnumerable<Guid> GetGuids();
    }

    public class TestsGuidList : ITestListDescription
    {
        private readonly TestSet _testsBase;
        private readonly ISet<Guid> _testsIdsGuid = new HashSet<Guid>();
        public ISet<TestDescription> Tests => _testsIdsGuid?.Select(t => _testsBase[t]).ToHashSet();

        private static readonly ITestListDescription EveryTests;
        private static readonly ITestListDescription NoTestAtAll;

        static TestsGuidList()
        {
            EveryTests = new TestsGuidList();
            NoTestAtAll = new TestsGuidList(null, Array.Empty<TestDescription>());
        }

        private TestsGuidList()
        {
            _testsIdsGuid = null;
        }

        public TestsGuidList(TestSet testsBase, IEnumerable<TestDescription> testDescriptions) : this(testsBase, testDescriptions?.Select(t => t.Id))
        {
        }

        public TestsGuidList(TestSet testsBase, IEnumerable<Guid> guids)
        {
            _testsBase = testsBase;
            if (guids != null)
            {
                _testsIdsGuid.UnionWith(guids);
            }
        }

        public bool IsEveryTest => _testsIdsGuid == null;

        public bool IsEmpty => _testsIdsGuid != null && _testsIdsGuid.Count == 0;

        public int Count => _testsIdsGuid?.Count ?? 0;

        public TestsGuidList Merge(ITestListDescription other)
        {
            return new TestsGuidList(_testsBase, _testsIdsGuid.Union(other.GetGuids()));
        }

        public bool Contains(TestDescription test) => Contains(test.Id);

        public bool Contains(Guid testId)
        {
            return IsEveryTest || _testsIdsGuid.Contains(testId);
        }

        public bool IsIncluded(ITestListDescription test)
        {
            return test.IsEveryTest || _testsIdsGuid.All(test.Contains);
        }

        public static ITestListDescription EveryTest()
        {
            return EveryTests;
        }

        public static ITestListDescription NoTest()
        {
            return NoTestAtAll;
        }

        public IEnumerable<Guid> GetGuids()
        {
            return _testsIdsGuid;
        }

        public bool ContainsAny(ITestListDescription other)
        {
            return !other.IsEmpty && (IsEveryTest || _testsIdsGuid.Any(other.Contains));
        }

        public bool ContainsAny(IReadOnlyList<TestDescription> usedTests)
        {
            return usedTests.Select(t => t.Id).Any(Contains);
        }
    }
}
