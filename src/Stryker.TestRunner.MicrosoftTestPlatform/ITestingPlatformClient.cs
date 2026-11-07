using Stryker.TestRunner.MicrosoftTestPlatform.Models;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

/// <summary>
/// Abstracts the TestingPlatformClient for testability of AssemblyTestServer.
/// </summary>
/// <remarks>
/// Result callbacks are ordered and awaited, but must not await another request on the same client.
/// A cancelled request does not establish server quiescence. Its host must be discarded before reuse.
/// </remarks>
public interface ITestingPlatformClient : IDisposable
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task ExitAsync(bool gracefully = true);
    Task<int> WaitServerProcessExitAsync();
    Task DiscoverTestsAsync(Func<TestNodeUpdate[], Task> action, CancellationToken cancellationToken = default);
    Task RunTestsAsync(Func<TestNodeUpdate[], Task> action, TestNode[]? testNodes = null, CancellationToken cancellationToken = default);
}
