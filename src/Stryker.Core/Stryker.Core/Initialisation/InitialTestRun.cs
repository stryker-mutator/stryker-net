namespace Stryker.Core.Initialisation;
using Stryker.Core.TestRunners;

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
