using Microsoft.Testing.Platform.Extensions.TestHost;

internal class TestApplicationLifecycleCallbacks : ITestHostApplicationLifetime
{
    public TestApplicationLifecycleCallbacks(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public string Uid => throw new NotImplementedException();

    public string Version => throw new NotImplementedException();

    public string DisplayName => throw new NotImplementedException();

    public string Description => throw new NotImplementedException();

    public IServiceProvider ServiceProvider { get; }

    public async Task AfterRunAsync(int exitCode, CancellationToken cancellation)
    {
        Console.WriteLine("AfterRunAsync");
    }

    public async Task BeforeRunAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("BeforeRunAsync");
    }

    public Task<bool> IsEnabledAsync()
    {
        throw new NotImplementedException();
    }
}
