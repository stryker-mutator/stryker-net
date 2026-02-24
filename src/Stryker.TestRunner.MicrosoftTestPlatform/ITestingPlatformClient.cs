using Stryker.TestRunner.MicrosoftTestPlatform.Models;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

/// <summary>
/// Abstracts the TestingPlatformClient for testability of AssemblyTestServer.
/// </summary>
public interface ITestingPlatformClient : IDisposable
{
    Task<InitializeResponse> InitializeAsync();
    Task ExitAsync(bool gracefully = true);
    Task<int> WaitServerProcessExitAsync();
    Task<ResponseListener> DiscoverTestsAsync(Guid requestId, Func<TestNodeUpdate[], Task> action, bool @checked = true);
    Task<ResponseListener> RunTestsAsync(Guid requestId, Func<TestNodeUpdate[], Task> action, TestNode[]? testNodes = null);
}
