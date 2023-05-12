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

        public bool Contains(Guid testId) => _guids.Any(g => g == testId);

        public bool ContainsAny(ITestGuids other) => _guids.Any(other.Contains);

        public bool IsIncludedIn(ITestGuids other) => _guids.All(other.Contains);

        public WrappedGuidsEnumeration(IEnumerable<Guid> guids) => _guids = guids;

        public ITestGuids Excluding(ISet<Guid> testsToSkip) => (IsEveryTest || IsEmpty) ? this : new TestGuidsList(_guids.Except(testsToSkip));

        public static ITestGuids MergeList(ITestGuids a, ITestGuids b)
        {
            if (a.GetGuids() == null)
            {
                return b;
            }

            return b.GetGuids() == null ? a : new WrappedGuidsEnumeration(a.GetGuids().Union(b.GetGuids()));
        }

        public IEnumerable<Guid> GetGuids() => _guids;

        public ITestGuids Intersect(ITestGuids other) => IsEveryTest ? new WrappedGuidsEnumeration(other.GetGuids()) : new WrappedGuidsEnumeration(_guids.Intersect(other.GetGuids()));
    }
}
