using System;
using System.IO;
using System.IO.Abstractions;
using McMaster.Extensions.CommandLineUtils;
using Stryker.CLI.Logging;
using Stryker.Core;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;

namespace Stryker.CLI
{
    public static class InputBuilder
    {
        /// <summary>
        /// Initializes all stryker inputs.
        /// </summary>
        /// <returns></returns>
        public static IStrykerInputs InitializeInputs()
        {
            var inputs = new StrykerInputs()
            {
                AdditionalTimeoutInput = new AdditionalTimeoutInput(),
                AzureFileStorageSasInput = new AzureFileStorageSasInput(),
                AzureFileStorageUrlInput = new AzureFileStorageUrlInput(),
                BaselineProviderInput = new BaselineProviderInput(),
                BasePathInput = new BasePathInput(),
                ConcurrencyInput = new ConcurrencyInput(),
                DashboardApiKeyInput = new DashboardApiKeyInput(),
                DashboardUrlInput = new DashboardUrlInput(),
                DevModeInput = new DevModeInput(),
                DiffIgnoreChangesInput = new DiffIgnoreChangesInput(),
                DisableBailInput = new DisableBailInput(),
                DisableMixMutantsInput = new DisableMixMutantsInput(),
                ExcludedMutationsInput = new IgnoreMutationsInput(),
                FallbackVersionInput = new FallbackVersionInput(),
                IgnoredMethodsInput = new IgnoreMethodsInput(),
                LanguageVersionInput = new LanguageVersionInput(),
                VerbosityInput = new VerbosityInput(),
                LogToFileInput = new LogToFileInput(),
                ModuleNameInput = new ModuleNameInput(),
                MutateInput = new MutateInput(),
                MutationLevelInput = new MutationLevelInput(),
                CoverageAnalysisInput = new CoverageAnalysisInput(),
                OutputPathInput = new OutputPathInput(),
                ProjectNameInput = new ProjectNameInput(),
                ProjectUnderTestNameInput = new ProjectUnderTestNameInput(),
                ProjectVersionInput = new ProjectVersionInput(),
                ReportersInput = new ReportersInput(),
                SinceInput = new SinceInput(),
                SinceTargetInput = new SinceTargetInput(),
                SolutionInput = new SolutionInput(),
                TestProjectsInput = new TestProjectsInput(),
                ThresholdBreakInput = new ThresholdBreakInput(),
                ThresholdHighInput = new ThresholdHighInput(),
                ThresholdLowInput = new ThresholdLowInput(),
                WithBaselineInput = new WithBaselineInput()
            };

            return inputs;
        }

        /// <summary>
        /// Reads all config from json and console to fill stryker inputs
        /// </summary>
        /// <param name="args">Console app arguments</param>
        /// <param name="app">The console application containing all argument information</param>
        /// <param name="cmdConfigHandler">Mock console config handler</param>
        /// <returns>Filled stryker inputs (except output path)</returns>
        public static IStrykerInputs Build(IStrykerInputs inputs, string[] args, CommandLineApplication app, CommandLineConfigHandler cmdConfigHandler)
        {
            // set basepath
            var basePath = Directory.GetCurrentDirectory();
            inputs.BasePathInput.SuppliedInput = basePath;

            // read config from json and commandline
            var configFilePath = Path.Combine(basePath, cmdConfigHandler.GetConfigFilePath(args, app));
            if (File.Exists(configFilePath))
            {
                JsonConfigHandler.DeserializeConfig(configFilePath, inputs);
            }
            cmdConfigHandler.ReadCommandLineConfig(args, app, inputs);

            return inputs;
        }

        /// <summary>
        /// Creates the needed paths for logging and initializes the logger factory
        /// </summary>
        /// <param name="fileSystem">Mock filesystem</param>
        public static void SetupLogOptions(IStrykerInputs inputs, IFileSystem fileSystem = null)
        {
            fileSystem ??= new FileSystem();
            var basePath = inputs.BasePathInput.SuppliedInput;

            var outputPath = CreateOutputPath(basePath, fileSystem);
            inputs.OutputPathInput.SuppliedInput = outputPath;

            var logLevel = inputs.VerbosityInput.Validate();
            var logToFile = inputs.LogToFileInput.Validate(outputPath);

            ApplicationLogging.ConfigureLogger(logLevel, logToFile, outputPath);
        }

        private static string CreateOutputPath(string basePath, IFileSystem fileSystem)
        {
            var strykerDir = "StrykerOutput";

            var outputPath = Path.Combine(basePath, strykerDir, DateTime.Now.ToString("yyyy-MM-dd.HH-mm-ss"));
            // outputpath should always be created
            fileSystem.Directory.CreateDirectory(FilePathUtils.NormalizePathSeparators(outputPath));

            // add gitignore if it didn't exist yet
            var gitignorePath = FilePathUtils.NormalizePathSeparators(Path.Combine(basePath, strykerDir, ".gitignore"));
            if (!fileSystem.File.Exists(gitignorePath))
            {
                try
                {
                    fileSystem.File.WriteAllText(gitignorePath, "*");
                }
                catch (IOException e)
                {
                    Console.WriteLine($"Could't create gitignore file because of error {e.Message}. \n" +
                        "If you use any diff compare features this may mean that stryker logs show up as changes.");
                }
            }
            return outputPath;
        }
    }
}
