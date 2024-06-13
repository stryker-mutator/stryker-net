using Stryker.Shared.Tests;

namespace Stryker.TestRunner.VSTest;

internal class TestSet : ITestSet
{
    private readonly IDictionary<Guid, ITestDescription> _tests = new Dictionary<Guid, ITestDescription>();
    public int Count => _tests.Count;
    public ITestDescription this[Identifier id] => _tests[id.ToGuid()];

    public void RegisterTests(IEnumerable<ITestDescription> tests)
    {
        foreach (var test in tests)
        {
            RegisterTest(test);
        }
    }

    public void RegisterTest(ITestDescription test) => _tests[test.Id.ToGuid()] = test;

    public IEnumerable<ITestDescription> Extract(IEnumerable<Identifier> ids) => ids.Select(i => _tests[i.ToGuid()]);
}
