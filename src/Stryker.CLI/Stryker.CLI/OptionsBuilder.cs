using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using System;
using System.IO;

namespace Stryker.CLI
{
    public class OptionsBuilder
    {
        private readonly ILogger _logger;
        private IConfiguration _config;

        public OptionsBuilder(ILogger logger)
        {
            _logger = logger;
        }

        public StrykerOptions Build(string[] args, CommandLineApplication app)
        {
            var basePath = Directory.GetCurrentDirectory();

            var strykerOptions = new StrykerOptions(basePath);

            var configFilePath = Path.Combine(basePath, CliOptionsParser.ConfigFilePath(args, app));
            if (File.Exists(configFilePath))
            {
                try
                {
                    _config = new ConfigurationBuilder()
                        .SetBasePath(basePath)
                        .AddJsonFile(configFilePath)
                        .Build()
                        .GetSection("stryker-config");
                }
                catch (FormatException formatException)
                {
                    throw new StrykerInputException("The stryker config file was in an incorrect format.", formatException.InnerException.Message);
                }
            }

            return strykerOptions.EnrichWithCommandLineArguments(args, app);
        }
    }
}
