using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Mutants
{

    public class TestGuidsList : ITestGuids
    {
        private readonly HashSet<Guid> _testsGuid;

        private static readonly TestGuidsList EveryTests = new();
        private static readonly TestGuidsList NoTestAtAll = new(Array.Empty<Guid>());

        private TestGuidsList() => _testsGuid = null;

        public TestGuidsList(IEnumerable<TestDescription> testDescriptions) : this(testDescriptions?.Select(t => t.Id))
        {
        }

        public TestGuidsList(HashSet<Guid> set) => _testsGuid = set;
        
        public TestGuidsList(IEnumerable<Guid> guids) => _testsGuid = guids != null ? new HashSet<Guid>(guids) : null;

        public TestGuidsList(params Guid[] guids) : this((IEnumerable<Guid>)guids)
        {}

        public bool IsEmpty => _testsGuid is { Count: 0 };
        
        public bool IsEveryTest => _testsGuid == null;
        
        public int Count => _testsGuid?.Count ?? 0;

        public ITestGuids Merge(ITestGuids other)
        {
            if (IsEveryTest)
            {
                return this;
            }

            var result = new HashSet<Guid>(_testsGuid);
            result.UnionWith(other.GetGuids());
            return new TestGuidsList(result);
        }

        public TestGuidsList Merge(TestGuidsList other)
        {
            if (IsEveryTest)
            {
                return this;
            }

            var result = new HashSet<Guid>(_testsGuid);
            result.UnionWith(other.GetGuids());
            return new TestGuidsList(result);
        }

        public bool Contains(Guid id) => IsEveryTest || _testsGuid.Contains(id);

        public bool IsIncludedIn(ITestGuids other) => other.IsEveryTest || _testsGuid?.IsSubsetOf(other.GetGuids())==true;

        public ITestGuids Intersect(ITestGuids failedTests)
        {
            if (IsEveryTest)
            {
                return failedTests;
            }

            if (failedTests.IsEveryTest)
            {
                return this;
            }
            var result = new HashSet<Guid>(_testsGuid);
            result.IntersectWith(failedTests.GetGuids());
            return new TestGuidsList(result);
        }

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

            return testsToSkip.IsEveryTest ? NoTest() : new TestGuidsList(_testsGuid.Except(testsToSkip._testsGuid));
        }

        public static TestGuidsList EveryTest() => EveryTests;

        public static TestGuidsList NoTest() => NoTestAtAll;

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
