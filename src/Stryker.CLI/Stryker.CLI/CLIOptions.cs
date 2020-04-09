using McMaster.Extensions.CommandLineUtils;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stryker.CLI
{
    public static class CLIOptions
    {
        private static readonly StrykerOptions _defaultOptions = new StrykerOptions();

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
            ArgumentDescription = $@"Sets the reporter | { FormatOptionsString(_defaultOptions.Reporters, (IEnumerable<Reporter>)Enum.GetValues(_defaultOptions.Reporters.First().GetType()), new List<Reporter> { Reporter.ConsoleProgressBar, Reporter.ConsoleProgressDots, Reporter.ConsoleReport }) }]
    This argument takes a json array as a value. Example: ['{ Reporter.Progress }', '{ Reporter.Html }']",
            DefaultValue = _defaultOptions.Reporters.Select(r => r.ToString()).ToArray(),
            JsonKey = "reporters"
        };

        public static readonly CLIOption<string> LogLevel = new CLIOption<string>
        {
            ArgumentName = "--log-level",
            ArgumentShortName = "-l <logLevel>",
            ArgumentDescription = "Sets the console output logging level | Options [error, warning, info (default), debug, trace]",
            DefaultValue = "info",
            JsonKey = "log-level"
        };

        public static readonly CLIOption<bool> LogToFile = new CLIOption<bool>
        {
            ArgumentName = "--log-file",
            ArgumentShortName = "-f",
            ArgumentDescription = "Makes the logger write to a file (Logging to file always uses loglevel trace)",
            DefaultValue = _defaultOptions.LogOptions.LogToFile,
            ValueType = CommandOptionType.NoValue,
            JsonKey = "log-file"
        };

        public static readonly CLIOption<bool> DevMode = new CLIOption<bool>
        {
            ArgumentName = "--dev-mode",
            ArgumentShortName = "-dev",
            ArgumentDescription = @"Stryker automatically removes all mutations from a method if a failed mutation could not be rolled back
    Setting this flag makes stryker not remove the mutations but rather break on failed rollbacks",
            DefaultValue = _defaultOptions.DevMode,
            ValueType = CommandOptionType.NoValue,
            JsonKey = "dev-mode"
        };

        public static readonly CLIOption<int> AdditionalTimeoutMS = new CLIOption<int>
        {
            ArgumentName = "--timeout-ms",
            ArgumentShortName = "-t <ms>",
            ArgumentDescription = $"Stryker calculates a timeout based on the time the testrun takes before the mutations| Options {_defaultOptions.AdditionalTimeoutMS}",
            DefaultValue = _defaultOptions.AdditionalTimeoutMS,
            JsonKey = "timeout-ms"
        };

        public static readonly CLIOption<string[]> ExcludedMutations = new CLIOption<string[]>
        {
            ArgumentName = "--excluded-mutations",
            ArgumentShortName = "-em <mutators>",
            ArgumentDescription = @"The given mutators will be excluded for this mutation testrun.
    This argument takes a json array as value. Example: ['string', 'logical']",
            JsonKey = "excluded-mutations"
        };

        public static readonly CLIOption<string[]> IgnoreMethods = new CLIOption<string[]>
        {
            ArgumentName = "--ignore-methods",
            ArgumentShortName = "-im <methodNames>",
            ArgumentDescription = @"Mutations that would affect parameters that are directly passed into methods with given names are ignored. Example: ['ConfigureAwait', 'ToString']",
            JsonKey = "ignore-methods"
        };

        public static readonly CLIOption<string> ProjectFileName = new CLIOption<string>
        {
            ArgumentName = "--project-file",
            ArgumentShortName = "-p <projectFileName>",
            ArgumentDescription = @"Used for matching the project references when finding the project to mutate. Example: ""ExampleProject.csproj""",
            JsonKey = "project-file"
        };

        public static readonly CLIOption<bool> Diff = new CLIOption<bool>
        {
            ArgumentName = "--diff",
            ArgumentShortName = "-diff",
            ArgumentDescription = @"Enables the diff feature. It makes sure to only mutate changed files. Gets the diff from git by default.",
            ValueType = CommandOptionType.NoValue,
            JsonKey = "diff"
        };

        public static readonly CLIOption<bool> DiffCompareToDashboard = new CLIOption<bool>
        {
            ArgumentName = "--diff-compare-dashboard",
            ArgumentShortName = "-dc",
            ArgumentDescription = $@"Enables comparing to results stored in Stryker Dashboard. This feature is only available in combination with {Diff.ArgumentName}",
            ValueType = CommandOptionType.NoValue,
            JsonKey = "diff-compare-dashboard"

        };

        public static readonly CLIOption<string> GitSource = new CLIOption<string>
        {
            ArgumentName = "--git-source",
            ArgumentShortName = "-gs <branchName>",
            ArgumentDescription = @"Sets the source branch to compare with the current codebase, used for calculating the difference when --diff is enabled.",
            DefaultValue = _defaultOptions.GitSource,
            JsonKey = "git-source"
        };

        public static readonly CLIOption<string> CoverageAnalysis = new CLIOption<string>
        {
            ArgumentName = "--coverage-analysis",
            ArgumentShortName = "-ca <mode>",
            DefaultValue = _defaultOptions.OptimizationMode,
            ArgumentDescription = @"Use coverage info to speed up execution. Possible values are: off, all, perTest, perIsolatedTest.
    - off: coverage data is not captured.
    - perTest (Default): capture the list of mutations covered by each test. For every mutation that has tests, only the tests that cover this mutation are tested. Fastest option.
    - all: capture the list of mutations covered by each test. Test only these mutations. Fast option.
    - perTestInIsolation: like 'perTest', but running each test in an isolated run. Slowest fast option.",
            JsonKey = "coverage-analysis"
        };

        public static readonly CLIOption<bool> AbortTestOnFail = new CLIOption<bool>
        {
            ArgumentName = "--abort-test-on-fail",
            ArgumentShortName = "-atof",
            DefaultValue = _defaultOptions.Optimizations.HasFlag(OptimizationFlags.AbortTestOnKill),
            ArgumentDescription = @"Abort unit testrun as soon as any one unit test fails. This can reduce the overall running time.",
            ValueType = CommandOptionType.NoValue,
            JsonKey = "abort-test-on-fail"
        };

        public static readonly CLIOption<bool> DisableTestingMix = new CLIOption<bool>
        {
            ArgumentName = "--disable-testing-mix-mutations",
            ArgumentShortName = "-tmm",
            DefaultValue = _defaultOptions.Optimizations.HasFlag(OptimizationFlags.DisableTestMix),
            ArgumentDescription = @"Test each mutation in an isolated test run.",
            ValueType = CommandOptionType.NoValue,
            JsonKey = "disable-testing-mix-mutations"
        };

        public static readonly CLIOption<int> MaxConcurrentTestRunners = new CLIOption<int>
        {
            ArgumentName = "--max-concurrent-test-runners",
            ArgumentShortName = "-c <integer>",
            ArgumentDescription = @"Mutation testing is time consuming. 
    By default Stryker tries to make the most of your CPU, by spawning as many test runners as you have CPU cores.
    This setting allows you to override this default behavior.

    Reasons you might want to lower this setting:
                                                                 
        - Your test runner starts a browser (another CPU-intensive process)
        - You're running on a shared server
        - You are running stryker in the background while doing other work",
            DefaultValue = _defaultOptions.ConcurrentTestrunners,
            JsonKey = "max-concurrent-test-runners"
        };

        public static readonly CLIOption<int> ThresholdBreak = new CLIOption<int>
        {
            ArgumentName = "--threshold-break",
            ArgumentShortName = "-tb <thresholdBreak>",
            ArgumentDescription = $"Set the minimum mutation score threshold. Anything below this score will return a non-zero exit code. | {_defaultOptions.Thresholds.Break} (default)",
            DefaultValue = _defaultOptions.Thresholds.Break,
            JsonKey = "threshold-break"
        };

        public static readonly CLIOption<int> ThresholdLow = new CLIOption<int>
        {
            ArgumentName = "--threshold-low",
            ArgumentShortName = "-tl <thresholdLow>",
            ArgumentDescription = $"Set the lower bound of the mutation score threshold. It will not fail the test. | {_defaultOptions.Thresholds.Low} (default)",
            DefaultValue = _defaultOptions.Thresholds.Low,
            JsonKey = "threshold-low"
        };

        public static readonly CLIOption<int> ThresholdHigh = new CLIOption<int>
        {
            ArgumentName = "--threshold-high",
            ArgumentShortName = "-th <thresholdHigh>",
            ArgumentDescription = $"Set the preferred mutation score threshold. | {_defaultOptions.Thresholds.High} (default)",
            DefaultValue = _defaultOptions.Thresholds.High,
            JsonKey = "threshold-high"
        };

        public static readonly CLIOption<string[]> FilesToExclude = new CLIOption<string[]>
        {
            ArgumentName = "--files-to-exclude",
            ArgumentShortName = "-fte <files-to-exclude>",
            ArgumentDescription = "Set files to exclude for mutation. Example: ['C:/ExampleProject/Example.cs','C:/ExampleProject/Example2.cs']",
            DefaultValue = null,
            JsonKey = "files-to-exclude",
            IsDeprecated = true,
            DeprecatedMessage = "Use '--mutate' instead."
        };

        public static readonly CLIOption<string[]> Mutate = new CLIOption<string[]>
        {
            ArgumentName = "--mutate",
            ArgumentShortName = "-m <file-patterns>",
            ArgumentDescription = @"Allows to specify file that should in- or excluded for the mutations.
    Use glob syntax for wildcards: https://en.wikipedia.org/wiki/Glob_(programming)
    Use '!' at the start of a pattern to exclude all matched files.
    Use '{<start>..<end>}' at the end of a pattern to specify spans of text in files to in- or exclude.
    Example: ['**/*Service.cs','!**/MySpecialService.cs', '**/MyOtherService.cs{1..10}{32..45}']",
            DefaultValue = null,
            JsonKey = "mutate",
        };

        public static readonly CLIOption<string> SolutionPath = new CLIOption<string>
        {
            ArgumentName = "--solution-path",
            ArgumentShortName = "-s <path>",
            ArgumentDescription = @"Full path to your solution file. The solution file is needed to build the project and resolve dependencies for
    .net framework but can optionally be used for .net core. Path can be relative from test project or full path.",
            DefaultValue = null,
            JsonKey = "solution-path"
        };

        public static readonly CLIOption<string> TestRunner = new CLIOption<string>
        {
            ArgumentName = "--test-runner",
            ArgumentShortName = "-tr <testRunner>",
            ArgumentDescription = $"Choose which testrunner should be used to run your tests. | { FormatOptionsString(_defaultOptions.TestRunner, (IEnumerable<TestRunner>)Enum.GetValues(_defaultOptions.TestRunner.GetType())) }",
            DefaultValue = _defaultOptions.TestRunner.ToString(),
            JsonKey = "test-runner"
        };

        public static readonly CLIOption<string> LanguageVersionOption = new CLIOption<string>
        {
            ArgumentName = "--language-version",
            ArgumentShortName = "-lv <csharp-version-name>",
            ArgumentDescription = $"Set the c# version used to compile. | { FormatOptionsString(_defaultOptions.LanguageVersion, ((IEnumerable<LanguageVersion>)Enum.GetValues(_defaultOptions.LanguageVersion.GetType())).Where(l => l != LanguageVersion.CSharp1)) }",
            DefaultValue = _defaultOptions.LanguageVersion.ToString(),
            JsonKey = "language-version"
        };

        public static readonly CLIOption<string> DashboardApiKeyOption = new CLIOption<string>
        {
            ArgumentName = "--dashboard-api-key",
            ArgumentShortName = "-dk <api-key>",
            ArgumentDescription = $"Api key for dashboard reporter. You can get your key here: {_defaultOptions.DashboardUrl}",
            DefaultValue = null,
            JsonKey = "dashboard-api-key"
        };

        public static readonly CLIOption<string> DashboardProjectNameOption = new CLIOption<string>
        {
            ArgumentName = "--dashboard-project",
            ArgumentShortName = "-project <name>",
            ArgumentDescription = @"The organizational name for your project. Required when dashboard reporter is turned on.
For example: Your project might be called 'consumer-loans' and it might contains sub-modules 'consumer-loans-frontend' and 'consumer-loans-backend'.",
            DefaultValue = null,
            JsonKey = "dashboard-project"
        };

        public static readonly CLIOption<string> DashboardModuleNameOption = new CLIOption<string>
        {
            ArgumentName = "--dashboard-module",
            ArgumentShortName = "-module <name>",
            ArgumentDescription = $"Module name used in reporters when project consists of multiple modules. See project-name for examples.",
            DefaultValue = null,
            JsonKey = "dashboard-module"
        };

        public static readonly CLIOption<string> DashboardProjectVersionOption = new CLIOption<string>
        {
            ArgumentName = "--dashboard-version",
            ArgumentShortName = "-version <version>",
            ArgumentDescription = $"Project version used in reporters. Can be semver, git commit hash, branch name or anything else to indicate what version of your software you're testing.",
            DefaultValue = null,
            JsonKey = "dashboard-version"
        };

        public static readonly CLIOption<string> DashboardUrlOption = new CLIOption<string>
        {
            ArgumentName = "--dashboard-url",
            ArgumentShortName = "-url <dashboard-url>",
            ArgumentDescription = $"Provide an alternative root url for Stryker Dashboard.",
            DefaultValue = _defaultOptions.DashboardUrl,
            JsonKey = "dashboard-url"
        };

        public static readonly CLIOption<IEnumerable<string>> TestProjects = new CLIOption<IEnumerable<string>>
        {
            ArgumentName = "--test-projects",
            ArgumentShortName = "-tp",
            ArgumentDescription = $"Specify what test projects should run on the project under test.",
            DefaultValue = _defaultOptions.TestProjects,
            JsonKey = "test-projects"
        };

        private static string FormatOptionsString<T, Y>(T @default, IEnumerable<Y> options)
        {
            return FormatOptionsString(new List<T> { @default }, options, new List<Y>());
        }

        private static string FormatOptionsString<T, Y>(IEnumerable<T> @default, IEnumerable<Y> options, IEnumerable<Y> deprecated)
        {
            StringBuilder optionsString = new StringBuilder();

            optionsString.Append($"Options[ (default)[ {string.Join(", ", @default)} ], ");
            string nonDefaultOptions = string.Join(
            ", ",
            options
            .Where(o => !@default.Any(d => d.ToString() == o.ToString()))
            .Where(o => !deprecated.Any(d => d.ToString() == o.ToString())));

            string deprecatedOptions = "";
            if (deprecated.Any())
            {
                deprecatedOptions = "(deprecated) " + string.Join(", (deprecated) ", options.Where(o => deprecated.Any(d => d.ToString() == o.ToString())));
            }

            optionsString.Append(string.Join(", ", nonDefaultOptions, deprecatedOptions));
            optionsString.Append(" ]");

            return optionsString.ToString();
        }
    }
}
