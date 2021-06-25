using System;
using System.IO;
using System.Reflection;
using Crayon;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

            var inputs = InputBuilder.InitializeInputs(_logBuffer);
            var cmdConfigHandler = new CommandLineConfigHandler();

            cmdConfigHandler.RegisterCommandlineOptions(app, inputs);

            app.OnExecute(() =>
            {
                // app started
                PrintStrykerASCIIName();
                PrintStrykerVersionInformationAsync();

                var inputs = InputBuilder.Build(args, app, cmdConfigHandler);

                RunStryker(inputs);
                return ExitCode;
            });
            return app.Execute(args);
        }

        private void RunStryker(IStrykerInputs inputs)
        {
            StrykerRunResult result = _stryker.RunMutationTest(inputs, _logBuffer.GetMessages());

            HandleStrykerRunResult(inputs, result);
        }

        private void HandleStrykerRunResult(IStrykerInputs inputs, StrykerRunResult result)
        {
            var logger = ApplicationLogging.LoggerFactory.CreateLogger<StrykerCLI>();

            logger.LogInformation("The final mutation score is {MutationScore:P2}", result.MutationScore);
            if (result.ScoreIsLowerThanThresholdBreak())
            {
                var thresholdBreak = (double)inputs.ValidateAll().Thresholds.Break / 100;
                logger.LogWarning("Final mutation score is below threshold break. Crashing...");

                Console.WriteLine(Output.Red($@"
 The mutation score is lower than the configured break threshold of {thresholdBreak:P0}.
 If you're running in a CI environment, this means your pipeline will now fail."));

                Console.WriteLine(Output.Green(" Looks like you've got some work to do :)"));

                ExitCode = 1;
            }
        }

        private static void PrintStrykerASCIIName()
        {
            // Crayon does not support background coloring (yet)
            Console.WriteLine(Output.Yellow().Text(@"
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

        private static async void PrintStrykerVersionInformationAsync()
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
    }
}
