namespace Stryker.Abstractions;

public interface ITimeoutValueCalculator
{
    int CalculateTimeoutValue(int estimatedTime);
    int DefaultTimeout { get; }
}
