using Stryker.Abstractions;
using Stryker.Abstractions.Testing;

namespace Stryker.Core.Initialisation;

public class InitialTestRun
{
    public InitialTestRun(ITestRunResult result, ITimeoutValueCalculator timeoutValueCalculator)
    {
        Result = result;
        TimeoutValueCalculator = timeoutValueCalculator;
    }

    public ITestRunResult Result { get; }
    public ITimeoutValueCalculator TimeoutValueCalculator { get; }
}
