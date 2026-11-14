using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Testing;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

/// <summary>
/// Counts the results of a test-by-test coverage capture and logs how far it has got, at most once per
/// interval. Such a capture runs every test on its own, which takes minutes to hours on a large suite,
/// and without these lines the log shows nothing between its start and its end.
/// </summary>
internal sealed class CoverageCaptureProgress
{
    private readonly ILogger _logger;
    private readonly int _testCount;
    private readonly TimeSpan _interval;
    private readonly TimeProvider _timeProvider;
    private readonly object _lock = new();
    private long _lastReport;
    private int _captured;
    private int _dubious;

    public CoverageCaptureProgress(ILogger logger, int testCount, TimeSpan interval, TimeProvider timeProvider)
    {
        _logger = logger;
        _testCount = testCount;
        _interval = interval;
        _timeProvider = timeProvider;
        _lastReport = timeProvider.GetTimestamp();
    }

    public int Dubious
    {
        get
        {
            lock (_lock)
            {
                return _dubious;
            }
        }
    }

    public void Add(ICoverageRunResult result)
    {
        int captured;
        int dubious;
        lock (_lock)
        {
            _captured++;
            if (result.Confidence == CoverageConfidence.Dubious)
            {
                _dubious++;
            }

            var now = _timeProvider.GetTimestamp();
            if (_timeProvider.GetElapsedTime(_lastReport, now) < _interval)
            {
                return;
            }

            _lastReport = now;
            captured = _captured;
            dubious = _dubious;
        }

        _logger.LogInformation("Captured coverage for {CapturedCount}/{TestCount} tests ({DubiousCount} dubious)",
            captured, _testCount, dubious);
    }
}
