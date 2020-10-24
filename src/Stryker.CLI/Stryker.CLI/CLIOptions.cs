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
            ArgumentDescription = ReportersInput.HelpText,
            DefaultValue = ReportersInput.DefaultValue.Select(r => r.ToString()).ToArray(),
            JsonKey = "reporters"
        };

        public static readonly CLIOption<string> LogLevel = new CLIOption<string>
        {
            ArgumentName = "--log-level",
            ArgumentShortName = "-l <logLevel>",
            ArgumentDescription = LogOptionLevelInput.HelpText,
            DefaultValue = "info",
            JsonKey = "log-level"
        };

        public static readonly CLIOption<bool> LogToFile = new CLIOption<bool>
        {
            ArgumentName = "--log-file",
            ArgumentShortName = "-f",
            ArgumentDescription = LogOptionLevelInput.HelpText,
            DefaultValue = LogOptionLevelInput.DefaultValue.LogToFile,
            ValueType = CommandOptionType.NoValue,
            JsonKey = "log-file"
        };

        public static readonly CLIOption<bool> DevMode = new CLIOption<bool>
        {
            ArgumentName = "--dev-mode",
            ArgumentShortName = "-dev",
            ArgumentDescription = DevModeInput.HelpText,
            DefaultValue = DevModeInput.DefaultValue,
            ValueType = CommandOptionType.NoValue,
            JsonKey = "dev-mode"
        };

        public static readonly CLIOption<int> AdditionalTimeoutMS = new CLIOption<int>
        {
            ArgumentName = "--timeout-ms",
            ArgumentShortName = "-t <ms>",
            ArgumentDescription = AdditionalTimeoutMsInput.HelpText,
            DefaultValue = AdditionalTimeoutMsInput.DefaultValue,
            JsonKey = "timeout-ms"
        };

        public static readonly CLIOption<IEnumerable<string>> ExcludedMutations = new CLIOption<IEnumerable<string>>
        {
            ArgumentName = "--excluded-mutations",
            ArgumentShortName = "-em <mutators>",
            ArgumentDescription = ExcludedMutatorsInput.HelpText,
            DefaultValue = Enumerable.Empty<string>(),
            JsonKey = "excluded-mutations"
        };

        public static readonly CLIOption<IEnumerable<string>> IgnoreMethods = new CLIOption<IEnumerable<string>>
        {
            ArgumentName = "--ignore-methods",
            ArgumentShortName = "-im <methodNames>",
            ArgumentDescription = IgnoredMethodsInput.HelpText,
            DefaultValue = IgnoredMethodsInput.DefaultInput,
            JsonKey = "ignore-methods"
        };

        public static readonly CLIOption<string> ProjectFileName = new CLIOption<string>
        {
            ArgumentName = "--project-file",
            ArgumentShortName = "-p <projectFileName>",
            ArgumentDescription = ProjectUnderTestNameFilterInput.HelpText,
            DefaultValue = ProjectUnderTestNameFilterInput.DefaultValue,
            JsonKey = "project-file"
        };

        public static readonly CLIOption<bool> Diff = new CLIOption<bool>
        {
            ArgumentName = "--diff",
            ArgumentShortName = "-diff",
            ArgumentDescription = DiffEnabledInput.HelpText,
            DefaultValue = DiffEnabledInput.DefaultValue,
            ValueType = CommandOptionType.NoValue,
            JsonKey = "diff"
        };

        public static readonly CLIOption<bool> DashboardCompare = new CLIOption<bool>
        {
            ArgumentName = "--dashboard-compare",
            ArgumentShortName = "-compare",
            ArgumentDescription = CompareToDashboardInput.HelpText,
            DefaultValue = CompareToDashboardInput.DefaultValue,
            ValueType = CommandOptionType.NoValue,
            JsonKey = "dashboard-compare"

        };

        public static readonly CLIOption<IEnumerable<string>> DiffIgnoreFiles = new CLIOption<IEnumerable<string>>
        {
            ArgumentName = "--diff-ignore-files",
            ArgumentShortName = "-diffignorefiles",
            ArgumentDescription = DiffIgnoreFilePatternsInput.HelpText,
            DefaultValue = Enumerable.Empty<string>(),
            JsonKey = "diff-ignore-files"
        };


        public static readonly CLIOption<string> BaselineStorageLocation = new CLIOption<string>
        {
            ArgumentName = "--baseline-storage-location",
            ArgumentShortName = "-bsl <storageLocation>",
            ArgumentDescription = BaselineProviderInput.HelpText,
            DefaultValue = BaselineProviderInput.DefaultValue.ToString(),
            JsonKey = "baseline-storage-location"
        };

        public static readonly CLIOption<string> GitDiffTarget = new CLIOption<string>
        {
            ArgumentName = "--git-diff-target",
            ArgumentShortName = "-gdt <commitish>",
            ArgumentDescription = GitDiffTargetInput.HelpText,
            DefaultValue = GitDiffTargetInput.DefaultValue,
            JsonKey = "git-diff-target"
        };

        public static readonly CLIOption<string> CoverageAnalysis = new CLIOption<string>
        {
            ArgumentName = "--coverage-analysis",
            ArgumentShortName = "-ca <mode>",
            ArgumentDescription = OptimizationModeInput.HelpText,
            DefaultValue = OptimizationModeInput.DefaultValue,
            JsonKey = "coverage-analysis"
        };

        public static readonly CLIOption<bool> AbortTestOnFail = new CLIOption<bool>
        {
            ArgumentName = "--abort-test-on-fail",
            ArgumentShortName = "-atof",
            ArgumentDescription = OptimizationsInput.HelpText,
            DefaultValue = OptimizationsInput.DefaultValue.HasFlag(OptimizationFlags.AbortTestOnKill),
            ValueType = CommandOptionType.NoValue,
            JsonKey = "abort-test-on-fail"
        };

        public static readonly CLIOption<bool> DisableTestingMix = new CLIOption<bool>
        {
            ArgumentName = "--disable-testing-mix-mutations",
            ArgumentShortName = "-tmm",
            ArgumentDescription = OptimizationsInput.HelpText,
            DefaultValue = OptimizationsInput.DefaultValue.HasFlag(OptimizationFlags.DisableTestMix),
            ValueType = CommandOptionType.NoValue,
            JsonKey = "disable-testing-mix-mutations"
        };

        public static readonly CLIOption<int> MaxConcurrentTestRunners = new CLIOption<int>
        {
            ArgumentName = "--max-concurrent-test-runners",
            ArgumentShortName = "-c <integer>",
            ArgumentDescription = ConcurrentTestrunnersInput.HelpText,
            DefaultValue = ConcurrentTestrunnersInput.DefaultValue,
            JsonKey = "max-concurrent-test-runners"
        };

        public static readonly CLIOption<int> ThresholdBreak = new CLIOption<int>
        {
            ArgumentName = "--threshold-break",
            ArgumentShortName = "-tb <thresholdBreak>",
            ArgumentDescription = ThresholdsHighInput.HelpText,
            DefaultValue = ThresholdsHighInput.DefaultValue.Break,
            JsonKey = "threshold-break"
        };

        public static readonly CLIOption<int> ThresholdLow = new CLIOption<int>
        {
            ArgumentName = "--threshold-low",
            ArgumentShortName = "-tl <thresholdLow>",
            ArgumentDescription = ThresholdsHighInput.HelpText,
            DefaultValue = ThresholdsHighInput.DefaultValue.Low,
            JsonKey = "threshold-low"
        };

        public static readonly CLIOption<int> ThresholdHigh = new CLIOption<int>
        {
            ArgumentName = "--threshold-high",
            ArgumentShortName = "-th <thresholdHigh>",
            ArgumentDescription = ThresholdsHighInput.HelpText,
            DefaultValue = ThresholdsHighInput.DefaultValue.High,
            JsonKey = "threshold-high"
        };

        public static readonly CLIOption<IEnumerable<string>> Mutate = new CLIOption<IEnumerable<string>>
        {
            ArgumentName = "--mutate",
            ArgumentShortName = "-m <file-patterns>",
            ArgumentDescription = MutateInput.HelpText,
            DefaultValue = Enumerable.Empty<string>(),
            JsonKey = "mutate",
        };

        public static readonly CLIOption<string> SolutionPath = new CLIOption<string>
        {
            ArgumentName = "--solution-path",
            ArgumentShortName = "-s <path>",
            ArgumentDescription = SolutionPathInput.HelpText,
            DefaultValue = SolutionPathInput.DefaultValue,
            JsonKey = "solution-path"
        };

        public static readonly CLIOption<string> TestRunner = new CLIOption<string>
        {
            ArgumentName = "--test-runner",
            ArgumentShortName = "-tr <testRunner>",
            ArgumentDescription = TestRunnerInput.HelpText,
            DefaultValue = TestRunnerInput.DefaultValue.ToString(),
            JsonKey = "test-runner"
        };

        public static readonly CLIOption<string> LangVersion = new CLIOption<string>
        {
            ArgumentName = "--language-version",
            ArgumentShortName = "-lv <csharp-version-name>",
            ArgumentDescription = LanguageVersionInput.HelpText,
            DefaultValue = LanguageVersionInput.DefaultValue.ToString(),
            JsonKey = "language-version"
        };

        public static readonly CLIOption<string> DashboardApiKey = new CLIOption<string>
        {
            ArgumentName = "--dashboard-api-key",
            ArgumentShortName = "-dk <api-key>",
            ArgumentDescription = DashboardApiKeyInput.HelpText,
            DefaultValue = DashboardApiKeyInput.DefaultValue,
            JsonKey = "dashboard-api-key"
        };

        public static readonly CLIOption<string> DashboardProjectName = new CLIOption<string>
        {
            ArgumentName = "--dashboard-project",
            ArgumentShortName = "-project <name>",
            ArgumentDescription = ProjectNameInput.HelpText,
            DefaultValue = ProjectNameInput.DefaultValue,
            JsonKey = "dashboard-project"
        };

        public static readonly CLIOption<string> DashboardModuleName = new CLIOption<string>
        {
            ArgumentName = "--dashboard-module",
            ArgumentShortName = "-module <name>",
            ArgumentDescription = ModuleNameInput.HelpText,
            DefaultValue = ModuleNameInput.DefaultValue,
            JsonKey = "dashboard-module"
        };

        public static readonly CLIOption<string> DashboardProjectVersion = new CLIOption<string>
        {
            ArgumentName = "--dashboard-version",
            ArgumentShortName = "-version <version>",
            ArgumentDescription = ProjectVersionInput.HelpText,
            DefaultValue = ProjectVersionInput.DefaultValue,
            JsonKey = "dashboard-version"
        };

        public static readonly CLIOption<string> DashboardFallbackVersion = new CLIOption<string>
        {
            ArgumentName = "--dashboard-fallback-version",
            ArgumentShortName = "-fallback-version <version>",
            ArgumentDescription = FallbackVersionInput.HelpText,
            DefaultValue = FallbackVersionInput.DefaultValue,
            JsonKey = "dashboard-fallback-version"
        };

        public static readonly CLIOption<string> DashboardUrl = new CLIOption<string>
        {
            ArgumentName = "--dashboard-url",
            ArgumentShortName = "-url <dashboard-url>",
            ArgumentDescription = DashboardUrlInput.HelpText,
            DefaultValue = DashboardUrlInput.DefaultValue,
            JsonKey = "dashboard-url"
        };

        public static readonly CLIOption<IEnumerable<string>> TestProjects = new CLIOption<IEnumerable<string>>
        {
            ArgumentName = "--test-projects",
            ArgumentShortName = "-tp",
            ArgumentDescription = TestProjectsInput.HelpText,
            DefaultValue = TestProjectsInput.DefaultValue,
            JsonKey = "test-projects"
        };

        public static readonly CLIOption<string> AzureSAS = new CLIOption<string>
        {
            ArgumentName = "--azure-storage-sas",
            ArgumentShortName = "-sas <azure-sas-key>",
            ArgumentDescription = AzureFileStorageSasInput.HelpText,
            DefaultValue = AzureFileStorageSasInput.DefaultValue,
            JsonKey = "azure-storage-sas"
        };

        public static readonly CLIOption<string> AzureFileStorageUrl = new CLIOption<string>
        {
            ArgumentName = "--azure-storage-url",
            ArgumentShortName = "-storage-url <url>",
            ArgumentDescription = AzureFileStorageUrlInput.HelpText,
            DefaultValue = AzureFileStorageUrlInput.DefaultValue,
            JsonKey = "azure-storage-url"
        };

        public static readonly CLIOption<string> MutationLevel = new CLIOption<string>
        {
            ArgumentName = "--mutation-level",
            ArgumentShortName = "-level",
            ArgumentDescription = MutationLevelInput.HelpText,
            DefaultValue = MutationLevelInput.DefaultValue.ToString(),
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
