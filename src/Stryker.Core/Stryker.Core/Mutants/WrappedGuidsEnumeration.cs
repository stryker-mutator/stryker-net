using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Mutants
{
    public class WrappedGuidsEnumeration : ITestGuids
    {
        private readonly IEnumerable<Guid> _guids;

        public ICollection<TestDescription> Tests => throw new NotImplementedException();

        public int Count => _guids.Count();

        public bool IsEmpty => _guids == null || !_guids.Any();

        public bool IsEveryTest => false;
        public ITestGuids Merge(ITestGuids other) => MergeList(this, other);

        public bool Contains(Guid id) => _guids.Any( g => g == id);

        public bool ContainsAny(ITestGuids other) => _guids.Any(other.Contains);

        public bool IsIncludedIn(ITestGuids other) => _guids.All(other.Contains);

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
            var result = new HashSet<Guid>(_guids);
            result.IntersectWith(failedTests.GetGuids());
            return new TestGuidsList(result);
        }

        public WrappedGuidsEnumeration(IEnumerable<Guid> guids) => _guids = guids;

        public static ITestGuids MergeList(ITestGuids a, ITestGuids b)
        {
            if (a.GetGuids() == null)
            {
                return b;
            }

            return b.GetGuids() == null ? a : new WrappedGuidsEnumeration(a.GetGuids().Union(b.GetGuids()));
        }

        public IEnumerable<Guid> GetGuids() => _guids;
    }
}
