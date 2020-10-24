using McMaster.Extensions.CommandLineUtils;
using Stryker.Core.Options;
using Stryker.Core.Options.Options;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.CLI
{
    public static class CLIOptions
    {
        public static readonly CLIOption<string> ConfigFilePath = new CLIOption<string>
        {
            ArgumentName = "--config-file-path",
            ArgumentShortName = "-cp <path>",
            ArgumentDescription = "Sets the config-file-path relative to current workingDirectory | stryker-config.json (default)",
            DefaultValue = "stryker-config.json"
        };

        public static readonly CLIOption<string[]> Reporters = new CLIOption<string[]>
        {
            ArgumentName = "--reporters",
            ArgumentShortName = "-r <reporters>",
            ArgumentDescription = ReportersOption.HelpText,
            DefaultValue = ReportersOption.DefaultValue.Select(r => r.ToString()).ToArray(),
            JsonKey = "reporters"
        };

        public static readonly CLIOption<string> LogLevel = new CLIOption<string>
        {
            ArgumentName = "--log-level",
            ArgumentShortName = "-l <logLevel>",
            ArgumentDescription = LogOptionsOption.HelpText,
            DefaultValue = "info",
            JsonKey = "log-level"
        };

        public static readonly CLIOption<bool> LogToFile = new CLIOption<bool>
        {
            ArgumentName = "--log-file",
            ArgumentShortName = "-f",
            ArgumentDescription = LogOptionsOption.HelpText,
            DefaultValue = LogOptionsOption.DefaultValue.LogToFile,
            ValueType = CommandOptionType.NoValue,
            JsonKey = "log-file"
        };

        public static readonly CLIOption<bool> DevMode = new CLIOption<bool>
        {
            ArgumentName = "--dev-mode",
            ArgumentShortName = "-dev",
            ArgumentDescription = DevModeOption.HelpText,
            DefaultValue = DevModeOption.DefaultValue,
            ValueType = CommandOptionType.NoValue,
            JsonKey = "dev-mode"
        };

        public static readonly CLIOption<int> AdditionalTimeoutMS = new CLIOption<int>
        {
            ArgumentName = "--timeout-ms",
            ArgumentShortName = "-t <ms>",
            ArgumentDescription = AdditionalTimeoutMsOption.HelpText,
            DefaultValue = AdditionalTimeoutMsOption.DefaultValue,
            JsonKey = "timeout-ms"
        };

        public static readonly CLIOption<IEnumerable<string>> ExcludedMutations = new CLIOption<IEnumerable<string>>
        {
            ArgumentName = "--excluded-mutations",
            ArgumentShortName = "-em <mutators>",
            ArgumentDescription = ExcludedMutatorsOption.HelpText,
            DefaultValue = Enumerable.Empty<string>(),
            JsonKey = "excluded-mutations"
        };

        public static readonly CLIOption<IEnumerable<string>> IgnoreMethods = new CLIOption<IEnumerable<string>>
        {
            ArgumentName = "--ignore-methods",
            ArgumentShortName = "-im <methodNames>",
            ArgumentDescription = IgnoredMethodsOption.HelpText,
            DefaultValue = Enumerable.Empty<string>(),
            JsonKey = "ignore-methods"
        };

        public static readonly CLIOption<string> ProjectFileName = new CLIOption<string>
        {
            ArgumentName = "--project-file",
            ArgumentShortName = "-p <projectFileName>",
            ArgumentDescription = ProjectUnderTestNameFilterOption.HelpText,
            DefaultValue = ProjectUnderTestNameFilterOption.DefaultValue,
            JsonKey = "project-file"
        };

        public static readonly CLIOption<bool> Diff = new CLIOption<bool>
        {
            ArgumentName = "--diff",
            ArgumentShortName = "-diff",
            ArgumentDescription = DiffEnabledOption.HelpText,
            DefaultValue = DiffEnabledOption.DefaultValue,
            ValueType = CommandOptionType.NoValue,
            JsonKey = "diff"
        };

        public static readonly CLIOption<bool> DashboardCompare = new CLIOption<bool>
        {
            ArgumentName = "--dashboard-compare",
            ArgumentShortName = "-compare",
            ArgumentDescription = CompareToDashboardOption.HelpText,
            DefaultValue = CompareToDashboardOption.DefaultValue,
            ValueType = CommandOptionType.NoValue,
            JsonKey = "dashboard-compare"

        };

        public static readonly CLIOption<IEnumerable<string>> DiffIgnoreFiles = new CLIOption<IEnumerable<string>>
        {
            ArgumentName = "--diff-ignore-files",
            ArgumentShortName = "-diffignorefiles",
            ArgumentDescription = DiffIgnoreFilePatternsOption.HelpText,
            DefaultValue = Enumerable.Empty<string>(),
            JsonKey = "diff-ignore-files"
        };


        public static readonly CLIOption<string> BaselineStorageLocation = new CLIOption<string>
        {
            ArgumentName = "--baseline-storage-location",
            ArgumentShortName = "-bsl <storageLocation>",
            ArgumentDescription = BaselineProviderOption.HelpText,
            DefaultValue = BaselineProviderOption.DefaultValue.ToString(),
            JsonKey = "baseline-storage-location"
        };

        public static readonly CLIOption<string> GitDiffTarget = new CLIOption<string>
        {
            ArgumentName = "--git-diff-target",
            ArgumentShortName = "-gdt <commitish>",
            ArgumentDescription = GitDiffTargetOption.HelpText,
            DefaultValue = GitDiffTargetOption.DefaultValue,
            JsonKey = "git-diff-target"
        };

        public static readonly CLIOption<string> CoverageAnalysis = new CLIOption<string>
        {
            ArgumentName = "--coverage-analysis",
            ArgumentShortName = "-ca <mode>",
            ArgumentDescription = OptimizationModeOption.HelpText,
            DefaultValue = OptimizationModeOption.DefaultValue,
            JsonKey = "coverage-analysis"
        };

        public static readonly CLIOption<bool> AbortTestOnFail = new CLIOption<bool>
        {
            ArgumentName = "--abort-test-on-fail",
            ArgumentShortName = "-atof",
            ArgumentDescription = OptimizationsOption.HelpText,
            DefaultValue = OptimizationsOption.DefaultValue.HasFlag(OptimizationFlags.AbortTestOnKill),
            ValueType = CommandOptionType.NoValue,
            JsonKey = "abort-test-on-fail"
        };

        public static readonly CLIOption<bool> DisableTestingMix = new CLIOption<bool>
        {
            ArgumentName = "--disable-testing-mix-mutations",
            ArgumentShortName = "-tmm",
            ArgumentDescription = OptimizationsOption.HelpText,
            DefaultValue = OptimizationsOption.DefaultValue.HasFlag(OptimizationFlags.DisableTestMix),
            ValueType = CommandOptionType.NoValue,
            JsonKey = "disable-testing-mix-mutations"
        };

        public static readonly CLIOption<int> MaxConcurrentTestRunners = new CLIOption<int>
        {
            ArgumentName = "--max-concurrent-test-runners",
            ArgumentShortName = "-c <integer>",
            ArgumentDescription = ConcurrentTestrunnersOption.HelpText,
            DefaultValue = ConcurrentTestrunnersOption.DefaultValue,
            JsonKey = "max-concurrent-test-runners"
        };

        public static readonly CLIOption<int> ThresholdBreak = new CLIOption<int>
        {
            ArgumentName = "--threshold-break",
            ArgumentShortName = "-tb <thresholdBreak>",
            ArgumentDescription = ThresholdsOption.HelpText,
            DefaultValue = ThresholdsOption.DefaultValue.Break,
            JsonKey = "threshold-break"
        };

        public static readonly CLIOption<int> ThresholdLow = new CLIOption<int>
        {
            ArgumentName = "--threshold-low",
            ArgumentShortName = "-tl <thresholdLow>",
            ArgumentDescription = ThresholdsOption.HelpText,
            DefaultValue = ThresholdsOption.DefaultValue.Low,
            JsonKey = "threshold-low"
        };

        public static readonly CLIOption<int> ThresholdHigh = new CLIOption<int>
        {
            ArgumentName = "--threshold-high",
            ArgumentShortName = "-th <thresholdHigh>",
            ArgumentDescription = ThresholdsOption.HelpText,
            DefaultValue = ThresholdsOption.DefaultValue.High,
            JsonKey = "threshold-high"
        };

        public static readonly CLIOption<IEnumerable<string>> Mutate = new CLIOption<IEnumerable<string>>
        {
            ArgumentName = "--mutate",
            ArgumentShortName = "-m <file-patterns>",
            ArgumentDescription = MutateOption.HelpText,
            DefaultValue = Enumerable.Empty<string>(),
            JsonKey = "mutate",
        };

        public static readonly CLIOption<string> SolutionPath = new CLIOption<string>
        {
            ArgumentName = "--solution-path",
            ArgumentShortName = "-s <path>",
            ArgumentDescription = SolutionPathOption.HelpText,
            DefaultValue = SolutionPathOption.DefaultValue,
            JsonKey = "solution-path"
        };

        public static readonly CLIOption<string> TestRunner = new CLIOption<string>
        {
            ArgumentName = "--test-runner",
            ArgumentShortName = "-tr <testRunner>",
            ArgumentDescription = TestRunnerOption.HelpText,
            DefaultValue = TestRunnerOption.DefaultValue.ToString(),
            JsonKey = "test-runner"
        };

        public static readonly CLIOption<string> LangVersion = new CLIOption<string>
        {
            ArgumentName = "--language-version",
            ArgumentShortName = "-lv <csharp-version-name>",
            ArgumentDescription = LanguageVersionOption.HelpText,
            DefaultValue = LanguageVersionOption.DefaultValue.ToString(),
            JsonKey = "language-version"
        };

        public static readonly CLIOption<string> DashboardApiKey = new CLIOption<string>
        {
            ArgumentName = "--dashboard-api-key",
            ArgumentShortName = "-dk <api-key>",
            ArgumentDescription = DashboardApiKeyOption.HelpText,
            DefaultValue = DashboardApiKeyOption.DefaultValue,
            JsonKey = "dashboard-api-key"
        };

        public static readonly CLIOption<string> DashboardProjectName = new CLIOption<string>
        {
            ArgumentName = "--dashboard-project",
            ArgumentShortName = "-project <name>",
            ArgumentDescription = ProjectNameOption.HelpText,
            DefaultValue = ProjectNameOption.DefaultValue,
            JsonKey = "dashboard-project"
        };

        public static readonly CLIOption<string> DashboardModuleName = new CLIOption<string>
        {
            ArgumentName = "--dashboard-module",
            ArgumentShortName = "-module <name>",
            ArgumentDescription = ModuleNameOption.HelpText,
            DefaultValue = ModuleNameOption.DefaultValue,
            JsonKey = "dashboard-module"
        };

        public static readonly CLIOption<string> DashboardProjectVersion = new CLIOption<string>
        {
            ArgumentName = "--dashboard-version",
            ArgumentShortName = "-version <version>",
            ArgumentDescription = ProjectVersionOption.HelpText,
            DefaultValue = ProjectVersionOption.DefaultValue,
            JsonKey = "dashboard-version"
        };

        public static readonly CLIOption<string> DashboardFallbackVersion = new CLIOption<string>
        {
            ArgumentName = "--dashboard-fallback-version",
            ArgumentShortName = "-fallback-version <version>",
            ArgumentDescription = FallbackVersionOption.HelpText,
            DefaultValue = FallbackVersionOption.DefaultValue,
            JsonKey = "dashboard-fallback-version"
        };

        public static readonly CLIOption<string> DashboardUrl = new CLIOption<string>
        {
            ArgumentName = "--dashboard-url",
            ArgumentShortName = "-url <dashboard-url>",
            ArgumentDescription = DashboardUrlOption.HelpText,
            DefaultValue = DashboardUrlOption.DefaultValue,
            JsonKey = "dashboard-url"
        };

        public static readonly CLIOption<IEnumerable<string>> TestProjects = new CLIOption<IEnumerable<string>>
        {
            ArgumentName = "--test-projects",
            ArgumentShortName = "-tp",
            ArgumentDescription = TestProjectsOption.HelpText,
            DefaultValue = TestProjectsOption.DefaultValue,
            JsonKey = "test-projects"
        };

        public static readonly CLIOption<string> AzureSAS = new CLIOption<string>
        {
            ArgumentName = "--azure-storage-sas",
            ArgumentShortName = "-sas <azure-sas-key>",
            ArgumentDescription = AzureFileStorageSasOption.HelpText,
            DefaultValue = AzureFileStorageSasOption.DefaultValue,
            JsonKey = "azure-storage-sas"
        };

        public static readonly CLIOption<string> AzureFileStorageUrl = new CLIOption<string>
        {
            ArgumentName = "--azure-storage-url",
            ArgumentShortName = "-storage-url <url>",
            ArgumentDescription = AzureFileStorageUrlOption.HelpText,
            DefaultValue = AzureFileStorageUrlOption.DefaultValue,
            JsonKey = "azure-storage-url"
        };

        public static readonly CLIOption<string> MutationLevel = new CLIOption<string>
        {
            ArgumentName = "--mutation-level",
            ArgumentShortName = "-level",
            ArgumentDescription = MutationLevelOption.HelpText,
            DefaultValue = MutationLevelOption.DefaultValue.ToString(),
            JsonKey = "mutation-level"
        };

        //private static string FormatOptionsString<T, Y>(T @default, IEnumerable<Y> options)
        //{
        //    return FormatOptionsString(new List<T> { @default }, options, new List<Y>());
        //}

        //private static string FormatOptionsString<T, Y>(IEnumerable<T> @default, IEnumerable<Y> options, IEnumerable<Y> deprecated)
        //{
        //    StringBuilder optionsString = new StringBuilder();

        //    optionsString.Append($"Options[ (default)[ {string.Join(", ", @default)} ], ");
        //    string nonDefaultOptions = string.Join(
        //    ", ",
        //    options
        //    .Where(o => !@default.Any(d => d.ToString() == o.ToString()))
        //    .Where(o => !deprecated.Any(d => d.ToString() == o.ToString())));

        //    string deprecatedOptions = "";
        //    if (deprecated.Any())
        //    {
        //        deprecatedOptions = "(deprecated) " + string.Join(", (deprecated) ", options.Where(o => deprecated.Any(d => d.ToString() == o.ToString())));
        //    }

        //    optionsString.Append(string.Join(", ", nonDefaultOptions, deprecatedOptions));
        //    optionsString.Append(" ]");

        //    return optionsString.ToString();
        //}
    }
}
