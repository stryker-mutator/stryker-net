namespace Stryker.Configuration;

public interface ITimeoutValueCalculator
{
    int CalculateTimeoutValue(int estimatedTime);
    int DefaultTimeout { get; }
}
