using Microsoft.Extensions.CommandLineUtils;
using Stryker.Core;
using Stryker.Core.Options;
using Stryker.Core.Logging;
using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Stryker.CLI
{
    public class StrykerCLI
    {
        private IStrykerRunner _stryker { get; set; }
        private ILogger _logger { get; set; }
        public StrykerCLI(IStrykerRunner stryker)
        {
            _stryker = stryker;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<StrykerCLI>();
        }

        /// <summary>
        /// Analyses the arguments and displays an interface to the user. Kicks off the program.
        /// </summary>
        /// <param name="args">User input</param>
        public void Run(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "StrykerNet",
                FullName = "StrykerNet: Stryker mutator for .Net Core",
                Description = "Stryker mutator for .Net Core",
                ExtendedHelpText = "Welcome to StrykerNet for .Net Core. Run dotnet stryker to kick off a mutation test run"
            };

            var configFilePathParam = app.Option($"{CLIOptions.ConfigFilePath.ArgumentName} | {CLIOptions.ConfigFilePath.ArgumentShortName}",
                CLIOptions.ConfigFilePath.ArgumentDescription,
                CommandOptionType.SingleValue);

            var reporterParam = app.Option($"{CLIOptions.Reporter.ArgumentName} | {CLIOptions.Reporter.ArgumentShortName}",
                CLIOptions.Reporter.ArgumentDescription,
                CommandOptionType.SingleValue);

            var logConsoleParam = app.Option($"{CLIOptions.LogLevel.ArgumentName} | {CLIOptions.LogLevel.ArgumentShortName}",
                CLIOptions.LogLevel.ArgumentDescription,
                CommandOptionType.SingleValue);

            var timeoutParam = app.Option($"{CLIOptions.AdditionalTimeoutMS.ArgumentName} | {CLIOptions.AdditionalTimeoutMS.ArgumentShortName}",
                CLIOptions.AdditionalTimeoutMS.ArgumentDescription,
                CommandOptionType.SingleValue);

            var fileLogParam = app.Option($"{CLIOptions.UseLogLevelFile.ArgumentName} | {CLIOptions.UseLogLevelFile.ArgumentShortName}",
                CLIOptions.UseLogLevelFile.ArgumentDescription, 
                CommandOptionType.SingleValue);

            var projectNameParam = app.Option($"{CLIOptions.ProjectFileName.ArgumentName} | {CLIOptions.ProjectFileName.ArgumentShortName}",
                CLIOptions.ProjectFileName.ArgumentDescription,
                CommandOptionType.SingleValue);

            var maxConcurrentTestRunnersParam = app.Option($"{CLIOptions.MaxConcurrentTestRunners.ArgumentName} | {CLIOptions.MaxConcurrentTestRunners.ArgumentShortName}",
                CLIOptions.MaxConcurrentTestRunners.ArgumentDescription,
                CommandOptionType.SingleValue);
            
            var thresholdHighParam = app.Option($"{CLIOptions.ThresholdHigh.ArgumentName} | {CLIOptions.ThresholdHigh.ArgumentShortName}",
                CLIOptions.ThresholdHigh.ArgumentDescription,
                CommandOptionType.SingleValue);

            var thresholdLowParam = app.Option($"{CLIOptions.ThresholdLow.ArgumentName} | {CLIOptions.ThresholdLow.ArgumentShortName}",
                CLIOptions.ThresholdLow.ArgumentDescription,
                CommandOptionType.SingleValue);
                
            var thresholdBreakParam = app.Option($"{CLIOptions.ThresholdBreak.ArgumentName} | {CLIOptions.ThresholdBreak.ArgumentShortName}",
                CLIOptions.ThresholdBreak.ArgumentDescription,
                CommandOptionType.SingleValue);

            app.HelpOption("--help | -h | -?");

            app.OnExecute(() => {
                // app started
                var options = new OptionsBuilder().Build(
                    Directory.GetCurrentDirectory(),
                    reporterParam,
                    projectNameParam,
                    timeoutParam,
                    logConsoleParam,
                    fileLogParam,
                    configFilePathParam,
                    maxConcurrentTestRunnersParam,
                    thresholdHighParam,
                    thresholdLowParam,
                    thresholdBreakParam
                    );
                RunStryker(options);
                // Default ExitCode = 0
                return Environment.ExitCode;
            });

            app.Execute(args);
        }
        
        private void RunStryker(StrykerOptions options)
        {
            // start with the stryker header
            PrintStykerASCIIName();

            try
            {  
               IStrykerRunResult results = _stryker.RunMutationTest(options);
               
               if(!results.isScoreAboveThresholdBreak()) 
               {
                   this.HandleBreakingThresholdScore(options, results);
               }
            }
            catch (Exception)
            {
                Environment.ExitCode = 1;
            }
        }

        private void HandleBreakingThresholdScore(StrykerOptions options, IStrykerRunResult results) {
            _logger.LogError("Final mutation score: {0} under breaking threshold value {1}"
            + ", setting exit code to 1 (failure)", results.mutationScore, options.ThresholdOptions.ThresholdBreak);
            _logger.LogError("Improve the mutation score or set the `threshold-break` value lower to prevent this error in the future");
            Environment.ExitCode = 1;
        }

        private void PrintStrykerASCIILogo()
        {
            Console.WriteLine("");
            Chalk.Yellow("             |STRYKER|              "); Console.WriteLine("");
            Chalk.Yellow("       ~control the mutants~        "); Console.WriteLine("");
            Chalk.Blue("           ..####"); Console.Write("@"); Chalk.Blue("####..            "); Console.WriteLine("");
            Chalk.Blue("        .########"); Console.Write("@"); Chalk.Blue("########.         "); Console.WriteLine("");
            Chalk.Blue("      .#####################.       "); Console.WriteLine("");
            Chalk.Blue("     #########"); Chalk.Yellow("#######"); Chalk.Blue("#########      "); Console.WriteLine("");
            Chalk.Blue("    #########"); Chalk.Yellow("##"); Chalk.Blue("#####"); Chalk.Yellow("##"); Chalk.Blue("#########     "); Console.WriteLine("");
            Chalk.Blue("    #########"); Chalk.Yellow("##"); Chalk.Blue("################     "); Console.WriteLine("");
            Chalk.Blue("    "); Console.Write("@@@"); Chalk.Blue("#######"); Chalk.Yellow("#######"); Chalk.Blue("#######"); Console.Write("@@@"); Chalk.Blue("     "); Console.WriteLine("");
            Chalk.Blue("    ################"); Chalk.Yellow("##"); Chalk.Blue("#########     "); Console.WriteLine("");
            Chalk.Blue("    #########"); Chalk.Yellow("##"); Chalk.Blue("#####"); Chalk.Yellow("##"); Chalk.Blue("#########     "); Console.WriteLine("");
            Chalk.Blue("     #########"); Chalk.Yellow("#######"); Chalk.Blue("#########      "); Console.WriteLine("");
            Chalk.Blue("      '######################'      "); Console.WriteLine("");
            Chalk.Blue("        '########"); Console.Write("@"); Chalk.Blue("#########'        "); Console.WriteLine("");
            Chalk.Blue("            '####"); Console.Write("@"); Chalk.Blue("####'            "); Console.WriteLine("");
        }

        private void PrintStykerASCIIName()
        {
            Console.WriteLine(@"
   _____ _              _               _   _ ______ _______ 
  / ____| |            | |             | \ | |  ____|__   __|
 | (___ | |_ _ __ _   _| | _____ _ __  |  \| | |__     | |   
  \___ \| __| '__| | | | |/ / _ \ '__| | . ` |  __|    | |   
  ____) | |_| |  | |_| |   <  __/ |    | |\  | |____   | |   
 |_____/ \__|_|   \__, |_|\_\___|_| (_)|_| \_|______|  |_|   
                   __/ |                                   
                  |___/                                    
");
            Console.WriteLine(@"
Beta version
"); 
        }
    }
}
