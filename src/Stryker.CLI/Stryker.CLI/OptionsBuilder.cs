using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.CommandLineUtils;
using Stryker.Core.Options;
using System.IO;
using System.Linq;
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
            CommandOption logLevel,
            CommandOption logToFile,
            CommandOption configFilePath,
            CommandOption maxConcurrentTestRunners,
            CommandOption thresholdHigh,
            CommandOption thresholdLow,
            CommandOption thresholdBreak)
        {
            var fileLocation = Path.Combine(basePath, GetOption(configFilePath, CLIOptions.ConfigFilePath));
            if (File.Exists(fileLocation))
            {
                config = new ConfigurationBuilder()
                        .SetBasePath(basePath)
                        .AddJsonFile(GetOption(configFilePath, CLIOptions.ConfigFilePath))
                        .Build().GetSection("stryker-config");
            }  
            return new StrykerOptions(
                basePath,
                GetOption(reporter, CLIOptions.Reporter),
                GetOption(projectUnderTestNameFilter, CLIOptions.ProjectFileName),
                GetOption(additionalTimeoutMS, CLIOptions.AdditionalTimeoutMS),
                GetOption(logLevel, CLIOptions.LogLevel),
                GetOption(logToFile, CLIOptions.UseLogLevelFile),
                GetOption(maxConcurrentTestRunners, CLIOptions.MaxConcurrentTestRunners),
                GetOption(thresholdHigh, CLIOptions.ThresholdHigh),
                GetOption(thresholdLow, CLIOptions.ThresholdLow),
                GetOption(thresholdBreak, CLIOptions.ThresholdBreak));
        }
        private T GetOption<T>(CommandOption value, CLIOption<T> defaultValue) where T : IConvertible
        { 
            if (value.HasValue())
            {
                //Convert commandOptionValue to desired type
                return (T)Convert.ChangeType(value.Value(), typeof(T));
            }
            if(config != null)
            {
                // Check if there is a threshold options object and use it when it's available
                string thresholdOptionsSectionKey = "threshold-options";
                if(config.GetSection(thresholdOptionsSectionKey).Exists() && 
                    !string.IsNullOrEmpty(config.GetSection(thresholdOptionsSectionKey).GetValue(defaultValue.JsonKey, string.Empty).ToString()))
                {   
                    return config.GetSection(thresholdOptionsSectionKey).GetValue<T>(defaultValue.JsonKey);
                }
                //Else return config value            
                else if(!string.IsNullOrEmpty(config.GetValue(defaultValue.JsonKey, string.Empty).ToString()))
                {
                    return config.GetValue<T>(defaultValue.JsonKey);
                }                   
            }
            //Else return default
            return defaultValue.DefaultValue;
        }
    }
}