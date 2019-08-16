using System.Collections.Generic;
using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners
{
    public class TestRunResult
    {
        public IList<TestDescription> FailingTests { get; set; }
        public bool Success { get; set; }
        public string ResultMessage { get; set; }
    }
}