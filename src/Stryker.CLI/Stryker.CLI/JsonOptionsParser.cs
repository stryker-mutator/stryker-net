using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;

namespace Stryker.CLI
{
    public static class JsonOptionsParser
    {

        public static StrykerOptions EnrichFromJsonConfig(this StrykerOptions options, string configFilePath)
        {
            var enrichedOptions = options;
            var jsonConfig = LoadJsonConfig(configFilePath);

            enrichedOptions = enrichedOptions
                .With(StrykerOption.Concurrency, jsonConfig.Concurrency)
                .With(StrykerOption.SolutionPath, jsonConfig.Solution);
            enrichedOptions = enrichedOptions.With(StrykerOption.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerOption.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerOption.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerOption.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerOption.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerOption.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerOption.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerOption.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerOption.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerOption.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerOption.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerOption.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerOption.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerOption.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerOption.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerOption.Concurrency, jsonConfig.Concurrency);

            return enrichedOptions;
        }

        private static FileBasedOptions LoadJsonConfig(string configFilePath)
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

                return JsonConvert.DeserializeObject<FileBasedOptions>(configJson, settings);
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
