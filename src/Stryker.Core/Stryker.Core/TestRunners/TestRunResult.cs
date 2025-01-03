//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Stryker.Abstractions.Testing;
//using Stryker.TestRunner.Tests;

//namespace Stryker.Core.TestRunners;

//public class TestRunResult : ITestRunResult
//{
//    public TestRunResult(bool success, string message = null)
//    {
//        TestDescriptions = new List<IFrameworkTestDescription>();
//        FailingTests = !success ? TestIdentifierList.EveryTest() : TestIdentifierList.NoTest();
//        ExecutedTests = TestIdentifierList.EveryTest();
//        TimedOutTests = TestIdentifierList.NoTest();
//        ResultMessage = message;
//        Duration = TimeSpan.Zero;
//    }

//    public TestRunResult(
//        IEnumerable<IFrameworkTestDescription> testDescriptions,
//        ITestIdentifiers executedTests,
//        ITestIdentifiers failedTests,
//        ITestIdentifiers timedOutTest,
//        string message,
//        IEnumerable<string> messages,
//        TimeSpan timeSpan)
//    {
//        TestDescriptions = testDescriptions.Where(p => executedTests.Contains(p.Id)).ToList();
//        ExecutedTests = executedTests;
//        FailingTests = failedTests;
//        TimedOutTests = timedOutTest;
//        ResultMessage = message;
//        Messages = messages;
//        Duration = timeSpan;
//    }

//    public static TestRunResult None(IEnumerable<IFrameworkTestDescription> testDescriptions, string message)
//        => new(testDescriptions, TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), message, Array.Empty<string>(), TimeSpan.Zero);

//    public static TestRunResult TimedOut(
//        IEnumerable<IFrameworkTestDescription> testDescriptions,
//        ITestIdentifiers ranTests,
//        ITestIdentifiers failedTest,
//        ITestIdentifiers timedOutTests,
//        string message,
//        IEnumerable<string> messages,
//        TimeSpan duration)
//        => new(testDescriptions, ranTests, failedTest, timedOutTests, message, messages, duration) { SessionTimedOut = true };

//    public static TestRunResult Successful(IEnumerable<IFrameworkTestDescription> testDescriptions,
//        ITestIdentifiers executedTests,
//        ITestIdentifiers failedTests,
//        ITestIdentifiers timedOutTests,
//        IEnumerable<string> messages,
//        TimeSpan duration)
//        => new(testDescriptions, executedTests, failedTests, timedOutTests, "All tests passed", messages, duration);

//    public ITestIdentifiers FailingTests { get; }
//    public ITestIdentifiers ExecutedTests { get; }
//    public ITestIdentifiers TimedOutTests { get; }
//    public bool SessionTimedOut { get; private init; }
//    public string ResultMessage { get; }
//    public IEnumerable<string> Messages { get; }
//    public TimeSpan Duration { get; }
//    public IEnumerable<IFrameworkTestDescription> TestDescriptions { get; }
//}
