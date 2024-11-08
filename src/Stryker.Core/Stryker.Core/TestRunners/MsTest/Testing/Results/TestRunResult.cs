using System;
using System.Collections.Generic;
using Stryker.Core.TestRunners.MsTest.Testing.Tests;

namespace Stryker.Core.TestRunners.MsTest.Testing.Results;

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
        TimeSpan duration) => new(testDescriptions, executedTests, failingTests, timedOutTests, false, string.Empty, [], duration);

    public static TestRunResult None(IEnumerable<IFrameworkTestDescription> testDescriptions, string resultMessage)
        => new(testDescriptions, TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), false, resultMessage, [], TimeSpan.Zero);

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
