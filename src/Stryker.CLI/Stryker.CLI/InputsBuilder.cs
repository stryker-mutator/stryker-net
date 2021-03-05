using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Stryker.Core.Options;

namespace Stryker.CLI
{
    public class InputsBuilder
    {
        private readonly ILogger _logger;

        public InputsBuilder(ILogger logger)
        {
            _logger = logger;
        }

        public StrykerInputs Build(string[] args, CommandLineApplication app)
        {
            var basePath = Directory.GetCurrentDirectory();

            var strykerInputs = new StrykerInputs(_logger);

            var configFilePath = Path.Combine(basePath, CliInputsParser.ConfigFilePath(args, app));
            if (File.Exists(configFilePath))
            {
                strykerInputs.EnrichFromJsonConfig(configFilePath);
            }

            return strykerInputs.EnrichFromCommandLineArguments(args, app);
        }
    }
}
