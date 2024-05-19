namespace Stryker.Shared.Tests;
public interface ITimeoutValueCalculator
{
    int CalculateTimeoutValue(int estimatedTime);
    int DefaultTimeout { get; }
}
