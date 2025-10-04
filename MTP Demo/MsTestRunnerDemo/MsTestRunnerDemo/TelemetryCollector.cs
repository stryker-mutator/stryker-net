using System.Collections.Concurrent;
using MsTestRunnerDemo.Models;

namespace MsTestRunnerDemo;

public class TelemetryCollector : ConcurrentBag<TelemetryPayload>;
