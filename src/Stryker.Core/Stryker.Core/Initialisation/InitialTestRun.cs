using Stryker.Abstractions.TestRunners;

namespace Stryker.Abstractions.Initialisation
{
    public class InitialTestRun
    {
        public InitialTestRun(TestRunResult result, ITimeoutValueCalculator timeoutValueCalculator)
        {
            Result = result;
            TimeoutValueCalculator = timeoutValueCalculator;
        }

        public TestRunResult Result { get; }
        public ITimeoutValueCalculator TimeoutValueCalculator { get;}
    }
}
