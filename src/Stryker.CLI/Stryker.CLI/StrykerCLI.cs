using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Stryker.CLI.NuGet;
using Stryker.Core;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.Testing;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Stryker.CLI
{
    public class StrykerCLI
    {
        private readonly IStrykerRunner _stryker;
        private readonly ILogger _logger;
        public int ExitCode { get; set; }

        public StrykerCLI(IStrykerRunner stryker)
        {
            _stryker = stryker;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<StrykerCLI>();
            ExitCode = 0;
        }

        /// <summary>
        /// Analyses the arguments and displays an interface to the user. Kicks off the program.
        /// </summary>
        /// <param name="args">User input</param>
        public int Run(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "Stryker",
                FullName = "Stryker: Stryker mutator for .Net",
                Description = "Stryker mutator for .Net",
                ExtendedHelpText = "Welcome to Stryker for .Net! Run dotnet stryker to kick off a mutation test run"
            };

            var configFilePathParam = CreateOption(app, CLIOptions.ConfigFilePath);
            var reporterParam = CreateOption(app, CLIOptions.Reporters);
            var logConsoleParam = CreateOption(app, CLIOptions.LogLevel);
            var devMode = CreateOption(app, CLIOptions.DevMode);
            var coverageAnalysis = CreateOption(app, CLIOptions.CoverageAnalysis);
            var advancedOptions = CreateOption(app, CLIOptions.AbortOnFailTest);
            var timeoutParam = CreateOption(app, CLIOptions.AdditionalTimeoutMS);
            var exludedMutationsParam = CreateOption(app, CLIOptions.ExcludedMutations);
            var fileLogParam = CreateOption(app, CLIOptions.LogToFile);
            var projectNameParam = CreateOption(app, CLIOptions.ProjectFileName);
            var maxConcurrentTestRunnersParam = CreateOption(app, CLIOptions.MaxConcurrentTestRunners);
            var thresholdHighParam = CreateOption(app, CLIOptions.ThresholdHigh);
            var thresholdLowParam = CreateOption(app, CLIOptions.ThresholdLow);
            var thresholdBreakParam = CreateOption(app, CLIOptions.ThresholdBreak);
            var filesToExclude = CreateOption(app, CLIOptions.FilesToExclude);
            var testRunner = CreateOption(app, CLIOptions.TestRunner);
            var solutionPathParam = CreateOption(app, CLIOptions.SolutionPath);
            var languageVersion = CreateOption(app, CLIOptions.LanguageVersionOption);

            app.HelpOption("--help | -h | -?");

            app.OnExecute(() =>
            {
                // app started
                var options = new OptionsBuilder().Build(
                    Directory.GetCurrentDirectory(),
                    reporterParam,
                    projectNameParam,
                    timeoutParam,
                    exludedMutationsParam,
                    logConsoleParam,
                    fileLogParam,
                    devMode,
                    coverageAnalysis,
                    advancedOptions,
                    configFilePathParam,
                    maxConcurrentTestRunnersParam,
                    thresholdHighParam,
                    thresholdLowParam,
                    thresholdBreakParam,
                    filesToExclude,
                    testRunner,
                    solutionPathParam,
                    languageVersion);

                RunStryker(options);
                return ExitCode;
            });
            return app.Execute(args);
        }

        private void RunStryker(StrykerOptions options)
        {
            PrintStykerASCIIName();
            _ = PrintStrykerVersionInformationAsync();

            StrykerRunResult results = _stryker.RunMutationTest(options);
            if (!results.IsScoreAboveThresholdBreak())
            {
                HandleBreakingThresholdScore(options, results);
            }
        }

        private void HandleBreakingThresholdScore(StrykerOptions options, StrykerRunResult results)
        {
            _logger.LogError($@"Final mutation score: {results.MutationScore * 100} under breaking threshold value {options.Thresholds.Break}.
Setting exit code to 1 (failure).
Improve the mutation score or set the `threshold-break` value lower to prevent this error in the future.");
            ExitCode = 1;
        }

        private void PrintStykerASCIIName()
        {
            new Chalk().Default(@"
   _____ _              _               _   _ ______ _______ 
  / ____| |            | |             | \ | |  ____|__   __|
 | (___ | |_ _ __ _   _| | _____ _ __  |  \| | |__     | |   
  \___ \| __| '__| | | | |/ / _ \ '__| | . ` |  __|    | |   
  ____) | |_| |  | |_| |   <  __/ |    | |\  | |____   | |   
 |_____/ \__|_|   \__, |_|\_\___|_| (_)|_| \_|______|  |_|   
                   __/ |                                   
                  |___/                                    
");
            Console.WriteLine();
        }

        private async Task PrintStrykerVersionInformationAsync()
        {
            var chalk = new Chalk();
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyVersion = assembly.GetName().Version;
            var currentVersion = $"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}";

            Console.Write("Version: ");
            chalk.Green(currentVersion);
            Console.WriteLine(" (beta)");
            Console.WriteLine();

            var nugetInfo = await StrykerNugetFeedInfo.Create();
            var latestVersion = nugetInfo?.LatestVersion;

            if (latestVersion != null && latestVersion != currentVersion)
            {
                chalk.Yellow($@"A new version of Stryker.NET ({latestVersion}) is available. Please consider upgrading using `dotnet tool update -g dotnet-stryker` {Environment.NewLine}");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Simplify app option creation to prevent code duplication
        /// </summary>
        private CommandOption CreateOption<T>(CommandLineApplication app, CLIOption<T> option)
        {
            return app.Option($"{option.ArgumentName} | {option.ArgumentShortName}",
                option.ArgumentDescription,
                option.ValueType);
        }
    }
}
