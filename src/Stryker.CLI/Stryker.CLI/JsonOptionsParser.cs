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

            //StrykerInput.DevMode,

            //StrykerInput.SolutionPath,

            //StrykerInput.LogToFile,
            //StrykerInput.LogLevel,

            //StrykerInput.MutationLevel,

            //StrykerInput.ThresholdHigh,
            //StrykerInput.ThresholdLow,
            //StrykerInput.ThresholdBreak,

            //StrykerInput.BaselineProvider,
            //StrykerInput.Reporters,
            //StrykerInput.ProjectUnderTestName,
            //StrykerInput.DiffCompare,
            //StrykerInput.DashboardCompare,
            //StrykerInput.DiffTarget,
            //StrykerInput.AdditionalTimeoutMs,
            //StrykerInput.ExcludedMutators,
            //StrykerInput.IgnoredMethods,
            //StrykerInput.Concurrency,
            //StrykerInput.TestRunner,
            //StrykerInput.Mutate,
            //StrykerInput.LanguageVersion,

            //StrykerInput.OptimizationMode,
            //StrykerInput.DisableAbortTestOnFail,
            //StrykerInput.DisableSimultaneousTesting,

            //StrykerInput.TestProjects,
            //StrykerInput.DashboardUrl,
            //StrykerInput.DashboardApiKey,
            //StrykerInput.ProjectName,
            //StrykerInput.ModuleName,
            //StrykerInput.ProjectVersion,
            //StrykerInput.DiffIgnoreFilePatterns,
            //StrykerInput.AzureFileStorageUrl,
            //StrykerInput.AzureFileStorageSas,
            //FStrykerInput.allbackVersion,

            enrichedOptions = enrichedOptions.With(StrykerInput.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerInput., jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerInput.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerInput.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerInput.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerInput.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerInput.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerInput.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerInput.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerInput.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerInput.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerInput.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerInput.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerInput.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerInput.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerInput.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerInput.Concurrency, jsonConfig.Concurrency);
            enrichedOptions = enrichedOptions.With(StrykerInput.Concurrency, jsonConfig.Concurrency);

            return enrichedOptions;
        }

        private static FileBasedOptions LoadJsonConfig(string configFilePath)
        {

            var json = new StreamReader(configFilePath).ReadToEnd();

            try
            {
                JToken strykerSection = JObject.Parse(json)["stryker-config"];

                return strykerSection.ToObject<FileBasedOptions>();
            }
            catch (JsonReaderException)
            {
                throw new StrykerInputException("Could not find stryker-config section in config file");
            }
        }
    }
}
