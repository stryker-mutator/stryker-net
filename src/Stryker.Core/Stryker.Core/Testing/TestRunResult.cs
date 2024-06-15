using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Mutants;
using Stryker.Shared.Tests;

namespace Stryker.Core.Testing;

public class TestRunResult : ITestRunResult
{
    public TestRunResult(bool success, string message = null)
    {
        TestDescriptions = new List<IFrameworkTestDescription>();
        FailingTests = !success ? TestIdentifiers.EveryTest() : TestIdentifiers.NoTest();
        ExecutedTests = TestIdentifiers.EveryTest();
        TimedOutTests = TestIdentifiers.NoTest();
        ResultMessage = message;
        Duration = TimeSpan.Zero;
    }

    public TestRunResult(
        IEnumerable<IFrameworkTestDescription> vsTestDescriptions,
        ITestIdentifiers executedTests,
        ITestIdentifiers failedTests,
        ITestIdentifiers timedOutTest,
        string message,
        IEnumerable<string> messages,
        TimeSpan timeSpan)
    {
        TestDescriptions = vsTestDescriptions.Where(p => executedTests.Contains(p.Id)).ToList();
        ExecutedTests = executedTests;
        FailingTests = failedTests;
        TimedOutTests = timedOutTest;
        ResultMessage = message;
        Messages = messages;
        Duration = timeSpan;
    }

    public static TestRunResult TimedOut(
        IEnumerable<IFrameworkTestDescription> vsTestDescriptions,
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
    public IEnumerable<IFrameworkTestDescription> TestDescriptions { get; }
}
