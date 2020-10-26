namespace Stryker.Core.Options
{
    public enum StrykerInput
    {
        None,
        DevMode,

        BasePath,
        SolutionPath,

        LogToFile,
        LogLevel,

        MutationLevel,

        ThresholdHigh,
        ThresholdLow,
        ThresholdBreak,

        BaselineProvider,
        Reporters,
        ProjectUnderTestName,
        DiffCompare,
        DashboardCompare,
        GitDiffTarget,
        AdditionalTimeoutMs,
        ExcludedMutators,
        IgnoredMethods,
        Concurrency,
        TestRunner,
        Mutate,
        LanguageVersion,

        OptimizationMode,
        AbortTestOnFail,
        DisableSimultaneousTesting,

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
    }
}
