using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;

namespace Stryker.CLI
{
    public static class JsonInputsParser
    {

        public static StrykerInputs EnrichFromJsonConfig(this StrykerInputs inputs, string configFilePath)
        {
            var enrichedInputs = inputs;
            var jsonConfig = LoadJsonConfig(configFilePath);

            enrichedInputs.Concurrency.SuppliedInput = jsonConfig.Concurrency;

            return enrichedInputs;
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
