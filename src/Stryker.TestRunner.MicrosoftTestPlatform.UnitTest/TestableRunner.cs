using Microsoft.Extensions.Logging.Abstractions;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

internal class TestableRunner : SingleMicrosoftTestPlatformRunner
{
    private readonly Action _onDispose;
    private readonly Func<string, TestNode, string, CoverageConfidence, Task<ICoverageRunResult>>? _coverageHandler;

    public TestableRunner(int id, Action onDispose)
        : base(id, new Dictionary<string, List<TestNode>>(),
                new Dictionary<string, MtpTestDescription>(),
                new TestSet(),
                new object(),
                NullLogger.Instance)
    {
        _onDispose = onDispose;
    }

    public TestableRunner(
        int id,
        Dictionary<string, List<TestNode>> testsByAssembly,
        Dictionary<string, MtpTestDescription> testDescriptions,
        TestSet testSet,
        object discoveryLock,
        Action onDispose,
        Func<string, TestNode, string, CoverageConfidence, Task<ICoverageRunResult>>? coverageHandler = null)
        : base(id, testsByAssembly, testDescriptions, testSet, discoveryLock, NullLogger.Instance)
    {
        _onDispose = onDispose;
        _coverageHandler = coverageHandler;
    }

    internal override async Task<ICoverageRunResult> RunSingleTestForCoverageAsync(
        string assembly, TestNode test, string testId, CoverageConfidence confidence)
    {
        if (_coverageHandler is not null)
        {
            return await _coverageHandler(assembly, test, testId, confidence).ConfigureAwait(false);
        }

        return CoverageRunResult.Create(testId, confidence, Array.Empty<int>(), Array.Empty<int>(), Array.Empty<int>());
    }

    public override void Dispose(bool disposing)
    {
        _onDispose?.Invoke();
        base.Dispose(disposing);
    }
}