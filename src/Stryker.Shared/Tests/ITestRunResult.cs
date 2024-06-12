namespace Stryker.Shared.Tests;

public interface ITestRunResult
{
    ITestIdentifiers FailingTests { get; }
    ITestIdentifiers ExecutedTests { get; }
    ITestIdentifiers TimedOutTests { get; }
    bool SessionTimedOut { get; }
    string ResultMessage { get; }
    IEnumerable<string> Messages { get; }
    TimeSpan Duration { get; }
    IEnumerable<IFrameworkTestDescription> TestDescriptions { get; }
}
