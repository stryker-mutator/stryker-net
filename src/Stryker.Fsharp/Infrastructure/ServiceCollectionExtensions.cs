using Microsoft.Extensions.DependencyInjection;
using Stryker.Abstractions;
using Stryker.Core.Initialisation;
using Stryker.Core.MutationTest;

namespace Stryker.Fsharp.Infrastructure;

public static class ServiceCollectionExtensions
{
    /// <summary>Registers the F# implementations under <see cref="Language.Fsharp"/>. Call after AddStrykerCore.</summary>
    public static IServiceCollection AddStrykerFsharp(this IServiceCollection services)
    {
        services.AddKeyedTransient<IMutationProcess, FsharpMutationProcess>(Language.Fsharp);
        services.AddKeyedSingleton<ProjectComponentsBuilderFactory>(Language.Fsharp,
            (_, _) => (projectInfo, _, _, logger, fileSystem) => new FsharpProjectComponentsBuilder(projectInfo, logger, fileSystem));

        return services;
    }
}
