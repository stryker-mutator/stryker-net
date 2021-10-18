using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            inputs.TargetFrameworkInput.SuppliedInput = jsonConfig.TargetFramework;

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
            using var streamReader = new StreamReader(configFilePath);
            var json = streamReader.ReadToEnd();
            FileBasedInput input;
            try
            {
                var root = JsonSerializer.Deserialize<FileBasedInputOuter>(json);
                if (root == null)
                {
                    throw new InputException($"The config file at \"{configFilePath}\" could not be parsed.");
                }
                IReadOnlyCollection<string> extraKeys = root.ExtraData != null ? root.ExtraData.Keys : Array.Empty<string>();
                if (extraKeys.Any())
                {
                    var description = extraKeys.Count == 1 ? $"\"{extraKeys.First()}\" was found" : $"several were found: {{ \"{string.Join("\", \"", extraKeys)}\" }}";
                    throw new InputException($"The config file at \"{configFilePath}\" must contain a single \"stryker-config\" root object but {description}.");
                }
                input = root.Input ?? throw new InputException($"The config file at \"{configFilePath}\" must contain a single \"stryker-config\" root object.");
            }
            catch (JsonException jsonException)
            {
                throw new InputException($"The config file at \"{configFilePath}\" could not be parsed.", jsonException.Message);
            }

            EnsureCorrectKeys(configFilePath, input, "stryker-config");

            return input;
        }

        private static void EnsureCorrectKeys(string configFilePath, IExtraData @object, string namePath)
        {
            var properties = @object.GetType().GetProperties().Where(e => e.GetCustomAttribute<JsonPropertyNameAttribute>() != null).ToList();
            foreach (var property in properties.Where(property => property.PropertyType.IsAssignableTo(typeof(IExtraData))))
            {
                var child = (IExtraData)property.GetValue(@object);
                if (child != null)
                {
                    EnsureCorrectKeys(configFilePath, child, $"{namePath}.{property.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name}");
                }
            }
            var extraData = @object.ExtraData;
            IReadOnlyCollection<string> extraKeys = extraData != null ? extraData.Keys : Array.Empty<string>();
            if (extraKeys.Any())
            {
                var allowedKeys = properties.Select(e => e.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name).OrderBy(e => e);
                var description = extraKeys.Count == 1 ? $"\"{extraKeys.First()}\" was found" : $"others were found (\"{string.Join("\", \"", extraKeys)}\")";
                throw new InputException($"The allowed keys for the \"{namePath}\" object are {{ \"{string.Join("\", \"", allowedKeys)}\" }} but {description} in the config file at \"{configFilePath}\"");
            }
        }
    }
}
