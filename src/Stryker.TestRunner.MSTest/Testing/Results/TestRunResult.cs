using Stryker.Shared.Tests;

namespace Stryker.TestRunner.MSTest.Testing.Results;

internal class TestRunResult : ITestRunResult
{
    private TestRunResult(
        IEnumerable<IFrameworkTestDescription> testDescriptions,
        ITestIdentifiers executedTests,
        ITestIdentifiers failingTests,
        ITestIdentifiers timedOutTests,
        bool sessionTimedOut,
        string resultMessage,
        IEnumerable<string> messages,
        TimeSpan duration)
    {
        TestDescriptions = testDescriptions;
        ExecutedTests = executedTests;
        FailingTests = failingTests;
        TimedOutTests = timedOutTests;
        SessionTimedOut = sessionTimedOut;
        ResultMessage = resultMessage;
        Messages = messages;
        Duration = duration;
    }
    public IEnumerable<IFrameworkTestDescription> TestDescriptions { get; }

    public static TestRunResult Successful(
        IEnumerable<IFrameworkTestDescription> testDescriptions,
        ITestIdentifiers executedTests,
        ITestIdentifiers failingTests,
        ITestIdentifiers timedOutTests,
        string resultMessage,
        IEnumerable<string> messages,
        TimeSpan duration) => new(testDescriptions, executedTests, failingTests, timedOutTests, false, resultMessage, messages, duration);

    public static TestRunResult TimeOut(
        IEnumerable<IFrameworkTestDescription> testDescriptions,
        ITestIdentifiers executedTests,
        ITestIdentifiers failingTests,
        ITestIdentifiers timedOutTests,
        string resultMessage,
        IEnumerable<string> messages,
        TimeSpan duration) => new(testDescriptions, executedTests, failingTests, timedOutTests, true, resultMessage, messages, duration);

    public ITestIdentifiers ExecutedTests { get; }

    public ITestIdentifiers FailingTests { get; }
    
    public ITestIdentifiers TimedOutTests { get; }

    public bool SessionTimedOut { get; private init; }

    public string ResultMessage { get; private set; }

    public IEnumerable<string> Messages { get; }

    public TimeSpan Duration { get; }
}
