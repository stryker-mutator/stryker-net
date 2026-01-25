using Microsoft.Extensions.Logging.Abstractions;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.Tests;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

internal class TestableRunner : SingleMicrosoftTestPlatformRunner
{
    private readonly Action _onDispose;

    public TestableRunner(int id, Action onDispose) 
        : base(id, new Dictionary<string, List<TestNode>>(), 
                new Dictionary<string, MtpTestDescription>(), 
                new TestSet(), 
                new object(), 
                NullLogger.Instance)
    {
        _onDispose = onDispose;
    }

    public override void Dispose()
    {
        _onDispose?.Invoke();
        base.Dispose();
    }
}