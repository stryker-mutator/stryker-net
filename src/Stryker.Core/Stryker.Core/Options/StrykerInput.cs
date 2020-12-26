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
        DiffTarget,
        AdditionalTimeoutMs,
        ExcludedMutators,
        IgnoredMethods,
        Concurrency,
        TestRunner,
        Mutate,
        LanguageVersion,

        OptimizationMode,
        DisableAbortTestOnFail,
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
