using Microsoft.Testing.Platform.Extensions.TestHostControllers;

namespace TestProject1;

public class StykerProcessLifeTimeHandler : ITestHostProcessLifetimeHandler
{

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

    public string Uid => "StykerProcessLifeTimeHandler";
    public string Version => "1.0";
    public string DisplayName => "Styker Process Lifetime Handler";
    public string Description => "Handles the lifetime of the test host process for Stryker";
    
    public Task BeforeTestHostProcessStartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("BeforeTestHostProcessStartAsync");
        return Task.CompletedTask;
    }

    public Task OnTestHostProcessStartedAsync(ITestHostProcessInformation testHostProcessInformation, CancellationToken cancellation)
    {
        Console.WriteLine("OnTestHostProcessStartedAsync");
        Console.WriteLine("TestHostProcessId: " + testHostProcessInformation.PID);
        return Task.CompletedTask;
    }

    public Task OnTestHostProcessExitedAsync(ITestHostProcessInformation testHostProcessInformation, CancellationToken cancellation)
    {
        Console.WriteLine("OnTestHostProcessExitedAsync");
        Console.WriteLine("TestHostProcessId: " + testHostProcessInformation.PID);
        Console.WriteLine("ExitCode: " + testHostProcessInformation.ExitCode);
        return Task.CompletedTask;
    }
}