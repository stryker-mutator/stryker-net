namespace Stryker.Core.Options
{
    public enum StrykerInput
    {
        DevMode,

        BasePath,
        SolutionPath,
        OutputPath,

        LogOptionToFileInput,
        LogOptionLevelInput,

        MutationLevel,

        ThresholdsHigh,
        ThresholdsLow,
        ThresholdsBreak,

        BaselineProvider,
        Reporters,
        ProjectUnderTestNameFilter,
        DiffEnabled,
        CompareToDashboard,
        GitDiffTarget,
        AdditionalTimeoutMs,
        ExcludedMutators,
        IgnoredMethods,
        ConcurrentTestrunners,
        TestRunner,
        Mutate,
        LanguageVersion,
        Optimizations,
        OptimizationMode,
        TestProjects,
        DashboardUrl,
        DashboardApiKey,
        ProjectName,
        ModuleName,
        ProjectVersion,
        DiffIgnoreFilePatterns,
        AzureFileStorageUrl,
        AzureFileStorageSas,
        FallbackVersion,
        SimultaneousTesting,
        AbortOnFail,
    }
}
