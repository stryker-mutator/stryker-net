using Stryker.Shared.Tests;
using Stryker.TestRunner.MSTest.Testing.Tests;

namespace Stryker.TestRunner.MSTest.Testing.Results;
internal class DiscoveryResult
{
    public IDictionary<string, MsTestDescription> MsTests { get; private set; } = new Dictionary<string, MsTestDescription>();

    public IDictionary<string, ISet<string>> TestsPerSource { get; } = new Dictionary<string, ISet<string>>();

    public ITestSet Tests { get; } = new TestSet();

    public ITestSet GetTestsForSources(IEnumerable<string> sources)
    {
        var result = new TestSet();
        foreach (var source in sources)
        {
            var tests = TestsPerSource[source];
            result.RegisterTests(TestsPerSource[source].Select(id => Tests[Identifier.Create(id)]));
        }

        return result;
    }
    public IEnumerable<string> GetValidSources(IEnumerable<string> sources) =>
          sources.Where(s => TestsPerSource.TryGetValue(s, out var result) && result.Count > 0);

    public void AddTestSource(string source, string testId)
    {
        var hasValue = TestsPerSource.TryGetValue(source, out var result);

        if (hasValue)
        {
            result!.Add(testId);
            return;
        }

        TestsPerSource.Add(source, new HashSet<string> { testId });
    }

    public void AddTestDescription(string testId, ITestCase testCase)
    {
        var hasValue = MsTests.TryGetValue(testId, out var result);

        if (hasValue)
        {
            result!.AddSubCase();
            return;
        }

        MsTests.Add(testId, new MsTestDescription(testCase));
    }

    public void AddTest(ITestDescription testDescription) => Tests.RegisterTest(testDescription);
}
