using System.Collections.Concurrent;

namespace MsTestRunnerDemo;

public class LogsCollector : ConcurrentBag<TestingPlatformClient.Log>;
