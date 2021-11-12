using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Mutants
{

    public class TestsGuidList : ITestListDescription
    {
        private readonly ISet<Guid> _testsGuid;

        private static readonly TestsGuidList EveryTests = new();
        private static readonly TestsGuidList NoTestAtAll = new(Array.Empty<Guid>());

        private TestsGuidList() => _testsGuid = null;

        public TestsGuidList(IEnumerable<TestDescription> testDescriptions) : this(testDescriptions?.Select(t => t.Id))
        {
        }

        public TestsGuidList(IEnumerable<Guid> guids) => _testsGuid = guids != null ? new HashSet<Guid>(guids) : new HashSet<Guid>();

        public TestsGuidList(params Guid[] guids) : this((IEnumerable<Guid>)guids)
        {
        }

        public bool IsEveryTest => _testsGuid == null;

        public bool IsEmpty => _testsGuid is { Count: 0 };

        public int Count => _testsGuid?.Count ?? 0;

        public TestsGuidList Merge(ITestGuids other) => new(_testsGuid.Union(other.GetGuids()));
        public ITestListDescription Intersect(ITestGuids failedTests) => IsEveryTest ? new TestsGuidList(failedTests.GetGuids()) : new TestsGuidList(failedTests.GetGuids().Intersect(_testsGuid));

        public bool Contains(Guid testId) => IsEveryTest || _testsGuid.Contains(testId);

        public bool IsIncluded(ITestGuids test) => test.IsEveryTest || _testsGuid.IsSubsetOf(test.GetGuids());

        public static TestsGuidList EveryTest() => EveryTests;

        public static TestsGuidList NoTest() => NoTestAtAll;

        public IEnumerable<Guid> GetGuids() => _testsGuid;

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
            return other.IsEveryTest || _testsGuid.Overlaps(other.GetGuids());
        }
    }
}
