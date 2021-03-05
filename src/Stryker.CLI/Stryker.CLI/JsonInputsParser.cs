using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;

namespace Stryker.CLI
{
    public static class JsonInputsParser
    {

        public static StrykerInputs EnrichFromJsonConfig(this StrykerInputs options, string configFilePath)
        {
            var enrichedOptions = options;
            var jsonConfig = LoadJsonConfig(configFilePath);

            enrichedOptions = enrichedOptions
                .With(StrykerOption.SolutionPath, jsonConfig.Solution)
                .With(StrykerOption.Concurrency, jsonConfig.Concurrency)
                .With(StrykerOption.ThresholdHigh, jsonConfig.Thresholds.High)
                .With(StrykerOption.ThresholdLow, jsonConfig.Thresholds.Low)
                .With(StrykerOption.ThresholdBreak, jsonConfig.Thresholds.Break)
                .With(StrykerOption.SolutionPath, jsonConfig.Solution)
                .With(StrykerOption.FallbackVersion, jsonConfig.BaseLine.FallbackVersion)
                .With(StrykerOption.Since, jsonConfig.Since)
                .With(StrykerOption.SinceBranch, jsonConfig.SinceBranch)
                .With(StrykerOption.SinceCommit, jsonConfig.SinceCommit)
                .With(StrykerOption.BaselineProvider, jsonConfig.BaseLine.Provider)
                .With(StrykerOption.FallbackVersion, jsonConfig.BaseLine.FallbackVersion)
                ;

            return enrichedOptions;
        }

        private static FileBasedInputs LoadJsonConfig(string configFilePath)
        {

            var json = new StreamReader(configFilePath).ReadToEnd();

            try
            {
                var settings = new JsonSerializerSettings()
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                };

                JToken strykerSection = JObject.Parse(json)["stryker-config"];

                var configJson = strykerSection.ToString();

                return JsonConvert.DeserializeObject<FileBasedInputs>(configJson, settings);
            }
            catch (JsonReaderException)
            {
                throw new StrykerInputException("Could not find stryker-config section in config file");
            }
            catch (JsonSerializationException ex)
            {
                throw new StrykerInputException(@$"There was a problem with one of the json properties in your stryker config. Path ""{ex.Path}"", message: ""{ex.Message}""");
            }
        }
    }
}
