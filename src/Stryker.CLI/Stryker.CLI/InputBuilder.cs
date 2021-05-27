using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;

namespace Stryker.CLI
{
    public static class InputBuilder
    {
        public static IStrykerInputs Inputs { get; private set; }

        public static IStrykerInputs InitializeInputs(ILogger logger)
        {
            Inputs = new StrykerInputs(logger)
            {
                AdditionalTimeoutMsInput = new AdditionalTimeoutMsInput(),
                AzureFileStorageSasInput = new AzureFileStorageSasInput(),
                AzureFileStorageUrlInput = new AzureFileStorageUrlInput(),
                BaselineProviderInput = new BaselineProviderInput(),
                BasePathInput = new BasePathInput(),
                ConcurrencyInput = new ConcurrencyInput(),
                DashboardApiKeyInput = new DashboardApiKeyInput(),
                DashboardUrlInput = new DashboardUrlInput(),
                DevModeInput = new DevModeInput(),
                DiffIgnoreFilePatternsInput = new DiffIgnoreFilePatternsInput(),
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
                SolutionPathInput = new SolutionPathInput(),
                TestProjectsInput = new TestProjectsInput(),
                ThresholdBreakInput = new ThresholdBreakInput(),
                ThresholdHighInput = new ThresholdHighInput(),
                ThresholdLowInput = new ThresholdLowInput(),
                WithBaselineInput = new WithBaselineInput()
            };

            return Inputs;
        }

        public static IStrykerInputs Build(string[] args, CommandLineApplication app, CommandLineConfigHandler cmdConfigHandler)
        {
            var basePath = Directory.GetCurrentDirectory();

            // basepath gets a default value without user input, but can be overwritten
            Inputs.BasePathInput.SuppliedInput = basePath;

            var configFilePath = Path.Combine(basePath, cmdConfigHandler.ConfigFilePath(args, app));
            if (File.Exists(configFilePath))
            {
                JsonConfigHandler.DeserializeConfig(configFilePath, Inputs);
            }
            cmdConfigHandler.ReadCommandLineConfig(args, app, Inputs);

            return Inputs;
        }
    }
}
