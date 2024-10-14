using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.CLI.CommandLineConfig;

namespace Stryker.CLI
{
    public interface IConfigBuilder
    {
        void Build(IStrykerInputs inputs, string[] args, CommandLineApplication app, CommandLineConfigReader cmdConfigHandler);
    }

    public class ConfigBuilder : IConfigBuilder
    {
        // Default config file names in order of precedence
        private readonly string[] _defaultConfigFileNames = ["stryker-config.json", "stryker-config.yml", "stryker-config.yaml"];

        /// <summary>
        /// Reads all config from json and console to fill stryker inputs
        /// </summary>
        /// <param name="inputs">Stryker inputs to fill</param>
        /// <param name="args">Console app arguments</param>
        /// <param name="app">The console application containing all argument information</param>
        /// <param name="cmdConfigHandler">Mock console config handler</param>
        /// <returns>Filled stryker inputs (except output path)</returns>
        public void Build(IStrykerInputs inputs, string[] args, CommandLineApplication app, CommandLineConfigReader cmdConfigHandler)
        {
            // set basepath
            var basePath = Directory.GetCurrentDirectory();
            inputs.BasePathInput.SuppliedInput = basePath;

            var finalConfigFilePath = string.Empty;
            var isConfigUserProvided = false;

            // Check user provided config file option
            var configFileOption = cmdConfigHandler.GetConfigFileOption(args, app);
            if (configFileOption != null && configFileOption.HasValue())
            {
                var userConfigFilePath = Path.Combine(basePath, configFileOption.Value()!);
                if (!File.Exists(userConfigFilePath))
                {
                    // Throw if user provided config file does not exist
                    throw new InputException($"Config file not found at {userConfigFilePath}");
                }

                finalConfigFilePath = userConfigFilePath;
                isConfigUserProvided = true;
            }

            // Attempt to read config with default names if user didn't provide one
            if (!isConfigUserProvided)
            {
                foreach (var defaultConfigFileName in _defaultConfigFileNames)
                {
                    var defaultConfigFilePath = Path.Combine(basePath, defaultConfigFileName);
                    if (File.Exists(defaultConfigFileName))
                    {
                        finalConfigFilePath = defaultConfigFilePath;
                        break;
                    }
                }
            }

            // Deserialize if final path is filled in
            if (!string.IsNullOrWhiteSpace(finalConfigFilePath))
            {
                FileConfigReader.DeserializeConfig(finalConfigFilePath, inputs);
            }

            cmdConfigHandler.ReadCommandLineConfig(args, app, inputs);
        }
    }
}
