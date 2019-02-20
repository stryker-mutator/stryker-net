using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Stryker.Core.Exceptions;
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
                basePath: basePath,
                reporters: GetOption(reporter.Value(), CLIOptions.Reporters),
                projectUnderTestNameFilter: GetOption(projectUnderTestNameFilter.Value(), CLIOptions.ProjectFileName),
                additionalTimeoutMS: GetOption(additionalTimeoutMS.Value(), CLIOptions.AdditionalTimeoutMS),
                excludedMutations: GetOption(excludedMutations.Value(), CLIOptions.ExcludedMutations),
                logLevel: GetOption(logLevel.Value(), CLIOptions.LogLevel),
                logToFile: GetOption(logToFile.HasValue(), CLIOptions.LogToFile),
                devMode: GetOption(devMode.HasValue(), CLIOptions.DevMode),
                maxConcurrentTestRunners: GetOption(maxConcurrentTestRunners.Value(), CLIOptions.MaxConcurrentTestRunners),
                thresholdHigh: GetOption(thresholdHigh.Value(), CLIOptions.ThresholdHigh),
                thresholdLow: GetOption(thresholdLow.Value(), CLIOptions.ThresholdLow),
                thresholdBreak: GetOption(thresholdBreak.Value(), CLIOptions.ThresholdBreak),
                filesToExclude: GetOption(filesToExclude.Value(), CLIOptions.FilesToExclude));
        }

        private T GetOption<V, T>(V cliValue, CLIOption<T> option)
        {
            if (cliValue != null)
            {
                // Convert the cliValue string to the disired type
                return ConvertTo(cliValue, option);
            }
            else if (config != null)
            {
                // Try to get the value from the config file
                if (typeof(IEnumerable).IsAssignableFrom(typeof(T)) && typeof(T) != typeof(string))
                {
                    return config.GetSection(option.JsonKey).Get<T>();
                }
                else
                {
                    string configValue = config.GetValue(option.JsonKey, string.Empty).ToString();
                    if (!string.IsNullOrEmpty(configValue))
                    {
                        return ConvertTo(configValue, option);
                    }
                }
            }

            // Unable to get value from user, return default value
            return option.DefaultValue;
        }

        private T ConvertTo<V, T>(V value, CLIOption<T> option)
        {
            try
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
            catch (Exception ex)
            {
                throw new StrykerInputException("A value passed to an option was not valid.", $@"The option {option.ArgumentName} with value {value} is not valid.
Hint:
{ex.Message}");
            }

        }
    }
}