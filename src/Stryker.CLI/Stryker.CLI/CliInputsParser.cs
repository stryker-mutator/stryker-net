using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;

namespace Stryker.CLI
{
    public class CliOption
    {
        public IInputDefinition Input { get; set; }
        public string ArgumentName { get; set; }
        public string ArgumentShortName { get; set; }
        public string ArgumentHint { get; set; }
        public string Description { get; set; }
        public CommandOptionType OptionType { get; set; }
    }

    public static class CliInputsParser
    {
        private static readonly IDictionary<string, CliOption> CliOptions = new Dictionary<string, CliOption>();
        private static readonly CliOption ConfigOption;
        private static readonly CliOption GenerateJsonConfigOption;

        static CliInputsParser()
        {
            ConfigOption = AddCliOption(StrykerOption.None, "config-file", "f",
                "Choose the file containing your stryker configuration relative to current working directory. | default: stryker-config.json", argumentHint: "file-path");
            GenerateJsonConfigOption = AddCliOption(StrykerOption.None, "init", "i",
                "Generate a stryker config file with selected plus default options where no option is selected.", optionType: CommandOptionType.SingleOrNoValue, argumentHint: "file-path");

            PrepareCliOptions();
        }

        public static void RegisterCliOptions(CommandLineApplication app)
        {
            foreach (var (_, value) in CliOptions)
            {
                RegisterCliOption(app, value);
            }
        }

        public static string ConfigFilePath(string[] args, CommandLineApplication app)
        {
            RegisterCliOption(app, ConfigOption);
            return app.Parse(args).SelectedCommand.Options.SingleOrDefault(o => o.LongName == ConfigOption.ArgumentName)?.Value() ?? "stryker-config.json";
        }

        public static bool GenerateConfigFile(string[] args, CommandLineApplication app)
        {
            RegisterCliOption(app, GenerateJsonConfigOption);
            return app.Parse(args).SelectedCommand.Options.SingleOrDefault(o => o.LongName == GenerateJsonConfigOption.ArgumentName)?.HasValue() ?? false;
        }

        public static StrykerInputs EnrichFromCommandLineArguments(this StrykerInputs options, string[] args, CommandLineApplication app)
        {
            var enrichedOptions = options;
            foreach (var option in app.Parse(args).SelectedCommand.Options.Where(option => option.HasValue()))
            {
                var input = CliOptions[option.LongName].Input;

                switch(input)
                {
                    case ConcurrencyInput concurrencyInput:
                        concurrencyInput.SuppliedInput = int.Parse(option.Value());
                        break;
                }

                _ = option.OptionType switch
                {
                    CommandOptionType.NoValue => enrichedOptions.With(input, option.HasValue()),
                    CommandOptionType.SingleOrNoValue => enrichedOptions.With(input, option.HasValue(), option.Value()),
                    CommandOptionType.SingleValue => enrichedOptions.With(input, option.Value()),
                    CommandOptionType.MultipleValue => enrichedOptions.With(input, option.Values),
                    _ => enrichedOptions
                };
            }

            return enrichedOptions;
        }

        private static void PrepareCliOptions()
        {
            AddCliOption(new ConcurrencyInput(), "concurrency", "c", argumentHint: "number");

            AddCliOption(StrykerOption.ThresholdBreak, "break", "b", new ThresholdBreakInput().HelpText, argumentHint: "0-100");

            AddCliOption(StrykerOption.Mutate, "mutate", "m", new MutateInput().HelpText, optionType: CommandOptionType.MultipleValue, argumentHint: "glob-pattern");

            AddCliOption(StrykerOption.SolutionPath, "solution", "s", new SolutionPathInput().HelpText, argumentHint: "file-path");
            AddCliOption(StrykerOption.ProjectUnderTestName, "project", "p", new ProjectUnderTestNameInput().HelpText, argumentHint: "project-name.csproj");
            AddCliOption(StrykerOption.ProjectVersion, "version", "v", new ProjectVersionInput().HelpText);
            AddCliOption(StrykerOption.MutationLevel, "mutation-level", "l", new MutationLevelInput().HelpText);

            AddCliOption(StrykerOption.LogToFile, "log-to-file", "L", new LogToFileInput().HelpText, optionType: CommandOptionType.NoValue);
            AddCliOption(StrykerOption.LogLevel, "verbosity", "V", new LogLevelInput().HelpText);
            AddCliOption(StrykerOption.Reporters, "reporter", "r", new ReportersInput().HelpText, optionType: CommandOptionType.MultipleValue);

            AddCliOption(StrykerOption.Since, "since", "since", new DiffCompareInput().HelpText, optionType: CommandOptionType.SingleOrNoValue, argumentHint: "comittish");
            AddCliOption(StrykerOption.DashboardCompare, "with-baseline", "baseline", new WithBaselineInput().HelpText, optionType: CommandOptionType.SingleOrNoValue, argumentHint: "comittish");

            AddCliOption(StrykerOption.DashboardApiKey, "dashboard-api-key", null, new DashboardApiKeyInput().HelpText);
            AddCliOption(StrykerOption.AzureFileStorageSas, "azure-fileshare-sas", null, new AzureFileStorageSasInput().HelpText);
            AddCliOption(StrykerOption.DevMode, "dev-mode", null, new DevModeInput().HelpText, optionType: CommandOptionType.NoValue);
        }

        private static void RegisterCliOption(CommandLineApplication app, CliOption option)
        {
            var argumentHint = option.OptionType switch
            {
                CommandOptionType.NoValue => "",
                CommandOptionType.SingleOrNoValue => $"[:<{option.ArgumentHint}>]",
                _ => $" <{option.ArgumentHint}>"
            };

            app.Option($"{option.ArgumentShortName}|{option.ArgumentName}{argumentHint}", option.Description, option.OptionType);
        }

        private static CliOption AddCliOption(IInputDefinition input, string argumentName, string argumentShortName,
            CommandOptionType optionType = CommandOptionType.SingleValue, string argumentHint = null)
        {
            var cliOption = new CliOption
            {
                Input = input,
                ArgumentName = $"--{argumentName}",
                ArgumentShortName = $"-{argumentShortName}",
                Description = input.HelpText,
                OptionType = optionType,
                ArgumentHint = argumentHint
            };

            CliOptions[argumentName] = cliOption;

            return cliOption;
        }
    }
}
