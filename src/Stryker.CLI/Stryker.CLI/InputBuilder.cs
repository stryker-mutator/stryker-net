using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Stryker.Core.Options;

namespace Stryker.CLI
{
    public class InputBuilder
    {
        private readonly ILogger _logger;

        public InputBuilder(ILogger logger)
        {
            _logger = logger;
        }

        public StrykerInputs Build(string[] args, CommandLineApplication app, StrykerInputs strykerInputs)
        {
            var basePath = Directory.GetCurrentDirectory();

            // basepath gets a default value without user input, but can be overwritten
            strykerInputs.BasePathInput.SuppliedInput = basePath;

            var configFilePath = Path.Combine(basePath, CliInputParser.ConfigFilePath(args, app));
            if (File.Exists(configFilePath))
            {
                strykerInputs.EnrichFromJsonConfig(configFilePath);
            }
            strykerInputs.EnrichFromCommandLineArguments(args, app);

            return strykerInputs;
        }
    }
}
