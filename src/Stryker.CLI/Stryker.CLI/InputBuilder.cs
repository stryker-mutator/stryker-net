using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Stryker.Core.Options;

namespace Stryker.CLI
{
    public static class InputBuilder
    {
        public static StrykerInputs Build(string[] args, CommandLineApplication app, StrykerInputs strykerInputs, CliInputParser cliInputParser)
        {
            var basePath = Directory.GetCurrentDirectory();

            // basepath gets a default value without user input, but can be overwritten
            strykerInputs.BasePathInput.SuppliedInput = basePath;

            var configFilePath = Path.Combine(basePath, cliInputParser.ConfigFilePath(args, app));
            if (File.Exists(configFilePath))
            {
                strykerInputs.EnrichFromJsonConfig(configFilePath);
            }
            cliInputParser.EnrichFromCommandLineArguments(strykerInputs, args, app);

            return strykerInputs;
        }
    }
}
