using System.Collections.Concurrent;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

public class TelemetryCollector : ConcurrentBag<TelemetryPayload>;
