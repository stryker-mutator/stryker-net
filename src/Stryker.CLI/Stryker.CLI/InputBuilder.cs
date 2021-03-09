using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;

namespace Stryker.CLI
{
    public class InputBuilder
    {
        private readonly ILogger _logger;

        public InputBuilder(ILogger logger)
        {
            _logger = logger;
        }

        public StrykerInputs Build(string[] args, CommandLineApplication app)
        {
            var basePath = Directory.GetCurrentDirectory();

            var inputs = new StrykerInputs(_logger)
            {
                AdditionalTimeoutMSInput = new AdditionalTimeoutMsInput(),
                AzureFileStorageSasInput = new AzureFileStorageSasInput(),
                AzureFileStorageUrlInput = new AzureFileStorageUrlInput(),
                BaselineProviderInput = new BaselineProviderInput(),
                BasePathInput = new BasePathInput(),
                ConcurrencyInput = new ConcurrencyInput(),
                DashboardApiKeyInput = new DashboardApiKeyInput(),
                DashboardUrlInput = new DashboardUrlInput(),
                DevModeInput = new DevModeInput(),
                DiffIgnoreFilePatternsInput = new DiffIgnoreFilePatternsInput(),
                ExcludedMutatorsInput = new ExcludedMutatorsInput(),
                FallbackVersionInput = new FallbackVersionInput(),
                IgnoredMethodsInput = new IgnoredMethodsInput(),
                LanguageVersionInput = new LanguageVersionInput(),
                VerbosityInput = new VerbosityInput(),
                LogToFileInput = new LogToFileInput(),
                ModuleNameInput = new ModuleNameInput(),
                MutateInput = new MutateInput(),
                MutationLevelInput = new MutationLevelInput(),
                OptimizationModeInput = new OptimizationModeInput(),
                OutputPathInput = new OutputPathInput(),
                ProjectInput = new ProjectInput(),
                ProjectNameInput = new ProjectNameInput(),
                ProjectUnderTestNameInput = new ProjectUnderTestNameInput(),
                ProjectVersionInput = new ProjectVersionInput(),
                ReportersInput = new ReportersInput(),
                SinceInput = new SinceInput(),
                SinceBranchInput = new SinceBranchInput(),
                SolutionPathInput = new SolutionPathInput(),
                TestProjectsInput = new TestProjectsInput(),
                ThresholdBreakInput = new ThresholdBreakInput(),
                ThresholdHighInput = new ThresholdHighInput(),
                ThresholdLowInput = new ThresholdLowInput(),
                WithBaselineInput = new WithBaselineInput()
            };

            // basepath gets a default value without user input, but can be overwritten
            inputs.BasePathInput.SuppliedInput = basePath;

            var configFilePath = Path.Combine(basePath, CliInputParser.ConfigFilePath(args, app));
            if (File.Exists(configFilePath))
            {
                inputs.EnrichFromJsonConfig(configFilePath);
            }
            inputs.EnrichFromCommandLineArguments(args, app);

            return inputs;
        }
    }
}
