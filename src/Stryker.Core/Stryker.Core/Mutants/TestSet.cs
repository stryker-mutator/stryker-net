using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Mutants
{
    public class TestSet
    {
        private readonly IDictionary<Guid, TestDescription> _tests = new Dictionary<Guid, TestDescription>();
        public int Count => _tests.Count;
        public TestDescription this[Guid guid]
        {
            get
            {
                return _tests[guid];
            }
        }

        public void RegisterTests(IEnumerable<TestDescription> tests)
        {
            foreach (var test in tests)
            {
                _tests[test.Id] = test;
            }
        }

        public IEnumerable<TestDescription> Extract(IEnumerable<Guid> ids)
        {
            return ids.Select(i => _tests[i]);
        }
    }
}
