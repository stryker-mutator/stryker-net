using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Testing.Platform;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.TestHostControllers;

string testAssemblyPath = Path.GetFullPath("C:\\Dev\\Repos\\stryker-net\\integrationtest\\TargetProjects\\NetCore\\NetCoreTestProject.XUnit\\bin\\Debug\\net8.0\\NetCoreTestProject.XUnit.dll");
var exits = File.Exists(testAssemblyPath);
if (!exits)
{
    throw new FileNotFoundException($"Test assembly not found at path: {testAssemblyPath}");
}
// this works, as long as the target framework is the same
var testAssembly = Assembly.LoadFrom(testAssemblyPath);

var testApplicationBuilder = await TestApplication.CreateBuilderAsync(args);
// Register the testing framework
testApplicationBuilder.AddMSTest(() => new[] { testAssembly });
testApplicationBuilder.TestHostControllers.AddEnvironmentVariableProvider(x => new TestHostEnvironmentVariableProvider(x));
//var factory = new CompositeExtensionFactory<StrykerExtension>(serviceProvider => new StrykerExtension());
//testApplicationBuilder.TestHost.AddDataConsumer(factory);
var testApplication = await testApplicationBuilder.BuildAsync();
var result = await testApplication.RunAsync();

Console.WriteLine($"Test run succeeded: {result == 0}");

public class TestHostEnvironmentVariableProvider : ITestHostEnvironmentVariableProvider
{
    public TestHostEnvironmentVariableProvider(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public IServiceProvider ServiceProvider { get; }

    public string Uid => throw new NotImplementedException();

    public string Version => throw new NotImplementedException();

    public string DisplayName => throw new NotImplementedException();

    public string Description => throw new NotImplementedException();

    public Task<bool> IsEnabledAsync() => throw new NotImplementedException();
    public Task UpdateAsync(IEnvironmentVariables environmentVariables) => throw new NotImplementedException();
    public Task<ValidationResult> ValidateTestHostEnvironmentVariablesAsync(IReadOnlyEnvironmentVariables environmentVariables) => throw new NotImplementedException();
}
