using System;
using System.Collections.Generic;

namespace Stryker.Core.Mutants
{
    public class TestSet
    {
        private readonly IDictionary<Guid, TestDescription> _tests = new Dictionary<Guid, TestDescription>();

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
    }
}
