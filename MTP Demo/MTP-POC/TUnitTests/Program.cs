using Microsoft.Extensions.DependencyInjection;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Services;
using TestProject1;
using TUnit.Engine.Extensions;

var testApplicationBuilder = await TestApplication.CreateBuilderAsync(args);

// Register the testing framework
testApplicationBuilder.AddTUnit();

testApplicationBuilder.TestHostControllers.AddEnvironmentVariableProvider(sp => ActivatorUtilities.CreateInstance<StrykerEnvironmentVariableProvider>(sp));
testApplicationBuilder.TestHostControllers.AddProcessLifetimeHandler(sp => ActivatorUtilities.CreateInstance<StykerProcessLifeTimeHandler>(sp));

testApplicationBuilder.TestHost.AddDataConsumer(sp =>
    ActivatorUtilities.CreateInstance<StrykerDataConsumer>(sp, sp.GetOutputDevice())
);

var app = await testApplicationBuilder.BuildAsync();

return await app.RunAsync();