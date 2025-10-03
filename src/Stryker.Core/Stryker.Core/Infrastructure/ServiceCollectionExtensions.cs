using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stryker.Core.Helpers.ProcessUtil;
using Stryker.Core.Initialisation;
using Stryker.Core.MutationTest;
using Stryker.Core.Reporters;

namespace Stryker.Core.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStrykerCore(this IServiceCollection services)
    {
        // Add logging support (providers configured by caller)
        services.AddLogging();

        // Core orchestration - Transient as they manage per-run state
        services.AddTransient<IStrykerRunner, StrykerRunner>();
        services.AddTransient<IProjectOrchestrator, ProjectOrchestrator>();
        services.AddTransient<IProjectMutator, ProjectMutator>();
        
        // Initialisation services - Transient as they perform per-run operations
        services.AddTransient<IInitialisationProcess, InitialisationProcess>();
        services.AddTransient<IInitialBuildProcess, InitialBuildProcess>();
        services.AddTransient<IInitialTestProcess, InitialTestProcess>();
        services.AddTransient<IInputFileResolver, InputFileResolver>();
        
        // Helpers and utilities - Transient or Singleton based on state
        services.AddTransient<IProcessExecutor, ProcessExecutor>();
        services.AddSingleton<IFileSystem, FileSystem>();
        
        // Reporter factory - Singleton as it's stateless
        services.AddSingleton<IReporterFactory, ReporterFactory>();

        return services;
    }
}
