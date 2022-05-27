using System;
using System.Collections.Generic;
using Stryker.Core.Mutants;
using Stryker.Core.TestRunners.VsTest;

namespace Stryker.Core.TestRunners
{
    public class TestRunResult
    {
        public TestRunResult(bool success, string message = null)
        {
            FailingTests = !success ? TestsGuidList.EveryTest() : TestsGuidList.NoTest();
            RanTests = TestsGuidList.EveryTest();
            ResultMessage = message;
            Duration = TimeSpan.Zero;
        }

        public TestRunResult(
            IEnumerable<VsTestDescription> vsTestDescriptions,
            ITestGuids ranTests,
            ITestGuids failedTests,
            ITestGuids timedOutTest,
            string message,
            TimeSpan timeSpan)
        {
            VsTestDescriptions = vsTestDescriptions;
            RanTests = ranTests;
            FailingTests = failedTests;
            TimedOutTests = timedOutTest;
            ResultMessage = message;
            Duration = timeSpan;
        }

        public static TestRunResult TimedOut(
            IEnumerable<VsTestDescription> vsTestDescriptions,
            ITestGuids ranTests,
            ITestGuids failedTest,
            ITestGuids timedOutTests,
            string message,
            TimeSpan duration) => new TestRunResult(vsTestDescriptions, ranTests, failedTest, timedOutTests, message, duration) { SessionTimedOut = true };

        public ITestGuids FailingTests { get; }
        public ITestGuids RanTests { get; }
        public ITestGuids TimedOutTests { get; }
        public bool SessionTimedOut { get; private set; }
        public string ResultMessage { get; }
        public TimeSpan Duration { get; }
        public IEnumerable<VsTestDescription> VsTestDescriptions { get; private set; }
    }
}
