

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Services;
using Shared;
using TestProject1;

var testApplicationBuilder = await TestApplication.CreateBuilderAsync(args);

// Register the testing framework
testApplicationBuilder.AddMSTest(() => [Assembly.GetExecutingAssembly()]);

testApplicationBuilder.TestHostControllers.AddEnvironmentVariableProvider(sp => ActivatorUtilities.CreateInstance<StrykerEnvironmentVariableProvider>(sp));
testApplicationBuilder.TestHostControllers.AddProcessLifetimeHandler(sp => ActivatorUtilities.CreateInstance<StykerProcessLifeTimeHandler>(sp));

testApplicationBuilder.TestHost.AddDataConsumer(sp =>
    ActivatorUtilities.CreateInstance<StrykerDataConsumer>(sp, sp.GetOutputDevice())
);

var app = await testApplicationBuilder.BuildAsync();

return await app.RunAsync();
