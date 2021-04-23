using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners
{
    public class TestRunResult
    {
        public TestRunResult(bool success, string message = null)
        {
            FailingTests = !success ? TestsGuidList.EveryTest() : TestsGuidList.NoTest();
            RanTests = TestsGuidList.EveryTest();
            ResultMessage = message;
        }

        public TestRunResult(ITestGuids ranTests,
            ITestGuids failedTests,
            ITestGuids timedOutTest,
            string message)
        {
            RanTests = ranTests;
            FailingTests = failedTests;
            TimedOutTests = timedOutTest;
            ResultMessage = message;
        }

        public static TestRunResult TimedOut(ITestGuids ranTests,
            ITestGuids failedTest,
            ITestGuids timedOutTests,
            string message)
        {
            return new TestRunResult(ranTests, failedTest, timedOutTests, message){SessionTimedOut = true};
        }

        public ITestGuids FailingTests { get; private set; }
        public ITestGuids RanTests { get; private set; }
        public ITestGuids TimedOutTests { get; private set; }
        public bool SessionTimedOut { get; private set; }
        public string ResultMessage { get; private set; }

        public void Merge(TestRunResult other)
        {
            ResultMessage += other.ResultMessage;
            FailingTests =  WrappedGuidsEnumeration.MergeList(FailingTests, other.FailingTests);
            TimedOutTests = WrappedGuidsEnumeration.MergeList(TimedOutTests, other.TimedOutTests);
            RanTests = WrappedGuidsEnumeration.MergeList(RanTests, other.RanTests);
        }
    }
}
