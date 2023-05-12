using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Mutants;
using Stryker.Core.TestRunners.VsTest;

namespace Stryker.Core.TestRunners
{
    public class TestRunResult
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
            IEnumerable<VsTestDescription> vsTestDescriptions,
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
        public IEnumerable<VsTestDescription> VsTestDescriptions { get; }
    }
}
