using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Mutants
{

    public class TestGuidsList : ITestGuids
    {
        private readonly HashSet<Guid> _testGuids;

        private static readonly TestGuidsList EveryTests = new();
        private static readonly TestGuidsList NoTestAtAll = new(Array.Empty<Guid>());

        private TestGuidsList() => _testGuids = null;

        public TestGuidsList(IEnumerable<TestDescription> testDescriptions) : this(testDescriptions?.Select(t => t.Id))
        { }

        public TestGuidsList(HashSet<Guid> set) => _testGuids = set;

        public TestGuidsList(IEnumerable<Guid> guids) => _testGuids = guids != null ? new HashSet<Guid>(guids) : null;

        public TestGuidsList(params Guid[] guids) : this((IEnumerable<Guid>)guids)
        { }

        public bool IsEmpty => _testGuids is { Count: 0 };

        public bool IsEveryTest => _testGuids == null;

        public int Count => _testGuids?.Count ?? 0;

        public ITestGuids Merge(ITestGuids other)
        {
            if (IsEveryTest)
            {
                return this;
            }

            var result = new HashSet<Guid>(_testGuids);
            result.UnionWith(other.GetGuids());
            return new TestGuidsList(result);
        }

        public TestGuidsList Merge(TestGuidsList other)
        {
            if (IsEveryTest)
            {
                return this;
            }

            var result = new HashSet<Guid>(_testGuids);
            result.UnionWith(other.GetGuids());
            return new TestGuidsList(result);
        }

        public bool Contains(Guid testId) => IsEveryTest || _testGuids.Contains(testId);

        public bool IsIncludedIn(ITestGuids other) => other.IsEveryTest || _testGuids?.IsSubsetOf(other.GetGuids()) == true;

        public TestGuidsList Excluding(TestGuidsList testsToSkip)
        {
            if (IsEmpty || testsToSkip.IsEmpty)
            {
                return this;
            }

            if (IsEveryTest)
            {
                throw new InvalidOperationException("Can't exclude from EveryTest");
            }

            return testsToSkip.IsEveryTest ? NoTest() : new TestGuidsList(_testGuids.Except(testsToSkip._testGuids));
        }

        public static TestGuidsList EveryTest() => EveryTests;

        public static TestGuidsList NoTest() => NoTestAtAll;

        public IEnumerable<Guid> GetGuids() => _testGuids;

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
            return _testGuids.Overlaps(other.GetGuids());
        }

        public ITestGuids Intersect(ITestGuids other) => IsEveryTest ? new TestGuidsList(other.GetGuids()) : new TestGuidsList(_testGuids.Intersect(other.GetGuids()));
    }
}
