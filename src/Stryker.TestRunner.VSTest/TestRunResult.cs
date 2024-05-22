using Stryker.Shared.Tests;

namespace Stryker.TestRunner.VSTest;

internal class TestRunResult : ITestRunResult
{
    public TestRunResult(bool success, string message = null)
    {
        VsTestDescriptions = new List<VsTestDescription>();
        FailingTests = !success ? TestGuidsList.EveryTest() : TestGuidsList.NoTest();
        ExecutedTests = TestGuidsList.EveryTest();
        TimedOutTests = TestGuidsList.NoTest();
        ResultMessage = message;
        Duration = TimeSpan.Zero;
    }

    public TestRunResult(
        IEnumerable<VsTestDescription> vsTestDescriptions,
        ITestIdentifiers executedTests,
        ITestIdentifiers failedTests,
        ITestIdentifiers timedOutTest,
        string message,
        IEnumerable<string> messages,
        TimeSpan timeSpan)
    {
        VsTestDescriptions = vsTestDescriptions.Where( p => executedTests.Contains(p.Id)).ToList();
        ExecutedTests = executedTests;
        FailingTests = failedTests;
        TimedOutTests = timedOutTest;
        ResultMessage = message;
        Messages = messages;
        Duration = timeSpan;
    }

    public static TestRunResult TimedOut(
        IEnumerable<VsTestDescription> vsTestDescriptions,
        ITestIdentifiers ranTests,
        ITestIdentifiers failedTest,
        ITestIdentifiers timedOutTests,
        string message,
        IEnumerable<string> messages,
        TimeSpan duration) => new(vsTestDescriptions, ranTests, failedTest, timedOutTests, message, messages, duration) { SessionTimedOut = true };

    public ITestIdentifiers FailingTests { get; }
    public ITestIdentifiers ExecutedTests { get; }
    public ITestIdentifiers TimedOutTests { get; }
    public bool SessionTimedOut { get; private init; }
    public string ResultMessage { get; }
    public IEnumerable<string> Messages { get; }
    public TimeSpan Duration { get; }
    public IEnumerable<IFrameworkTestDescription> VsTestDescriptions { get; }
}
