using System;
using Stryker.Abstractions;

namespace Stryker.Core.Initialisation;

public class TimeoutValueCalculator : ITimeoutValueCalculator
{
    public const double DefaultRatio = 1.5;

    private readonly int _extraMs;
    private readonly int _initializationTime;
    private readonly int _aggregatedTestTimes;
    private readonly double _ratio;

    public TimeoutValueCalculator(int extraMs, double ratio = DefaultRatio)
    {
        _extraMs = extraMs;
        _ratio = ratio;
    }

    public TimeoutValueCalculator(int extraMs, int testSessionTime, int aggregatedTestTimes, double ratio = DefaultRatio)
    {
        _extraMs = extraMs;
        _initializationTime = Math.Max(testSessionTime - aggregatedTestTimes, 0);
        _aggregatedTestTimes = aggregatedTestTimes;
        _ratio = ratio;
    }

    public int DefaultTimeout => CalculateTimeoutValue(_aggregatedTestTimes);

    public int CalculateTimeoutValue(int estimatedTime) => (int)((_initializationTime + estimatedTime) * _ratio) + _extraMs;
}
