using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestHost;
using Microsoft.Testing.Platform.Extensions.TestHostControllers;
using System.Reflection;

namespace UnitTests
{
    public class Injected
    {
        public static async Task Main(string[] args)
        {
            await MainAsync(args);
        }

        public static async Task MainAsync(string[] args)
        {
            Console.WriteLine("Custom stryker testrunner starting...");

            var testAssembly = Assembly.GetExecutingAssembly();

            Console.WriteLine("Starting testrun");
            await RunTestsAsync(args, testAssembly);
        }

        private static async Task RunTestsAsync(string[] args, Assembly testAssembly)
        {
            var testApplicationBuilder = await TestApplication.CreateBuilderAsync(args);
            // Register the testing framework
            testApplicationBuilder.AddMSTest(() => new[] { testAssembly });
            var factory = new CompositeExtensionFactory<StrykerExtension>(serviceProvider => new StrykerExtension());
            testApplicationBuilder.TestHost.AddDataConsumer(factory);
            var testApplication = await testApplicationBuilder.BuildAsync();
            await testApplication.RunAsync();
        }
    }
}

internal class StrykerExtension : IDataConsumer
{
    public StrykerExtension()
    {
    }

    string IExtension.Uid => "StrykerMsTestExtension";

    string IExtension.Version => "1.0";

    string IExtension.DisplayName => "StrykerMsTestExtension";

    string IExtension.Description => "StrykerMsTestExtension";
    public Type[] DataTypesConsumed => new[] { typeof(TestNodeUpdateMessage) };

    public Task ConsumeAsync(
        IDataProducer dataProducer,
        IData value,
        CancellationToken cancellationToken)
    {
        var testNodeUpdateMessage = (TestNodeUpdateMessage)value;

        switch (testNodeUpdateMessage.TestNode.Properties.Single<TestNodeStateProperty>())
        {
            case InProgressTestNodeStateProperty _:
                {
                    //Console.WriteLine("InProgressTestNodeStateProperty");
                    Console.WriteLine("StrykerActiveMutation: " + Environment.GetEnvironmentVariable("StrykerActiveMutation"));
                    break;
                }
            case PassedTestNodeStateProperty passed:
                {
                    //Console.WriteLine("PassedTestNodeStateProperty");
                    //Console.WriteLine(passed.Explanation);
                    break;
                }
            case FailedTestNodeStateProperty failedTestNodeStateProperty:
                {
                    //Console.WriteLine("FailedTestNodeStateProperty");
                    break;
                }
            case SkippedTestNodeStateProperty _:
                {
                    //Console.WriteLine("SkippedTestNodeStateProperty");
                    break;
                }
        }

        return Task.CompletedTask;
    }
    //public Task UpdateAsync(IEnvironmentVariables environmentVariables) => throw new NotImplementedException();
    public Task<ValidationResult> ValidateTestHostEnvironmentVariablesAsync(IReadOnlyEnvironmentVariables environmentVariables) => throw new NotImplementedException();
    Task<bool> IExtension.IsEnabledAsync() => Task.FromResult(true);
    public Task BeforeRunAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task AfterRunAsync(int exitCode, CancellationToken cancellation) => throw new NotImplementedException();
}
