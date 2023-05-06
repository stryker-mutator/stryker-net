using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;

namespace Stryker.Core.Initialisation;

public interface ITimeoutValueCalculator
{
    int CalculateTimeoutValue(int initialTestrunDurationMS);
    int DefaultTimeout { get; }
}

public class TimeoutValueCalculator : ITimeoutValueCalculator
{
    private readonly int _extraMs;
    private readonly int _initTimeMs;
    private readonly int _totalTestTime;
    private const double Ratio = 1.5;

    public TimeoutValueCalculator(int extraMS)
    {
        _extraMs = extraMS;
    }

    public TimeoutValueCalculator(int extraMs, int initTimeMs, int totalTestTime)
    {
        _extraMs = extraMs;
        _initTimeMs = initTimeMs;
        _totalTestTime = totalTestTime;
    }
    
    public int DefaultTimeout => Compute(_totalTestTime);

    private int Compute(int time) => (int)(time * Ratio) + _extraMs;
    public int CalculateTimeoutValue(int initialTestrunDurationMS) => Compute(_initTimeMs + initialTestrunDurationMS);
}
