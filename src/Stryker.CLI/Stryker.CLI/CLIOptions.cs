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

        public static readonly CLIOption<string> TestProjectFileName = new CLIOption<string>
        {
            ArgumentName = "--test-project-file",
            ArgumentShortName = "-tp <testProjectFileName>",
            ArgumentDescription = @"Used for specifying the test project if there are multiple projects in the folder. Example: ""ExampleTestProject.csproj""",
            DefaultValue = _defaultOptions.TestProjectNameFilter,
            JsonKey = "test-project-file"
        };

        public static readonly CLIOption<string> CoverageAnalysis = new CLIOption<string>
        {
            ArgumentName = "--coverage-analysis",
            ArgumentShortName = "-ca <mode>",
            DefaultValue = "off",
            ArgumentDescription = @"Use coverage info to speed up execution. Possible values are: off, all, perTest, perIsolatedTest.
    - off: coverage data is not captured (default mode).
    - perTest: capture the list of mutants covered by each test. For every mutant that has tests, only the tests that cover a the mutant are tested. Fastest option.
    - all: capture the list of mutants covered by each test. Test only these mutants. Non covered mutants are assumed as survivors. Fast option.
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
            ArgumentDescription = "Set files to exclude for mutation. Example: ['C:\\ExampleProject\\Example.cs','C:\\ExampleProject\\Example2.cs']",
            DefaultValue = null,
            JsonKey = "files-to-exclude",
            Deprecated = true,
            DeprecatedMessage = "Use 'filePattern' instead."
        };

        public static readonly CLIOption<string[]> FilePatterns = new CLIOption<string[]>
        {
            ArgumentName = "--file-patterns",
            ArgumentShortName = "-fp <file-patterns>",
            ArgumentDescription = @"Allows to specify file that should in- or excluded for the mutations.
    Use glob syntax for wildcards: https://en.wikipedia.org/wiki/Glob_(programming)
    Use '!' at the start of a pattern to exclude all matched files.
    Use '{<start>..<end>}' at the end of a pattern to specify spans of text in files to in- or exclude.
    Example: ['**\*Service.cs','!**\MySpecialService.cs', '**\MyOtherService.cs{1..10}{32..45}']",
            DefaultValue = null,
            JsonKey = "file-patterns",
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
            ArgumentShortName = "-tr",
            ArgumentDescription = $"Choose which testrunner should be used to run your tests. | { FormatOptionsString(_defaultOptions.TestRunner, (IEnumerable<TestRunner>)Enum.GetValues(_defaultOptions.TestRunner.GetType())) }",
            DefaultValue = _defaultOptions.TestRunner.ToString(),
            JsonKey = "test-runner"
        };

        public static readonly CLIOption<string> LanguageVersionOption = new CLIOption<string>
        {
            ArgumentName = "--language-version",
            ArgumentShortName = "-lv",
            ArgumentDescription = $"Set the c# version used to compile. | { FormatOptionsString(_defaultOptions.LanguageVersion, ((IEnumerable<LanguageVersion>)Enum.GetValues(_defaultOptions.LanguageVersion.GetType())).Where(l => l != LanguageVersion.CSharp1)) }",
            DefaultValue = _defaultOptions.LanguageVersion.ToString(),
            JsonKey = "language-version"
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
