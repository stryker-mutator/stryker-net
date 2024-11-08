using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.TestRunners.MsTest.Testing.Tests;

internal class TestSet : ITestSet
{
    private readonly IDictionary<string, ITestDescription> _tests = new Dictionary<string, ITestDescription>();
    public int Count => _tests.Count;
    public ITestDescription this[Identifier id] => _tests[id.ToString()];

    public void RegisterTests(IEnumerable<ITestDescription> tests)
    {
        foreach (var test in tests)
        {
            RegisterTest(test);
        }
    }

    public void RegisterTest(ITestDescription test) => _tests[test.Id.ToString()] = test;

    public IEnumerable<ITestDescription> Extract(IEnumerable<Identifier> ids) => ids.Select(i => _tests[i.ToString()]);
}
