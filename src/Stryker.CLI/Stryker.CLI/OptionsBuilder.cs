﻿using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.CommandLineUtils;
using Stryker.Core.Options;
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
            CommandOption logLevel,
            CommandOption logToFile,
            CommandOption configFilePath)
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
                GetOption(projectUnderTestNameFilter, CLIOptions.ProjectName),
                GetOption(additionalTimeoutMS, CLIOptions.AdditionalTimeoutMS),
                GetOption(logLevel, CLIOptions.LogLevel),
                GetOption(logToFile, CLIOptions.UseLogFile));
        }
        private T GetOption<T>(CommandOption value, CLIOption<T> defaultValue) where T : IConvertible
        { 
            if (value.HasValue())
            {
                //Convert commandOptionValue to desired type
                return (T)Convert.ChangeType(value.Value(), typeof(T));
            }
            if(config != null && 
                !string.IsNullOrEmpty(config.GetValue(defaultValue.JsonKey, string.Empty).ToString()))
            {
                //Else return config value                
                return config.GetValue<T>(defaultValue.JsonKey);
            }
            //Else return default
            return defaultValue.DefaultValue;
        }
    }
}