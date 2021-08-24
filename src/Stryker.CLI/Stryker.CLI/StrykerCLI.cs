using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Crayon;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;
using Stryker.CLI.NuGet;
using Stryker.Core;
using Stryker.Core.Logging;
using Stryker.Core.Options;

namespace Stryker.CLI
{
    public class StrykerCLI
    {
        private readonly IStrykerRunner _stryker;
        private readonly LogBuffer _logBuffer;
        public int ExitCode { get; private set; }

        public StrykerCLI(IStrykerRunner stryker)
        {
            _stryker = stryker;
            // Create a log buffer to buffer log messages until the logging is configured.
            _logBuffer = new LogBuffer();
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
            var dashboardApiKeyParam = CreateOption(app, CLIOptions.DashboardApiKeyOption);
            var dashboardUrlParam = CreateOption(app, CLIOptions.DashboardUrlOption);
            var reportersProjectNameParam = CreateOption(app, CLIOptions.DashboardProjectNameOption);
            var reportersModuleNameParam = CreateOption(app, CLIOptions.DashboardModuleNameOption);
            var reportersProjectVersionParam = CreateOption(app, CLIOptions.DashboardProjectVersionOption);
            var logConsoleParam = CreateOption(app, CLIOptions.LogLevel);
            var devMode = CreateOption(app, CLIOptions.DevMode);
            var coverageAnalysis = CreateOption(app, CLIOptions.CoverageAnalysis);
            var abortTestOnFailParam = CreateOption(app, CLIOptions.AbortTestOnFail);
            var disableSimultaneousTesting = CreateOption(app, CLIOptions.DisableTestingMix);
            var timeoutParam = CreateOption(app, CLIOptions.AdditionalTimeoutMS);
            var excludedMutationsParam = CreateOption(app, CLIOptions.ExcludedMutations);
            var ignoreMethodsParam = CreateOption(app, CLIOptions.IgnoreMethods);
            var fileLogParam = CreateOption(app, CLIOptions.LogToFile);
            var projectNameParam = CreateOption(app, CLIOptions.ProjectFileName);
            var maxConcurrentTestRunnersParam = CreateOption(app, CLIOptions.MaxConcurrentTestRunners);
            var thresholdHighParam = CreateOption(app, CLIOptions.ThresholdHigh);
            var thresholdLowParam = CreateOption(app, CLIOptions.ThresholdLow);
            var thresholdBreakParam = CreateOption(app, CLIOptions.ThresholdBreak);
            var filesToExclude = CreateOption(app, CLIOptions.FilesToExclude);
            var mutateParam = CreateOption(app, CLIOptions.Mutate);
            var testRunner = CreateOption(app, CLIOptions.TestRunner);
            var solutionPathParam = CreateOption(app, CLIOptions.SolutionPath);
            var languageVersion = CreateOption(app, CLIOptions.LanguageVersionOption);
            var diffParam = CreateOption(app, CLIOptions.Diff);
            var diffCompareToDashboard = CreateOption(app, CLIOptions.DashboardCompare);
            var gitDiffTargetParam = CreateOption(app, CLIOptions.GitDiffTarget);
            var testProjectsParam = CreateOption(app, CLIOptions.TestProjects);
            var fallbackVersionParam = CreateOption(app, CLIOptions.DashboardFallbackVersionOption);
            var baselineStorageLocation = CreateOption(app, CLIOptions.BaselineStorageLocation);
            var azureSAS = CreateOption(app, CLIOptions.AzureSAS);
            var azureFileStorageUrl = CreateOption(app, CLIOptions.AzureFileStorageUrl);
            var mutationLevelParam = CreateOption(app, CLIOptions.MutationLevel);
            var dashboardCompareFileExcludePatterns = CreateOption(app, CLIOptions.DiffIgnoreFiles);

            app.HelpOption("--help | -h | -?");

            app.OnExecute(() =>
            {
                // app started
                var options = new OptionsBuilder(_logBuffer).Build(
                    basePath: Directory.GetCurrentDirectory(),
                    reporter: reporterParam,
                    dashboardApiKey: dashboardApiKeyParam,
                    dashboardUrl: dashboardUrlParam,
                    reportersProjectName: reportersProjectNameParam,
                    fallbackVersion: fallbackVersionParam,
                    reportersModuleName: reportersModuleNameParam,
                    reportersProjectVersion: reportersProjectVersionParam,
                    projectUnderTestNameFilter: projectNameParam,
                    additionalTimeoutMS: timeoutParam,
                    excludedMutations: excludedMutationsParam,
                    ignoreMethods: ignoreMethodsParam,
                    logLevel: logConsoleParam,
                    logToFile: fileLogParam,
                    devMode: devMode,
                    coverageAnalysis: coverageAnalysis,
                    abortTestOnFail: abortTestOnFailParam,
                    configFilePath: configFilePathParam,
                    disableSimultaneousTesting: disableSimultaneousTesting,
                    maxConcurrentTestRunners: maxConcurrentTestRunnersParam,
                    thresholdHigh: thresholdHighParam,
                    thresholdLow: thresholdLowParam,
                    thresholdBreak: thresholdBreakParam,
                    filesToExclude: filesToExclude,
                    filePatterns: mutateParam,
                    testRunner: testRunner,
                    solutionPath: solutionPathParam,
                    languageVersion: languageVersion,
                    diff: diffParam,
                    diffCompareToDashboard: diffCompareToDashboard,
                    gitDiffTarget: gitDiffTargetParam,
                    testProjects: testProjectsParam,
                    baselineStorageLocation: baselineStorageLocation,
                    azureFileStorageUrl: azureFileStorageUrl,
                    azureSAS: azureSAS,
                    mutationLevel: mutationLevelParam,
                    dashboardCompareFileExcludePatterns: dashboardCompareFileExcludePatterns);

                RunStryker(options);
                return ExitCode;
            });
            return app.Execute(args);
        }

        private void RunStryker(StrykerOptions options)
        {
            PrintStrykerASCIIName();
            _ = PrintStrykerVersionInformationAsync();

            StrykerRunResult result = _stryker.RunMutationTest(options, _logBuffer.GetMessages());

            HandleStrykerRunResult(options, result);
        }

        private void HandleStrykerRunResult(StrykerOptions options, StrykerRunResult result)
        {
            var logger = ApplicationLogging.LoggerFactory.CreateLogger<StrykerCLI>();

            logger.LogInformation("The final mutation score is {MutationScore:P2}", result.MutationScore);
            if (result.ScoreIsLowerThanThresholdBreak())
            {
                var thresholdBreak = (double)options.Thresholds.Break / 100;
                logger.LogWarning("Final mutation score is below threshold break. Crashing...");

                Console.WriteLine(Output.Red($@"
 The mutation score is lower than the configured break threshold of {thresholdBreak:P0}.
 If you're running in a CI environment, this means your pipeline will now fail."));

                Console.WriteLine(Output.Green(" Looks like you've got some work to do :)"));

                ExitCode = ExitCodes.BreakThresholdViolated;
            }
        }

        private void PrintStrykerASCIIName()
        {
            Console.WriteLine(Output.Yellow(@"
   _____ _              _               _   _ ______ _______  
  / ____| |            | |             | \ | |  ____|__   __| 
 | (___ | |_ _ __ _   _| | _____ _ __  |  \| | |__     | |    
  \___ \| __| '__| | | | |/ / _ \ '__| | . ` |  __|    | |    
  ____) | |_| |  | |_| |   <  __/ |    | |\  | |____   | |    
 |_____/ \__|_|   \__, |_|\_\___|_| (_)|_| \_|______|  |_|    
                   __/ |                                      
                  |___/                                       
"));
            Console.WriteLine();
        }

        private async Task PrintStrykerVersionInformationAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyVersion = assembly.GetName().Version;
            var currentVersion = SemanticVersion.Parse($"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}");

            Console.WriteLine($" Version: {Output.Green(currentVersion.ToString())} (beta)");
            Console.WriteLine();

            var nugetInfo = await StrykerNugetFeedInfo.Create();
            var latestVersion = nugetInfo?.LatestVersion;

            if (latestVersion != null && latestVersion != currentVersion)
            {
                Console.WriteLine(Output.Yellow($@" A new version of Stryker.NET ({latestVersion}) is available. Please consider upgrading using `dotnet tool update -g dotnet-stryker`"));
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Simplify app option creation to prevent code duplication
        /// </summary>
        private CommandOption CreateOption<T>(CommandLineApplication app, CLIOption<T> option)
        {
            var description = option.IsDeprecated
                ? $"(deprecated:{option.DeprecatedMessage})" + option.ArgumentDescription
                : option.ArgumentDescription;

            return app.Option($"{option.ArgumentName} | {option.ArgumentShortName}",
                description,
                option.ValueType);
        }
    }
}
