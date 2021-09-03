using System;
using System.Reflection;
using System.Threading.Tasks;
using Crayon;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;
using Stryker.CLI.Clients;
using Stryker.CLI.Logging;
using Stryker.Core;
using Stryker.Core.Options;

namespace Stryker.CLI
{
    public class StrykerCli
    {
        private readonly IStrykerRunner _stryker;
        private readonly IConfigReader _configReader;
        private readonly ILoggingInitializer _loggingInitializer;
        private readonly IStrykerNugetFeedClient _nugetClient;

        public int ExitCode { get; private set; } = 0;

        public StrykerCli(IStrykerRunner stryker = null,
            IConfigReader configReader = null,
            ILoggingInitializer loggingInitializer = null,
            IStrykerNugetFeedClient nugetClient = null)
        {
            _stryker = stryker ?? new StrykerRunner();
            _configReader = configReader ?? new ConfigReader();
            _loggingInitializer = loggingInitializer ?? new LoggingInitializer();
            _nugetClient = nugetClient ?? new StrykerNugetFeedClient();
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
                ExtendedHelpText = "Welcome to Stryker for .Net! Run dotnet stryker to kick off a mutation test run"
            };
            app.HelpOption();

            var inputs = new StrykerInputs();
            var cmdConfigHandler = new CommandLineConfigHandler();

            cmdConfigHandler.RegisterCommandLineOptions(app, inputs);

            app.OnExecute(() =>
            {
                // app started
                PrintStrykerASCIIName();

                _configReader.Build(inputs, args, app, cmdConfigHandler);
                _loggingInitializer.SetupLogOptions(inputs);

                PrintStrykerVersionInformationAsync();
                RunStryker(inputs);
                return ExitCode;
            });
            return app.Execute(args);
        }

        private void RunStryker(IStrykerInputs inputs)
        {
            var result = _stryker.RunMutationTest(inputs, ApplicationLogging.LoggerFactory);

            HandleStrykerRunResult(inputs, result);
        }

        private void HandleStrykerRunResult(IStrykerInputs inputs, StrykerRunResult result)
        {
            var logger = ApplicationLogging.LoggerFactory.CreateLogger<StrykerCli>();

            logger.LogInformation("The final mutation score is {MutationScore:P2}", result.MutationScore);
            if (result.ScoreIsLowerThanThresholdBreak())
            {
                var thresholdBreak = (double)inputs.ValidateAll().Thresholds.Break / 100;
                logger.LogWarning("Final mutation score is below threshold break. Crashing...");

                Console.WriteLine(Output.Red($@"
 The mutation score is lower than the configured break threshold of {thresholdBreak:P0}."));

                Console.WriteLine(Output.Red(" Looks like you've got some work to do :)"));

                ExitCode = 1;
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Bug", "S3168:\"async\" methods should not return \"void\"", Justification = "This method is fire and forget. Task.Run also doesn't work in unit tests")]
        private async void PrintStrykerVersionInformationAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyVersion = assembly.GetName().Version;
            var currentVersion = SemanticVersion.Parse($"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}");

            Console.WriteLine($"Version: {Output.Green(currentVersion.ToString())}");
            Console.WriteLine();

            var latestVersion = await _nugetClient.GetMaxVersion();

            if (latestVersion > currentVersion)
            {
                Console.WriteLine(Output.Yellow($@"A new version of Stryker.NET ({latestVersion}) is available. Please consider upgrading using `dotnet tool update -g dotnet-stryker`"));
                Console.WriteLine();
            }
        }
    }
}
