namespace Stryker.Shared.Tests;

public interface ITestRunResult
{
    ITestGuids FailingTests { get; }
    ITestGuids ExecutedTests { get; }
    ITestGuids TimedOutTests { get; }
    bool SessionTimedOut { get; }
    string ResultMessage { get; }
    IEnumerable<string> Messages { get; }
    TimeSpan Duration { get; }
    IEnumerable<IFrameworkTestDescription> VsTestDescriptions { get; }
}
