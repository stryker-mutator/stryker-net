using System;
using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners
{
    public class TestRunResult
    {
        public TestRunResult(bool success, string message = null)
        {
            Success = success;
            FailingTests = !success ? TestListDescription.EveryTest() : new TestListDescription(ArraySegment<TestDescription>.Empty);
            RanTests = TestListDescription.EveryTest();
            ResultMessage = message;
        }

        public TestRunResult(TestListDescription ranTests, TestListDescription failedTests, string message)
        {
            RanTests = ranTests;
            FailingTests = failedTests;
            ResultMessage = message;
            Success = failedTests.IsEmpty;
        }

        public static TestRunResult TimedOut(TestListDescription ranTests, TestListDescription failedTest,
            string message)
        {
            return new TestRunResult(ranTests, failedTest, message) { TimeOut = true};
        }

        public TestListDescription FailingTests { get; set; }

        public TestListDescription RanTests { get; }
        public bool Success { get; private set; }
        public bool TimeOut { get; private set; }
        public string ResultMessage { get; set; }
        public void Merge(TestRunResult other)
        {
            Success = Success && other.Success;
            ResultMessage += other.ResultMessage;
            FailingTests.AddTests(other.FailingTests);
        }
    }
}