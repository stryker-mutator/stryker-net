using Microsoft.Testing.Platform.Extensions.TestHost;
using Microsoft.Testing.Platform.TestHost;

namespace Stryker.TestRunner.MSTest.Testing.LifeTimeHandlers;
internal class CoverageLifeTimeHandler : ITestSessionLifetimeHandler
{
    public string Uid => nameof(CoverageLifeTimeHandler);

    public string Version => "1.0.0";

    public string DisplayName => "CoverageLifeTimeHandler";

    public string Description => "Handler for coverage";

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);
    public Task OnTestSessionFinishingAsync(SessionUid sessionUid, CancellationToken cancellationToken)
    {

        var t = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.FullName.Contains("Project", StringComparison.OrdinalIgnoreCase));


        return Task.FromResult(true);
    }
    public Task OnTestSessionStartingAsync(SessionUid sessionUid, CancellationToken cancellationToken)
    {
        var t = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.FullName.Contains("Project", StringComparison.OrdinalIgnoreCase));


        return Task.FromResult(true);
    }
}
