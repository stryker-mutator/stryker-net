using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Mutants;

namespace Stryker.Core.MutationTest
{
    public class TestRunResults : ITestRunResults
    {
        public ITestGuids RanTests { get; }
        public ITestGuids FailedTests { get; }
        public ITestGuids TimedOutTests { get; }
        public ITestGuids NonCoveringTests { get; }

        public TestRunResults(IReadOnlySet<Guid> ranTests,
            IReadOnlySet<Guid> failedTests,
            IReadOnlySet<Guid> timedOutTests,
            IReadOnlySet<Guid> nonCoveringTests)
        {
            RanTests = new TestGuidsList(ranTests);
            FailedTests = new TestGuidsList(failedTests);
            TimedOutTests = new TestGuidsList(timedOutTests);
            NonCoveringTests = new TestGuidsList(nonCoveringTests);
        }
        
        public TestRunResults(ITestGuids ranTests, ITestGuids failedTests, ITestGuids timedOutTests)
        {
            RanTests = ranTests;
            FailedTests = failedTests;
            TimedOutTests = timedOutTests;
            NonCoveringTests = TestGuidsList.NoTest();
        }

    }
}
