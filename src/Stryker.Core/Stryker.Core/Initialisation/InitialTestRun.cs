using Stryker.Core.Mutants;

namespace Stryker.Core.Initialisation
{
    public class InitialTestRun
    {
        public InitialTestRun(ITestGuids ranTests, ITestGuids failedTests, ITimeoutValueCalculator timeoutValueCalculator)
        {
            AllTests = ranTests;
            FailedTests = failedTests;

            TimeoutValueCalculator = timeoutValueCalculator;
        }
         
        public ITestGuids AllTests { get; }

        public ITestGuids FailedTests { get; }

        public ITimeoutValueCalculator TimeoutValueCalculator { get;}
    }
}
