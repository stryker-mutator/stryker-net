using System.Collections.Generic;
using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners
{
    public class TestRunResult
    {
        public TestListDescription FailingTests { get; set; } = TestListDescription.EveryTest();
        public bool Success { get; set; }
        public string ResultMessage { get; set; }
    }
}