using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;

namespace Stryker.CLI
{
    public class JsonOption
    {
        public StrykerInput InputType { get; set; }
        public string JsonKey { get; set; }
        public bool MultipleValue { get; set; }

        public JsonOption(StrykerInput inputType, string jsonKey, bool multipleValue = false)
        {
            InputType = inputType;
            JsonKey = jsonKey;
            MultipleValue = multipleValue;
        }
    }

    public static class JsonOptionsParser
    {
        private static readonly IEnumerable<JsonOption> JsonOptions = PrepareJsonOptions();

        public static StrykerOptions EnrichFromJsonConfig(this StrykerOptions options, string configFilePath)
        {
            var enrichedOptions = options;
            var config = LoadJsonConfig(configFilePath);

            foreach (var option in JsonOptions)
            {
                if (config.TryGetProperty(option.JsonKey, out var configValue))
                {
                    enrichedOptions = option.MultipleValue switch
                    {
                        true => enrichedOptions.With(option.InputType, ParseMultipleValue(configValue)),
                        false => enrichedOptions.With(option.InputType, Parse(configValue)),
                    };
                }
            }

            return enrichedOptions;
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

        private static IEnumerable<JsonOption> PrepareJsonOptions()
        {
            return new List<JsonOption>
            {
                new JsonOption(StrykerInput.Concurrency, "concurrency")
            };
            //AddCliOption(StrykerInput.ThresholdBreak, "break", "b", new ThresholdBreakInput().HelpText, argumentHint: "number");
            //AddCliOption(StrykerInput.DevMode, "dev-mode", "dev", new DevModeInput().HelpText, optionType: CommandOptionType.NoValue);

            //AddCliOption(StrykerInput.Mutate, "mutate", "m", new MutateInput().HelpText, optionType: CommandOptionType.MultipleValue, argumentHint: "glob-pattern");

            //AddCliOption(StrykerInput.SolutionPath, "solution-path", "s", new SolutionPathInput().HelpText, argumentHint: "file-path");
            //AddCliOption(StrykerInput.ProjectUnderTestName, "project-file", "p", new ProjectUnderTestNameInput().HelpText, argumentHint: "project-name");
            //AddCliOption(StrykerInput.MutationLevel, "mutation-level", "level", new MutationLevelInput().HelpText);

            //AddCliOption(StrykerInput.LogToFile, "log-file", "f", new LogToFileInput().HelpText, optionType: CommandOptionType.NoValue);
            //AddCliOption(StrykerInput.LogLevel, "log-level", "l", new LogLevelInput().HelpText);
            //AddCliOption(StrykerInput.Reporters, "reporter", "r", new ReportersInput().HelpText, optionType: CommandOptionType.MultipleValue);

            //AddCliOption(StrykerInput.DiffCompare, "diff", "diff", new DiffCompareInput().HelpText, optionType: CommandOptionType.SingleOrNoValue, argumentHint: "comittish");
            //AddCliOption(StrykerInput.DashboardCompare, "dashboard-compare", "compare", new DashboardCompareInput().HelpText, optionType: CommandOptionType.SingleOrNoValue, argumentHint: "comittish");

            //AddCliOption(StrykerInput.DashboardApiKey, "dashboard-api-key", "dk", new DashboardApiKeyInput().HelpText);
            //AddCliOption(StrykerInput.AzureFileStorageSas, "azure-storage-sas", "sas", new AzureFileStorageSasInput().HelpText);

            //AddCliOption(StrykerInput.ProjectVersion, "dashboard-version", "dv", new ProjectVersionInput().HelpText);
            //AddCliOption(StrykerInput.FallbackVersion, "fallback-version", "fv", new FallbackVersionInput().HelpText, argumentHint: "comittish");

            //AddCliOption(StrykerInput.Concurrency, "concurrency", "c", new ConcurrencyInput().HelpText, argumentHint: "number");
        }

        private static string Parse(JsonElement element)
        {
            return element.GetRawText();
        }

        private static IEnumerable<string> ParseMultipleValue(JsonElement element)
        {
            return JsonSerializer.Deserialize<IEnumerable<string>>(element.GetRawText());
        }
    }
}
