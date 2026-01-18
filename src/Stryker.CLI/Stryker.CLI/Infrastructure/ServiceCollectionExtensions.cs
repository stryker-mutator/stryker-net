using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Stryker.CLI.Clients;
using Stryker.CLI.Logging;

namespace Stryker.CLI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStrykerCli(this IServiceCollection services)
    {
        // CLI services
        services.AddTransient<StrykerCli>();
        services.AddSingleton<IConfigBuilder, ConfigBuilder>();
        services.AddSingleton<ILoggingInitializer, LoggingInitializer>();
        services.AddSingleton<IStrykerNugetFeedClient, StrykerNugetFeedClient>();
        services.AddSingleton(_ => AnsiConsole.Console);
        services.AddSingleton<IFileSystem, FileSystem>();

        return services;
    }
}
