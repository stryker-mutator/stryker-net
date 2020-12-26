using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;

namespace Stryker.CLI
{
    public class JsonOption
    {
        public StrykerInput InputType { get; }
        public string JsonKey { get; }
        public bool Array { get; }
        public IEnumerable<JsonOption> Children { get; }

        public JsonOption(StrykerInput inputType, string jsonKey, bool array = false, IEnumerable<JsonOption> children = null)
        {
            InputType = inputType;
            JsonKey = jsonKey;
            Array = array;
            Children = children;
        }
    }

    public static class JsonOptionsParser
    {
        private static readonly IEnumerable<JsonOption> JsonOptions;

        static JsonOptionsParser()
        {
            JsonOptions = new List<JsonOption>
            {
                new JsonOption(StrykerInput.DevMode, "dev-mode"),
                new JsonOption(StrykerInput.MutationLevel, "mutation-level"),

                new JsonOption(StrykerInput.Mutate, "mutate"),

                new JsonOption(StrykerInput.SolutionPath, "solution"),
                new JsonOption(StrykerInput.ProjectUnderTestName, "project"),

                new JsonOption(StrykerInput.None, "thresholds", children: new List<JsonOption> {
                    new JsonOption(StrykerInput.ThresholdHigh, "high"),
                    new JsonOption(StrykerInput.ThresholdLow, "low"),
                    new JsonOption(StrykerInput.ThresholdBreak, "break")
                }),

                new JsonOption(StrykerInput.LogToFile, "log-to-file"),
                new JsonOption(StrykerInput.LogLevel, "log-level"),
                new JsonOption(StrykerInput.Reporters, "reporters", array: true),

                new JsonOption(StrykerInput.DiffCompare, "diff"),
                new JsonOption(StrykerInput.DiffTarget, "diff-target"),
                new JsonOption(StrykerInput.DashboardCompare, "dashboard-compare"),

                new JsonOption(StrykerInput.DashboardApiKey, "api-key"),
                new JsonOption(StrykerInput.AzureFileStorageSas, "azure-storage-sas"),

                new JsonOption(StrykerInput.ProjectVersion, "dashboard-version"),
                new JsonOption(StrykerInput.FallbackVersion, "fallback-version"),

                new JsonOption(StrykerInput.Concurrency, "concurrency")
            };
        }

        public static StrykerOptions EnrichFromJsonConfig(this StrykerOptions options, string configFilePath)
        {
            var enrichedOptions = options;
            var configFile = LoadJsonConfig(configFilePath);

            foreach (var option in JsonOptions)
            {
                if (option.InputType is StrykerInput.None)
                {
                    foreach (var child in option.Children)
                    {
                        enrichedOptions = ReadJsonOption(configFile.GetProperty(option.JsonKey), option, options);
                    }
                }
                else
                {
                    enrichedOptions = ReadJsonOption(configFile, option, enrichedOptions);
                }
            }

            return enrichedOptions;
        }

        private static StrykerOptions ReadJsonOption(JsonElement configFile, JsonOption option, StrykerOptions options)
        {
            if (configFile.TryGetProperty(option.JsonKey, out var configValue))
            {
                return option.Array switch
                {
                    true => options.With(option.InputType, ParseArray(configValue)),
                    false => options.With(option.InputType, Parse(configValue)),
                };
            }
            return options;
        }

        private static JsonElement LoadJsonConfig(string configFilePath)
        {
            using var reader = new StreamReader(configFilePath);
            var jsonFile = JsonDocument.Parse(reader.ReadToEnd());

            if (jsonFile.RootElement.TryGetProperty("stryker-config", out var configSection))
            {
                return configSection;
            }

            throw new StrykerInputException("Could not find stryker-config section in config file");
        }

        private static string Parse(JsonElement element)
        {
            return element.GetRawText();
        }

        private static IEnumerable<string> ParseArray(JsonElement element)
        {
            return JsonSerializer.Deserialize<IEnumerable<string>>(element.GetRawText());
        }
    }
}
