using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

[ExcludeFromCodeCoverage]
public class TelemetryCollector : ConcurrentBag<TelemetryPayload>;
