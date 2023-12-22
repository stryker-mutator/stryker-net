using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Stryker.CLI
{
    public static class FileConfigReader
    {
        public static void DeserializeConfig(string configFilePath, IStrykerInputs inputs)
        {
            var config = LoadConfig(configFilePath);

            // As json values are first in line we can just overwrite all supplied inputs
            inputs.ConcurrencyInput.SuppliedInput = config.Concurrency;


            if (config.Since is not null)
            {
                // Since is implicitly enabled when the object exists in the file config
                inputs.SinceInput.SuppliedInput = config.Since.Enabled ?? true;

                inputs.SinceTargetInput.SuppliedInput = config.Since.Target;
                inputs.DiffIgnoreChangesInput.SuppliedInput = config.Since.IgnoreChangesIn;
            }

            if (config.Baseline is not null)
            {
                // Baseline is implicitly enabled when the object exists in the file config
                inputs.WithBaselineInput.SuppliedInput = config.Baseline.Enabled ?? true;

                inputs.BaselineProviderInput.SuppliedInput = config.Baseline.Provider;
                inputs.FallbackVersionInput.SuppliedInput = config.Baseline.FallbackVersion;
                inputs.AzureFileStorageUrlInput.SuppliedInput = config.Baseline.AzureFileShareUrl;
            }


            inputs.CoverageAnalysisInput.SuppliedInput = config.CoverageAnalysis;
            inputs.DisableBailInput.SuppliedInput = config.DisableBail;
            inputs.DisableMixMutantsInput.SuppliedInput = config.DisableMixMutants;
            inputs.AdditionalTimeoutInput.SuppliedInput = config.AdditionalTimeout;
            inputs.MutateInput.SuppliedInput = config.Mutate;
            inputs.MutationLevelInput.SuppliedInput = config.MutationLevel;
            inputs.ProjectNameInput.SuppliedInput = config.ProjectInfo?.Name;
            inputs.ModuleNameInput.SuppliedInput = config.ProjectInfo?.Module;
            inputs.ProjectVersionInput.SuppliedInput = config.ProjectInfo?.Version;
            inputs.ReportersInput.SuppliedInput = config.Reporters;

            inputs.SolutionInput.SuppliedInput = config.Solution;
            inputs.TargetFrameworkInput.SuppliedInput = config.TargetFramework;

            inputs.SourceProjectNameInput.SuppliedInput = config.Project;
            inputs.ThresholdBreakInput.SuppliedInput = config.Thresholds?.Break;
            inputs.ThresholdHighInput.SuppliedInput = config.Thresholds?.High;
            inputs.ThresholdLowInput.SuppliedInput = config.Thresholds?.Low;
            inputs.VerbosityInput.SuppliedInput = config.Verbosity;
            inputs.LanguageVersionInput.SuppliedInput = config.LanguageVersion;
            inputs.TestProjectsInput.SuppliedInput = config.TestProjects;
            inputs.TestCaseFilterInput.SuppliedInput = config.TestCaseFilter;
            inputs.DashboardUrlInput.SuppliedInput = config.DashboardUrl;
            inputs.IgnoreMutationsInput.SuppliedInput = config.IgnoreMutations;
            inputs.IgnoredMethodsInput.SuppliedInput = config.IgnoreMethods;

            inputs.ReportFileNameInput.SuppliedInput = config.ReportFileName;
            inputs.BreakOnInitialTestFailureInput.SuppliedInput = config.BreakOnInitialTestFailure;
        }

        private static FileBasedInput LoadConfig(string configFilePath)
        {
            using var streamReader = new StreamReader(configFilePath);
            var fileContents = streamReader.ReadToEnd();

            FileBasedInput input;
            try
            {
                FileBasedInputOuter root;
                if (configFilePath.EndsWith(".yaml") || configFilePath.EndsWith(".yml"))
                {
                    root = DeserializeYaml(fileContents);
                }
                else if (configFilePath.EndsWith(".json"))
                {
                    root = DeserializeJson(fileContents);
                }
                else
                {
                    throw new InputException($"Unkown file type for config file at \"{configFilePath}\"");
                }

                if (root == null)
                {
                    throw new InputException($"The config file at \"{configFilePath}\" could not be parsed.");
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

        private static FileBasedInputOuter DeserializeJson(string json)
        {
            FileBasedInputOuter root;
            var serializerOptions = new JsonSerializerOptions { ReadCommentHandling = JsonCommentHandling.Skip };
            root = JsonSerializer.Deserialize<FileBasedInputOuter>(json, serializerOptions);
            return root;
        }

        private static FileBasedInputOuter DeserializeYaml(string yaml)
        {
            FileBasedInputOuter root;
            var yamldeserializer = new DeserializerBuilder()
                                    .IgnoreUnmatchedProperties()
                                    .WithNamingConvention(HyphenatedNamingConvention.Instance)
                                    .Build();

            root = yamldeserializer.Deserialize<FileBasedInputOuter>(yaml);
            return root;
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
