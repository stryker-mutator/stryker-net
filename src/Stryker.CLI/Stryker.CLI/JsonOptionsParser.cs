using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;

namespace Stryker.CLI
{
    public class JsonOption
    {
        public StrykerOption InputType { get; }
        public string JsonKey { get; }
        public bool Array { get; }
        public IEnumerable<JsonOption> Children { get; }

        public JsonOption(StrykerOption inputType, string jsonKey, bool array = false, IEnumerable<JsonOption> children = null)
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
                new JsonOption(StrykerOption.DevMode, "dev-mode"),
                new JsonOption(StrykerOption.MutationLevel, "mutation-level"),

                new JsonOption(StrykerOption.Mutate, "mutate"),

                new JsonOption(StrykerOption.SolutionPath, "solution"),
                new JsonOption(StrykerOption.ProjectUnderTestName, "project"),

                new JsonOption(StrykerOption.None, "thresholds", children: new List<JsonOption> {
                    new JsonOption(StrykerOption.ThresholdHigh, "high"),
                    new JsonOption(StrykerOption.ThresholdLow, "low"),
                    new JsonOption(StrykerOption.ThresholdBreak, "break")
                }),

                new JsonOption(StrykerOption.LogToFile, "log-to-file"),
                new JsonOption(StrykerOption.LogLevel, "log-level"),
                new JsonOption(StrykerOption.Reporters, "reporters", array: true),

                new JsonOption(StrykerOption.DiffCompare, "diff"),
                new JsonOption(StrykerOption.DiffTarget, "diff-target"),
                new JsonOption(StrykerOption.DashboardCompare, "dashboard-compare"),

                new JsonOption(StrykerOption.DashboardApiKey, "api-key"),
                new JsonOption(StrykerOption.AzureFileStorageSas, "azure-storage-sas"),

                new JsonOption(StrykerOption.ProjectVersion, "dashboard-version"),
                new JsonOption(StrykerOption.FallbackVersion, "fallback-version"),

                new JsonOption(StrykerOption.Concurrency, "concurrency")
            };
        }

        public static StrykerOptions EnrichFromJsonConfig(this StrykerOptions options, string configFilePath)
        {
            var enrichedOptions = options;
            var configFile = LoadJsonConfig(configFilePath);

            foreach (var option in JsonOptions)
            {
                if (option.InputType is StrykerOption.None)
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
