using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;

namespace Stryker.Core.TestRunners
{
    public class TestRunResults : ITestRunResults
    {
        public TestRunResults(ITestGuids ranTests,
            ITestGuids failedTests,
            ITestGuids timedOutTest,
            ITestGuids nonCoveringTests)
        {
            RanTests = ranTests;
            FailedTests = failedTests;
            TimedOutTests = timedOutTest;
            NonCoveringTests = nonCoveringTests;
        }

        public ITestGuids FailedTests { get; }
        public ITestGuids RanTests { get; }
        public ITestGuids TimedOutTests { get; }
        public ITestGuids NonCoveringTests {get;}

        public bool SessionTimedOut { get; private init; }
        public static TestRunResults TimedOut(ITestGuids ranTests,
            ITestGuids failedTest,
            ITestGuids timedOutTests,
            ITestGuids nonCoveringTests) =>
            new(ranTests, failedTest, timedOutTests, nonCoveringTests){SessionTimedOut = true};

        public static TestRunResults GeneralSuccess()
            => new(TestGuidsList.EveryTest(), TestGuidsList.NoTest(), TestGuidsList.NoTest(), TestGuidsList.NoTest());

        public static TestRunResults GeneralFailure()
            => new(TestGuidsList.EveryTest(), TestGuidsList.EveryTest(), TestGuidsList.NoTest(), TestGuidsList.NoTest());

    }
}
