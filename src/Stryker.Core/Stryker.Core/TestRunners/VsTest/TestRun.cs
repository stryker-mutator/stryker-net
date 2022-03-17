
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Stryker.Core.TestRunners.VsTest
{
    internal class TestRun
    {
        private readonly VsTestDescription _testDescription;
        private readonly IList<TestResult> _results;

        public TestRun(VsTestDescription testDescription)
        {
            _testDescription = testDescription;
            _results = new List<TestResult>(testDescription.NbSubCases);
        }

        public bool AddResult(TestResult result)
        {
            _results.Add(result);
            return _results.Count >= _testDescription.NbSubCases;
        }

        public bool IsComplete() => _results.Count >= _testDescription.NbSubCases;

        public TestResult Result()
        {
            var result = _results.Aggregate((TestResult)null, (acc, next) =>
            {
                if (acc == null)
                {
                    return next;
                }
                if (next.Outcome == TestOutcome.Failed || acc.Outcome == TestOutcome.None)
                {
                    acc.Outcome = next.Outcome;
                }
                if (acc.StartTime > next.StartTime)
                {
                    acc.StartTime = next.StartTime;
                }
                if (acc.EndTime < next.EndTime)
                {
                    acc.EndTime = next.EndTime;
                }

                foreach (var message in next.Messages)
                {
                    acc.Messages.Add(message);
                }

                acc.Duration = acc.EndTime - acc.StartTime;
                return acc;
            });
            return result;
        }
    }
}
