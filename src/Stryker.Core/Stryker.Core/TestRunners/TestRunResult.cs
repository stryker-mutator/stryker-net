using System;
using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners
{
    public class TestRunResult
    {
        public TestRunResult(bool success)
        {
            Success = success;
            FailingTests = !success ? TestListDescription.EveryTest() : new TestListDescription(ArraySegment<TestDescription>.Empty);
        }

        public TestListDescription FailingTests { get; set; }
        public bool Success { get; }
        public string ResultMessage { get; set; }
    }
}