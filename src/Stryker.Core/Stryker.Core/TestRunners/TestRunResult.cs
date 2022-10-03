using System;
using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners
{
    public class TestRunResult
    {
        public TestRunResult(bool success, string message = null)
        {
            FailingTests = !success ? TestGuidsList.EveryTest() : TestGuidsList.NoTest();
            RanTests = TestGuidsList.EveryTest();
            TimedOutTests = TestGuidsList.NoTest();
            ResultMessage = message;
            Duration = TimeSpan.Zero;
        }

        public TestRunResult(ITestGuids ranTests,
            ITestGuids failedTests,
            ITestGuids timedOutTest,
            string message,
            TimeSpan timeSpan)
        {
            RanTests = ranTests;
            FailingTests = failedTests;
            TimedOutTests = timedOutTest;
            ResultMessage = message;
            Duration = timeSpan;
        }

        public static TestRunResult TimedOut(ITestGuids ranTests,
            ITestGuids failedTest,
            ITestGuids timedOutTests,
            string message,
            TimeSpan duration) =>
            new(ranTests, failedTest, timedOutTests, message, duration){SessionTimedOut = true};

        public ITestGuids FailingTests { get; }
        public ITestGuids RanTests { get; }
        public ITestGuids TimedOutTests { get; }
        public bool SessionTimedOut { get; private init; }
        public string ResultMessage { get; }
        public TimeSpan Duration { get; }
    }
}
