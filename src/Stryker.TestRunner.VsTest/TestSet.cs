using System.Collections.Generic;
using System.Linq;
using Stryker.Abstractions.Testing;

namespace Stryker.TestRunner.VsTest;

public class TestSet : ITestSet
{
    private readonly IDictionary<string, ITestDescription> _tests = new Dictionary<string, ITestDescription>();
    public int Count => _tests.Count;
    public ITestDescription this[string id] => _tests[id];

    public void RegisterTests(IEnumerable<ITestDescription> tests)
    {
        foreach (var test in tests)
        {
            RegisterTest(test);
        }
    }

    public void RegisterTest(ITestDescription test) => _tests[test.Id] = test;

    public IEnumerable<ITestDescription> Extract(IEnumerable<string> ids) => ids?.Select(i => _tests[i]) ?? Enumerable.Empty<ITestDescription>();
}
