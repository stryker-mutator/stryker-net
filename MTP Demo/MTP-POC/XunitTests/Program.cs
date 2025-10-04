using TestProject1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Testing.Platform.Services;
using Shared;
using Xunit.Runner.InProc.SystemConsole.TestingPlatform;

public class Program
{
    public static int Main(string[] args)
    {
        return TestPlatformTestFramework.RunAsync(
                args,
                (builder, argv) =>
                {
                    // Registratie zoals xunit dat doet (auto generated entrypoint)
                    builder.AddSelfRegisteredExtensions(argv);

                    // Eigen registratie
                    builder.TestHostControllers.AddEnvironmentVariableProvider(sp => ActivatorUtilities.CreateInstance<StrykerEnvironmentVariableProvider>(sp));
                    builder.TestHostControllers.AddProcessLifetimeHandler(sp => ActivatorUtilities.CreateInstance<StykerProcessLifeTimeHandler>(sp));

                    builder.TestHost.AddDataConsumer(sp =>
                        new StrykerDataConsumer(sp.GetOutputDevice())
                    );

                })
            .GetAwaiter().GetResult();
    }
}
