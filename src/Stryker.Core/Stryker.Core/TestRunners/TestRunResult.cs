using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners
{
    public class TestRunResult
    {
        public TestRunResult(bool success, string message = null)
        {
            FailingTests = !success ? TestListDescription.EveryTest() : TestListDescription.NoTest();
            RanTests = TestListDescription.EveryTest();
            ResultMessage = message;
        }

        public TestRunResult(ITestListDescription ranTests,
            ITestListDescription failedTests,
            ITestListDescription timedOutTest,
            string message)
        {
            RanTests = ranTests;
            FailingTests = failedTests;
            TimedOutTests = timedOutTest;
            ResultMessage = message;
        }

        public static TestRunResult TimedOut(ITestListDescription ranTests,
            ITestListDescription failedTest,
            ITestListDescription timedOutTests,
            string message)
        {
            return new TestRunResult(ranTests, failedTest, timedOutTests, message){SessionTimedOut = true};
        }

        public ITestListDescription FailingTests { get; set; }
        public ITestListDescription RanTests { get; }
        public ITestListDescription TimedOutTests { get; }
        public bool SessionTimedOut { get; private set; }
        public string ResultMessage { get; set; }

        public void Merge(TestRunResult other)
        {
            ResultMessage += other.ResultMessage;
            FailingTests.AddTests(other.FailingTests);
        }
    }
}