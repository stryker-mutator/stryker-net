using System.IO;
using Newtonsoft.Json;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;

namespace Stryker.CLI
{
    public static class JsonInputParser
    {
        public static void EnrichFromJsonConfig(this StrykerInputs inputs, string configFilePath)
        {
            var jsonConfig = LoadJsonConfig(configFilePath);

            // As json values are first in line we can just overwrite all supplied inputs
            inputs.ConcurrencyInput.SuppliedInput = jsonConfig.Concurrency;
            inputs.WithBaselineInput.SuppliedInput = jsonConfig.WithBaseline;
            inputs.BaselineProviderInput.SuppliedInput = jsonConfig.BaseLine?.Provider;
            inputs.WithBaselineInput.SuppliedInput = jsonConfig.BaseLine?.WithBaseline;
            inputs.DiffIgnoreFilePatternsInput.SuppliedInput = jsonConfig.BaseLine?.IgnoreChangedFiles;
            inputs.FallbackVersionInput.SuppliedInput = jsonConfig.BaseLine?.FallbackVersion;
            inputs.AzureFileStorageUrlInput.SuppliedInput = jsonConfig.BaseLine?.AzureFileShareUrl;
            inputs.CoverageAnalysisInput.SuppliedInput = jsonConfig.CoverageAnalysis;
            inputs.DisableBailInput.SuppliedInput = jsonConfig.DisableBail;
            inputs.DisableMixMutantsInput.SuppliedInput = jsonConfig.DisableMixMutants;
            inputs.AdditionalTimeoutMsInput.SuppliedInput = jsonConfig.AdditionalTimeout;
            inputs.MutateInput.SuppliedInput = jsonConfig.Mutate;
            inputs.MutationLevelInput.SuppliedInput = jsonConfig.MutationLevel;
            inputs.ProjectNameInput.SuppliedInput = jsonConfig.ProjectInfo?.Name;
            inputs.ModuleNameInput.SuppliedInput = jsonConfig.ProjectInfo?.Module;
            inputs.ProjectVersionInput.SuppliedInput = jsonConfig.ProjectInfo?.Version;
            inputs.ReportersInput.SuppliedInput = jsonConfig.Reporters;
            inputs.SinceInput.SuppliedInput = jsonConfig.Since;
            inputs.SinceTargetInput.SuppliedInput = jsonConfig.SinceTarget;
            inputs.SolutionPathInput.SuppliedInput = jsonConfig.Solution;
            inputs.ProjectUnderTestNameInput.SuppliedInput = jsonConfig.Project;
            inputs.ThresholdBreakInput.SuppliedInput = jsonConfig.Thresholds?.Break;
            inputs.ThresholdHighInput.SuppliedInput = jsonConfig.Thresholds?.High;
            inputs.ThresholdLowInput.SuppliedInput = jsonConfig.Thresholds?.Low;
            inputs.VerbosityInput.SuppliedInput = jsonConfig.Verbosity;
            inputs.LanguageVersionInput.SuppliedInput = jsonConfig.LanguageVersion;
            inputs.TestProjectsInput.SuppliedInput = jsonConfig.TestProjects;
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
                throw new StrykerInputException(@$"There was a problem with one of the json properties in your stryker config. Path ""{ex.Path}"", message: ""{ex.Message}""");
            }
        }
    }
}
