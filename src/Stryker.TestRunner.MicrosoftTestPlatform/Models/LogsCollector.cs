using System.Collections.Concurrent;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

public class LogsCollector : ConcurrentBag<TestingPlatformClient.Log>;
