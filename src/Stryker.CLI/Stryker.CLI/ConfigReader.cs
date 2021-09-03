using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Stryker.Core.Options;

namespace Stryker.CLI
{
    public interface IConfigReader
    {
        void Build(IStrykerInputs inputs, string[] args, CommandLineApplication app, CommandLineConfigHandler cmdConfigHandler);
    }

    public class ConfigReader : IConfigReader
    {
        /// <summary>
        /// Reads all config from json and console to fill stryker inputs
        /// </summary>
        /// <param name="args">Console app arguments</param>
        /// <param name="app">The console application containing all argument information</param>
        /// <param name="cmdConfigHandler">Mock console config handler</param>
        /// <returns>Filled stryker inputs (except output path)</returns>
        public void Build(IStrykerInputs inputs, string[] args, CommandLineApplication app, CommandLineConfigHandler cmdConfigHandler)
        {
            // set basepath
            var basePath = Directory.GetCurrentDirectory();
            inputs.BasePathInput.SuppliedInput = basePath;

            // read config from json and commandline
            var configFilePath = Path.Combine(basePath, cmdConfigHandler.GetConfigFilePath(args, app));
            if (File.Exists(configFilePath))
            {
                JsonConfigHandler.DeserializeConfig(configFilePath, inputs);
            }
            cmdConfigHandler.ReadCommandLineConfig(args, app, inputs);
        }
    }
}
