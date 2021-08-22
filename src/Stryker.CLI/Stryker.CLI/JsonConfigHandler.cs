using System.IO;
using Newtonsoft.Json;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;

namespace Stryker.CLI
{
    public static class JsonConfigHandler
    {
        public static void DeserializeConfig(string configFilePath, IStrykerInputs inputs)
        {
            var jsonConfig = LoadJsonConfig(configFilePath);

            // As json values are first in line we can just overwrite all supplied inputs
            inputs.ConcurrencyInput.SuppliedInput = jsonConfig.Concurrency;

            inputs.SinceInput.SuppliedInput =
                jsonConfig.Since is not null &&
                (jsonConfig.Since.Enabled.HasValue && jsonConfig.Since.Enabled.Value);

            inputs.WithBaselineInput.SuppliedInput =
                jsonConfig.Baseline is not null &&
                (jsonConfig.Baseline.Enabled.HasValue && jsonConfig.Baseline.Enabled.Value);

            inputs.BaselineProviderInput.SuppliedInput = jsonConfig.Baseline?.Provider;
            inputs.DiffIgnoreChangesInput.SuppliedInput = jsonConfig.Since?.IgnoreChangesIn;
            inputs.FallbackVersionInput.SuppliedInput = jsonConfig.Baseline?.FallbackVersion;
            inputs.AzureFileStorageUrlInput.SuppliedInput = jsonConfig.Baseline?.AzureFileShareUrl;
            inputs.CoverageAnalysisInput.SuppliedInput = jsonConfig.CoverageAnalysis;
            inputs.DisableBailInput.SuppliedInput = jsonConfig.DisableBail;
            inputs.DisableMixMutantsInput.SuppliedInput = jsonConfig.DisableMixMutants;
            inputs.AdditionalTimeoutInput.SuppliedInput = jsonConfig.AdditionalTimeout;
            inputs.MutateInput.SuppliedInput = jsonConfig.Mutate;
            inputs.MutationLevelInput.SuppliedInput = jsonConfig.MutationLevel;
            inputs.ProjectNameInput.SuppliedInput = jsonConfig.ProjectInfo?.Name;
            inputs.ModuleNameInput.SuppliedInput = jsonConfig.ProjectInfo?.Module;
            inputs.ProjectVersionInput.SuppliedInput = jsonConfig.ProjectInfo?.Version;
            inputs.ReportersInput.SuppliedInput = jsonConfig.Reporters;

            inputs.SinceTargetInput.SuppliedInput = jsonConfig.Since?.Target;
            inputs.SolutionInput.SuppliedInput = jsonConfig.Solution;
            inputs.ProjectUnderTestNameInput.SuppliedInput = jsonConfig.Project;
            inputs.ThresholdBreakInput.SuppliedInput = jsonConfig.Thresholds?.Break;
            inputs.ThresholdHighInput.SuppliedInput = jsonConfig.Thresholds?.High;
            inputs.ThresholdLowInput.SuppliedInput = jsonConfig.Thresholds?.Low;
            inputs.VerbosityInput.SuppliedInput = jsonConfig.Verbosity;
            inputs.LanguageVersionInput.SuppliedInput = jsonConfig.LanguageVersion;
            inputs.TestProjectsInput.SuppliedInput = jsonConfig.TestProjects;
            inputs.TestCaseFilterInput.SuppliedInput = jsonConfig.TestCaseFilter;
            inputs.ExcludedMutationsInput.SuppliedInput = jsonConfig.IgnoreMutations;
        }

        private static FileBasedInput LoadJsonConfig(string configFilePath)
        {
            var json = new StreamReader(configFilePath).ReadToEnd();

            try
            {
                var settings = new JsonSerializerSettings()
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                };

                return JsonConvert.DeserializeObject<FileBasedInputOuter>(json, settings).Input;
            }
            catch (JsonSerializationException ex)
            {
                throw new InputException(@$"There was a problem with one of the json properties in your stryker config. Path ""{ex.Path}"", message: ""{ex.Message}""");
            }
        }
    }
}
