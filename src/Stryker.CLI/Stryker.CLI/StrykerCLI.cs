using Microsoft.Extensions.CommandLineUtils;
using Serilog.Events;
using Stryker.Core;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using System;
using System.IO;

namespace Stryker.CLI
{
    public class StrykerCLI
    {
        private IStrykerRunner _stryker { get; set; }
        public StrykerCLI(IStrykerRunner stryker)
        {
            _stryker = stryker;
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

            var reporterParam = app.Option("--reporter | -r <reporter>",
                "Sets the reporter | Opions [Console (default), RapportOnly]",
                CommandOptionType.SingleValue);

            var logConsoleParam = app.Option("--logConsole | -l <logging>",
                "Sets the logging level | Opions [info (default), warning, debug, trace]",
                CommandOptionType.SingleValue);

            var fileLogParam = app.Option("--logFile | -f",
                "When passed, a logfile will be created for this mutationtest run on trace level", 
                CommandOptionType.NoValue);

            var projectNameParam = app.Option("--project | -p <projectName>",
                @"Used for matching the project references when finding the project to mutate. Example: ""ExampleProject.csproj""",
                CommandOptionType.SingleValue);

            app.HelpOption("--help | -h | -?");

            app.OnExecute(() => {
                // app started
                return RunStryker(reporterParam.Value(), projectNameParam.Value(), logConsoleParam.Value(), fileLogParam.HasValue());
            });

            app.Execute(args);
        }

        private int RunStryker(string reporter, string projectName, string logConsoleLevel, bool logToFile)
        {
            // start with the stryker header
            PrintStykerASCIIName();

            try
            {
                var options = new StrykerOptions(
                    basePath: Directory.GetCurrentDirectory(),
                    reporter: reporter,
                    projectUnderTestNameFilter: projectName,
                    logOptions: new LogOptions(logConsoleLevel, logToFile));

                _stryker.RunMutationTest(options);
            }
            catch (Exception)
            {
                return 1;
            }
            return 0;
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
