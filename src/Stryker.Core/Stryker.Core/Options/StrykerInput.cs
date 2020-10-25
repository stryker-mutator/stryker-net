namespace Stryker.Core.Options
{
    public enum StrykerInput
    {
        None,
        DevMode,

        BasePath,
        OutputPath,
        SolutionPath,

        LogOptionToFileInput,
        LogOptionLevelInput,

        MutationLevel,

        ThresholdsHigh,
        ThresholdsLow,
        ThresholdsBreak,

        BaselineProvider,
        Reporters,
        ProjectUnderTestName,
        DiffEnabled,
        DashboardCompareEnabled,
        GitDiffTarget,
        AdditionalTimeoutMs,
        ExcludedMutators,
        IgnoredMethods,
        Concurrency,
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
        AbortTestOnFail,
        DisableSimultaneousTesting,
    }
}
