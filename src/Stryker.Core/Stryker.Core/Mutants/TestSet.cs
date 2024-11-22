using System.Collections.Generic;
using System.Linq;
using Stryker.Abstractions.Testing;

namespace Stryker.Core.Mutants;

public class TestSet : ITestSet
{
    private readonly IDictionary<Identifier, ITestDescription> _tests = new Dictionary<Identifier, ITestDescription>();
    public int Count => _tests.Count;
    public ITestDescription this[Identifier guid] => _tests[guid];

    public void RegisterTests(IEnumerable<ITestDescription> tests)
    {
        foreach (var test in tests)
        {
            RegisterTest(test);
        }
    }

    public void RegisterTest(ITestDescription test) => _tests[test.Id] = test;

    public IEnumerable<ITestDescription> Extract(IEnumerable<Identifier> ids) => ids?.Select(i => _tests[i]) ?? Enumerable.Empty<ITestDescription>();
}
