using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Testing;
using Stryker.Core.Helpers.ProcessUtil;
using Stryker.Core.Initialisation;
using Stryker.Core.MutationTest;
using Stryker.Core.Reporters;
using Stryker.TestRunner.VsTest;
using Stryker.Utilities.Buildalyzer;

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
        services.AddTransient<IMutationTestExecutor, MutationTestExecutor>();

        // Mutation test process - Scoped as they manage per-project state
        services.AddScoped<IMutationTestProcess, MutationTestProcess>();
        services.AddScoped<IInitialisationProcess, InitialisationProcess>();

        // Initialisation services - Transient as they perform per-run operations
        services.AddTransient<IInitialBuildProcess, InitialBuildProcess>();
        services.AddTransient<IInitialTestProcess, InitialTestProcess>();
        services.AddTransient<IInputFileResolver, InputFileResolver>();
        services.AddTransient<INugetRestoreProcess, NugetRestoreProcess>();

        // Testing (use vstest by default for now)
        services.AddTransient<ITestRunner, VsTestRunnerPool>();
        
        // Helpers and utilities - Transient or Singleton based on state
        services.AddTransient<IProcessExecutor, ProcessExecutor>();
        services.AddTransient<IBuildalyzerProvider, BuildalyzerProvider>();
        services.AddSingleton<IFileSystem, FileSystem>();
        
        // Reporter factory - Singleton as it's stateless
        services.AddSingleton<IReporterFactory, ReporterFactory>();

        return services;
    }
}
