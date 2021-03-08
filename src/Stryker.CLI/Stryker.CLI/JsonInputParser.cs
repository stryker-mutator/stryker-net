using System.IO;
using Newtonsoft.Json;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;

namespace Stryker.CLI
{
    public static class JsonInputParser
    {
        public static void EnrichFromJsonConfig(this StrykerInputs inputs, string configFilePath)
        {
            var jsonConfig = LoadJsonConfig(configFilePath);

            inputs.ConcurrencyInput = new ConcurrencyInput { SuppliedInput = jsonConfig.Concurrency };
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
