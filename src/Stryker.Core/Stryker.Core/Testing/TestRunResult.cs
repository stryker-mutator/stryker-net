using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Shared.Mutants;
using Stryker.Shared.Tests;

namespace Stryker.Core.Testing;

public class TestRunResult : ITestRunResult
{
    public TestRunResult(bool success, string message = null)
    {
        VsTestDescriptions = new List<IFrameworkTestDescription>();
        FailingTests = !success ? TestGuidsList.EveryTest() : TestGuidsList.NoTest();
        ExecutedTests = TestGuidsList.EveryTest();
        TimedOutTests = TestGuidsList.NoTest();
        ResultMessage = message;
        Duration = TimeSpan.Zero;
    }

    public TestRunResult(
        IEnumerable<IFrameworkTestDescription> vsTestDescriptions,
        ITestGuids executedTests,
        ITestGuids failedTests,
        ITestGuids timedOutTest,
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
        IEnumerable<IFrameworkTestDescription> vsTestDescriptions,
        ITestGuids ranTests,
        ITestGuids failedTest,
        ITestGuids timedOutTests,
        string message,
        IEnumerable<string> messages,
        TimeSpan duration) => new(vsTestDescriptions, ranTests, failedTest, timedOutTests, message, messages, duration) { SessionTimedOut = true };

    public ITestGuids FailingTests { get; }
    public ITestGuids ExecutedTests { get; }
    public ITestGuids TimedOutTests { get; }
    public bool SessionTimedOut { get; private init; }
    public string ResultMessage { get; }
    public IEnumerable<string> Messages { get; }
    public TimeSpan Duration { get; }
    public IEnumerable<IFrameworkTestDescription> VsTestDescriptions { get; }
}
