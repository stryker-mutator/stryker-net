using System.Reflection;
using Microsoft.Testing.Platform.Builder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.TestRunners.MsTest.Discovery.FrameworkAdapters;
internal class MsTestAdapter : IFrameworkAdapter
{
    public const string EntryPoint = "MSTest";
    private MsTestAdapter() { }
    public static MsTestAdapter Create() => new();

    public async Task<ITestApplication> Build(Assembly assembly, string[] arguments, IEnumerable<string>? activeMutations)
    {
        var builder = await TestApplication.CreateBuilderAsync(arguments);
        builder.AddMSTest(() => new[] { assembly });

        if (activeMutations is not null)
        {
            // Set mutations
        }

        builder.TestHost.AddDataConsumer(_ => TestDataConsumer.Create());
        return await builder.BuildAsync();
    }

    public async Task<int> Run(ITestApplication testApplication) => await testApplication.RunAsync();
}
