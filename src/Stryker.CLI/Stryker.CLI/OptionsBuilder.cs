using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.CommandLineUtils;
using Stryker.Core.Options;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Collections;

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
                GetOption(reporter.Value(), CLIOptions.Reporter),
                GetOption(projectUnderTestNameFilter.Value(), CLIOptions.ProjectFileName),
                GetOption(additionalTimeoutMS.Value(), CLIOptions.AdditionalTimeoutMS),
                GetOption(excludedMutations.Value(), CLIOptions.ExcludedMutations),
                GetOption(logLevel.Value(), CLIOptions.LogLevel),
                GetOption(logToFile.Value(), CLIOptions.UseLogLevelFile),
                GetOption(maxConcurrentTestRunners.Value(), CLIOptions.MaxConcurrentTestRunners),
                GetOption(thresholdHigh.Value(), CLIOptions.ThresholdHigh),
                GetOption(thresholdLow.Value(), CLIOptions.ThresholdLow),
                GetOption(thresholdBreak.Value(), CLIOptions.ThresholdBreak),
                GetOption(filesToExclude.Value(), CLIOptions.FilesToExclude));
        }

        private T GetOption<T>(string value, CLIOption<T> defaultValue)
        {
            if (value != null)
            {
                return ConvertTo<T>(value);
            }
            if (config != null)
            {
                // Check if there is a threshold options object and use it when it's available
                string thresholdOptionsSectionKey = "threshold-options";
                if (config.GetSection(thresholdOptionsSectionKey).Exists() && 
                    !string.IsNullOrEmpty(config.GetSection(thresholdOptionsSectionKey).GetValue(defaultValue.JsonKey, string.Empty).ToString()))
                {
                    return config.GetSection(thresholdOptionsSectionKey).GetValue<T>(defaultValue.JsonKey);
                }
                else if (config.GetSection("files-to-exclude").Exists() &&
                         config.GetSection(defaultValue.JsonKey).Get<List<string>>() != null)
                {
                    var data = JsonConvert.SerializeObject(config.GetSection("files-to-exclude").Get<List<string>>());
                    return (T) Convert.ChangeType(data, typeof(T));
                }
                //Else return config value            
                else if (!string.IsNullOrEmpty(config.GetValue(defaultValue.JsonKey, string.Empty).ToString()))
                {
                    return config.GetValue<T>(defaultValue.JsonKey);
                }
            }

            //Else return default
            return defaultValue.DefaultValue;
        }

        private T ConvertTo<T>(string value)
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)) && typeof(T) != typeof(String))
            {
                //Convert commandOptionValue to list of desired type
                var list = JsonConvert.DeserializeObject<T>(value);
                return list;
            }
            else
            {
                //Convert commandOptionValue to desired type
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }
    }
}