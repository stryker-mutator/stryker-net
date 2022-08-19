using System;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;

namespace Stryker.Core.TestRunners
{
    public class TestRunResult : ITestRunResults
    {
        public TestRunResult(bool success)
        {
            FailedTests = !success ? TestGuidsList.EveryTest() : TestGuidsList.NoTest();
            RanTests = TestGuidsList.EveryTest();
            TimedOutTests = TestGuidsList.NoTest();
            NonCoveringTests = TestGuidsList.NoTest();
        }

        public TestRunResult(ITestGuids ranTests,
            ITestGuids failedTests,
            ITestGuids timedOutTest,
            ITestGuids nonCoveringTests)
        {
            RanTests = ranTests;
            FailedTests = failedTests;
            TimedOutTests = timedOutTest;
            NonCoveringTests = nonCoveringTests;
        }

        public static TestRunResult TimedOut(ITestGuids ranTests,
            ITestGuids failedTest,
            ITestGuids timedOutTests,
            ITestGuids nonCoveringTests) =>
            new(ranTests, failedTest, timedOutTests, nonCoveringTests){SessionTimedOut = true};

        public ITestGuids FailedTests { get; }
        public ITestGuids RanTests { get; }
        public ITestGuids TimedOutTests { get; }
        public ITestGuids NonCoveringTests {get;}
        public bool SessionTimedOut { get; private init; }
    }

    public class InitialTestRunResult
    {
        public ITestGuids RanTests {get;}

        public ITestGuids FailedTests { get; }

        public TimeSpan Duration { get; }

        public InitialTestRunResult(ITestGuids ranTests, ITestGuids failedTests, TimeSpan duration)
        {
            RanTests = ranTests;
            FailedTests = failedTests;
            Duration = duration;
        }
    }
}
