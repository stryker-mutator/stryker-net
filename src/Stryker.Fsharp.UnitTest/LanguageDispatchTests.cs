using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Core.Infrastructure;
using Stryker.Core.MutationTest;
using Stryker.Fsharp.Infrastructure;

namespace Stryker.Fsharp.UnitTest;

[TestClass]
public class LanguageDispatchTests
{
    private static ServiceProvider BuildProvider() =>
        new ServiceCollection().AddStrykerCore().AddStrykerFsharp().BuildServiceProvider(validateScopes: true);

    [TestMethod]
    public void ServiceProvider_FsharpProject_ResolvesFsharpMutationProcess()
    {
        using var provider = BuildProvider();

        provider.GetRequiredKeyedService<IMutationProcess>(Language.Fsharp).ShouldBeOfType<FsharpMutationProcess>();
    }

    [TestMethod]
    public void ServiceProvider_CsharpProject_ResolvesCsharpMutationProcess()
    {
        using var provider = BuildProvider();

        provider.GetRequiredKeyedService<IMutationProcess>(Language.Csharp).ShouldBeOfType<CsharpMutationProcess>();
    }
}
