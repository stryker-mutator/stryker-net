using Stryker.Shared.Tests;

namespace Stryker.TestRunner.VSTest;

internal class TestSet : ITestSet
{
    private readonly IDictionary<Guid, ITestDescription> _tests = new Dictionary<Guid, ITestDescription>();
    public int Count => _tests.Count;
    public ITestDescription this[Guid id] => _tests[id];

    public void RegisterTests(IEnumerable<ITestDescription> tests)
    {
        foreach (var test in tests)
        {
            RegisterTest(test);
        }
    }

    public void RegisterTest(ITestDescription test) => _tests[test.Id] = test;

    public IEnumerable<ITestDescription> Extract(IEnumerable<Guid> ids) => ids.Select(i => _tests[i]);
}