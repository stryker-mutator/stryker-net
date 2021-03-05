using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Stryker.Core.Options;

namespace Stryker.CLI
{
    public class OptionsBuilder
    {
        private readonly ILogger _logger;

        public OptionsBuilder(ILogger logger)
        {
            _logger = logger;
        }

        public StrykerOptions Build(string[] args, CommandLineApplication app)
        {
            var basePath = Directory.GetCurrentDirectory();

            var strykerOptions = new InputValidator(_logger);

            var configFilePath = Path.Combine(basePath, CliOptionsParser.ConfigFilePath(args, app));
            if (File.Exists(configFilePath))
            {
                strykerOptions.EnrichFromJsonConfig(configFilePath);
            }

            return strykerOptions.EnrichFromCommandLineArguments(args, app);
        }
    }
}
