using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Stryker.Core.Options;
using System;
using System.Collections;
using System.IO;

namespace Stryker.CLI
{
    public class OptionsBuilder
    {
        private IConfiguration config = null;

        public StrykerOptions Build(
            string basePath,
            CommandOption reporter,
            CommandOption projectUnderTestNameFilter,
            CommandOption additionalTimeoutMS,
            CommandOption excludedMutations,
            CommandOption logLevel,
            CommandOption logToFile,
            CommandOption devMode,
            CommandOption configFilePath,
            CommandOption maxConcurrentTestRunners,
            CommandOption thresholdHigh,
            CommandOption thresholdLow,
            CommandOption thresholdBreak,
            CommandOption filesToExclude)
        {
            var fileLocation = Path.Combine(basePath, GetOption(configFilePath.Value(), CLIOptions.ConfigFilePath));
            if (File.Exists(fileLocation))
            {
                config = new ConfigurationBuilder()
                        .SetBasePath(basePath)
                        .AddJsonFile(fileLocation)
                        .Build().GetSection("stryker-config");
            }

            return new StrykerOptions(
                basePath,
                GetOption(reporter.Value(), CLIOptions.Reporters),
                GetOption(projectUnderTestNameFilter.Value(), CLIOptions.ProjectFileName),
                GetOption(additionalTimeoutMS.Value(), CLIOptions.AdditionalTimeoutMS),
                GetOption(excludedMutations.Value(), CLIOptions.ExcludedMutations),
                GetOption(logLevel.Value(), CLIOptions.LogLevel),
                GetOption(logToFile.Value(), CLIOptions.LogToFile),
                GetOption(devMode.Value(), CLIOptions.DevMode),
                GetOption(maxConcurrentTestRunners.Value(), CLIOptions.MaxConcurrentTestRunners),
                GetOption(thresholdHigh.Value(), CLIOptions.ThresholdHigh),
                GetOption(thresholdLow.Value(), CLIOptions.ThresholdLow),
                GetOption(thresholdBreak.Value(), CLIOptions.ThresholdBreak),
                GetOption(filesToExclude.Value(), CLIOptions.FilesToExclude));
        }

        private T GetOption<V, T>(V cliValue, CLIOption<T> defaultValue)
        {
            if (defaultValue.ValueType == CommandOptionType.NoValue && cliValue is string cliNoValueValue && cliNoValueValue == "on")
            {
                // When the value of a NoValue type is passed it somehow returns the string "on". This means the argument was passed and the value should be true.
                return (T)(object)true;
            }
            if (cliValue != null)
            {
                // Convert the cliValue string to the disired type
                return ConvertTo<V, T>(cliValue);
            }
            else if (config != null)
            {
                // Try to get the value from the config file
                if (typeof(IEnumerable).IsAssignableFrom(typeof(T)) && typeof(T) != typeof(string))
                {
                    return config.GetSection(defaultValue.JsonKey).Get<T>();
                }
                else
                {
                    string configValue = config.GetValue(defaultValue.JsonKey, string.Empty).ToString();
                    if (!string.IsNullOrEmpty(configValue))
                    {
                        return ConvertTo<string, T>(configValue);
                    }
                }
            }

            // Unable to get value from user, return default value
            return defaultValue.DefaultValue;
        }

        private T ConvertTo<V, T>(V value)
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)) && typeof(T) != typeof(string))
            {
                // Convert json array to IEnummerable of desired type
                var list = JsonConvert.DeserializeObject<T>(value as string);
                return list;
            }
            else
            {
                // Convert value to desired type
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }
    }
}