using System;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;

namespace Stryker.Core.TestRunners
{
    public class TestRunResult : ITestRunResults
    {
        public TestRunResult(bool success, string message = null)
        {
            FailedTests = !success ? TestGuidsList.EveryTest() : TestGuidsList.NoTest();
            RanTests = TestGuidsList.EveryTest();
            TimedOutTests = TestGuidsList.NoTest();
            NonCoveringTests = TestGuidsList.NoTest();
            ResultMessage = message;
            Duration = TimeSpan.Zero;
        }

        public TestRunResult(ITestGuids ranTests,
            ITestGuids failedTests,
            ITestGuids timedOutTest,
            ITestGuids nonCoveringTests,
            string message,
            TimeSpan timeSpan)
        {
            RanTests = ranTests;
            FailedTests = failedTests;
            TimedOutTests = timedOutTest;
            NonCoveringTests = nonCoveringTests;
            ResultMessage = message;
            Duration = timeSpan;
        }

        public static TestRunResult TimedOut(ITestGuids ranTests,
            ITestGuids failedTest,
            ITestGuids timedOutTests,
            ITestGuids nonCoveringTests,
            string message,
            TimeSpan duration) =>
            new(ranTests, failedTest, timedOutTests, nonCoveringTests, message, duration){SessionTimedOut = true};

        public ITestGuids FailedTests { get; }
        public ITestGuids RanTests { get; }
        public ITestGuids TimedOutTests { get; }
        public ITestGuids NonCoveringTests {get;}
        public bool SessionTimedOut { get; private init; }
        public string ResultMessage { get; }
        public TimeSpan Duration { get; }
    }
}
