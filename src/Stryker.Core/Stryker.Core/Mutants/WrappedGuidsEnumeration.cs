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

        public WrappedGuidsEnumeration(IEnumerable<Guid> guids)
        {
            _guids = guids;
        }

        public static ITestGuids MergeList(ITestGuids a, ITestGuids b)
        {
            if (a.GetGuids() == null)
            {
                return b;
            }

            if (b.GetGuids() == null)
            {
                return a;
            }

            return new WrappedGuidsEnumeration(a.GetGuids().Union(b.GetGuids()));
        }

        public IEnumerable<Guid> GetGuids() => _guids;
    }
}
