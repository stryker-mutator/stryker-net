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

        public StrykerInputs Build(string[] args, CommandLineApplication app)
        {
            var basePath = Directory.GetCurrentDirectory();

            var inputs = new StrykerInputs(_logger);

            var configFilePath = Path.Combine(basePath, CliInputParser.ConfigFilePath(args, app));
            if (File.Exists(configFilePath))
            {
                inputs.EnrichFromJsonConfig(configFilePath);
            }
            inputs.EnrichFromCommandLineArguments(args, app);

            return inputs;
        }
    }
}
