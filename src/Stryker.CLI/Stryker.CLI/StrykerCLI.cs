using System;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;
using Spectre.Console;
using Stryker.CLI.Clients;
using Stryker.CLI.CommandLineConfig;
using Stryker.CLI.Logging;
using Stryker.Core;
using Stryker.Core.Options;

namespace Stryker.CLI
{
    public class StrykerCli
    {
        private readonly IStrykerRunner _stryker;
        private readonly IConfigBuilder _configReader;
        private readonly ILoggingInitializer _loggingInitializer;
        private readonly IStrykerNugetFeedClient _nugetClient;
        private readonly IAnsiConsole _console;
        private readonly IFileSystem _fileSystem;

        public int ExitCode { get; private set; } = ExitCodes.Success;

        public StrykerCli(IStrykerRunner stryker = null,
            IConfigBuilder configReader = null,
            ILoggingInitializer loggingInitializer = null,
            IStrykerNugetFeedClient nugetClient = null,
            IAnsiConsole console = null,
            IFileSystem fileSystem = null)
        {
            _stryker = stryker ?? new StrykerRunner();
            _configReader = configReader ?? new ConfigBuilder();
            _loggingInitializer = loggingInitializer ?? new LoggingInitializer();
            _nugetClient = nugetClient ?? new StrykerNugetFeedClient();
            _console = console ?? AnsiConsole.Console;
            _fileSystem = fileSystem ?? new FileSystem();
        }

        /// <summary>
        /// Analyzes the arguments and displays an interface to the user. Kicks off the program.
        /// </summary>
        /// <param name="args">User input</param>
        public int Run(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "Stryker",
                FullName = "Stryker: Stryker mutator for .Net",
                Description = "Stryker mutator for .Net",
                ExtendedHelpText = "Welcome to Stryker for .Net! Run dotnet stryker to kick off a mutation test run",
                HelpTextGenerator = new GroupedHelpTextGenerator()
            };
            app.HelpOption();

            var inputs = new StrykerInputs();
            var cmdConfigReader = new CommandLineConfigReader(_console);

            cmdConfigReader.RegisterCommandLineOptions(app, inputs);
            cmdConfigReader.RegisterInitCommand(app, _fileSystem, inputs, args);

            app.OnExecute(() =>
            {
                // app started
                PrintStrykerASCIIName();

                _configReader.Build(inputs, args, app, cmdConfigReader);
                _loggingInitializer.SetupLogOptions(inputs);

                PrintStrykerVersionInformationAsync();
                RunStryker(inputs);
                return ExitCode;
            });

            try
            {
                return app.Execute(args);
            }
            catch (CommandParsingException ex)
            {
                Console.Error.WriteLine(ex.Message);

                if (ex is UnrecognizedCommandParsingException uex && uex.NearestMatches.Any())
                {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine("Did you mean this?");
                    foreach(var match in uex.NearestMatches)
                    {
                        Console.Error.WriteLine("    " + match);
                    }
                }

                return ExitCodes.OtherError;
            }
        }

        private void RunStryker(IStrykerInputs inputs)
        {
            var result = _stryker.RunMutationTest(inputs, ApplicationLogging.LoggerFactory);

            HandleStrykerRunResult(inputs, result);
        }

        private void HandleStrykerRunResult(IStrykerInputs inputs, StrykerRunResult result)
        {
            var logger = ApplicationLogging.LoggerFactory.CreateLogger<StrykerCli>();

            if (double.IsNaN(result.MutationScore))
            {
                logger.LogInformation("Stryker was unable to calculate a mutation score");
            }
            else
            {
                logger.LogInformation("The final mutation score is {MutationScore:P2}", result.MutationScore);
            }

            if (result.ScoreIsLowerThanThresholdBreak())
            {
                var thresholdBreak = (double)inputs.ValidateAll().Thresholds.Break / 100;
                logger.LogWarning("Final mutation score is below threshold break. Crashing...");

                _console.WriteLine();
                _console.MarkupLine($"[Red]The mutation score is lower than the configured break threshold of {thresholdBreak:P0}.[/]");
                _console.MarkupLine(" [Red]Looks like you've got some work to do :smiling_face_with_halo:[/]");

                ExitCode = ExitCodes.BreakThresholdViolated;
            }
        }

        private void PrintStrykerASCIIName()
        {
            _console.MarkupLine(@"[Yellow]
   _____ _              _               _   _ ______ _______  
  / ____| |            | |             | \ | |  ____|__   __| 
 | (___ | |_ _ __ _   _| | _____ _ __  |  \| | |__     | |    
  \___ \| __| '__| | | | |/ / _ \ '__| | . ` |  __|    | |    
  ____) | |_| |  | |_| |   <  __/ |    | |\  | |____   | |    
 |_____/ \__|_|   \__, |_|\_\___|_| (_)|_| \_|______|  |_|    
                   __/ |                                      
                  |___/                                       
[/]");
            _console.WriteLine();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Bug", "S3168:\"async\" methods should not return \"void\"", Justification = "This method is fire and forget. Task.Run also doesn't work in unit tests")]
        private async void PrintStrykerVersionInformationAsync()
        {
            var logger = ApplicationLogging.LoggerFactory.CreateLogger<StrykerCli>();
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            if (!SemanticVersion.TryParse(version, out var currentVersion))
            {
                if (string.IsNullOrEmpty(version))
                {
                    logger.LogWarning("{Attribute} is missing in {Assembly} at {AssemblyLocation}", nameof(AssemblyInformationalVersionAttribute), assembly, assembly.Location);
                }
                else
                {
                    logger.LogWarning("Failed to parse version {Version} as a semantic version", version);
                }
                return;
            }

            _console.MarkupLine($"Version: [Green]{currentVersion}[/]");
            logger.LogDebug("Stryker starting, version: {Version}", currentVersion);
            _console.WriteLine();

            var latestVersion = await _nugetClient.GetLatestVersionAsync();
            if (latestVersion > currentVersion)
            {
                _console.MarkupLine($@"[Yellow]A new version of Stryker.NET ({latestVersion}) is available. Please consider upgrading using `dotnet tool update -g dotnet-stryker`[/]");
                _console.WriteLine();
            }
            else
            {
                var previewVersion = await _nugetClient.GetPreviewVersionAsync();
                if (previewVersion > currentVersion)
                {
                    _console.MarkupLine($@"[Cyan]A preview version of Stryker.NET ({previewVersion}) is available.
If you would like to try out this preview version you can install it with `dotnet tool update -g dotnet-stryker --version {previewVersion}`
Since this is a preview feature things might not work as expected! Please report any findings on GitHub![/]");
                    _console.WriteLine();
                }
            }
        }
    }
}
