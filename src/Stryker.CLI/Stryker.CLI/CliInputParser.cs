using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;

namespace Stryker.CLI
{
    public class CliInput
    {
        public IInputDefinition Input { get; set; }
        public string ArgumentName { get; set; }
        public string ArgumentShortName { get; set; }
        public string ArgumentHint { get; set; }
        public string Description { get; set; }
        public CommandOptionType OptionType { get; set; }
    }

    public static class CliInputParser
    {
        private static readonly IDictionary<string, CliInput> _cliStrykerInputs = new Dictionary<string, CliInput>();
        private static readonly CliInput _configInput;
        private static readonly CliInput _generateJsonConfigInput;

        static CliInputParser()
        {
            _configInput = AddCliOnlyInput("config-file", "f", "Choose the file containing your stryker configuration relative to current working directory. | default: stryker-config.json", argumentHint: "file-path");
            _generateJsonConfigInput = AddCliOnlyInput("init", "i", "Generate a stryker config file with selected plus default options where no option is selected.", optionType: CommandOptionType.SingleOrNoValue, argumentHint: "file-path");

            PrepareCliOptions();
        }

        public static void RegisterCliStrykerInputs(CommandLineApplication app)
        {
            foreach (var (_, value) in _cliStrykerInputs)
            {
                RegisterCliInput(app, value);
            }
        }

        public static string ConfigFilePath(string[] args, CommandLineApplication app)
        {
            RegisterCliInput(app, _configInput);
            return app.Parse(args).SelectedCommand.Options.SingleOrDefault(o => o.LongName == _configInput.ArgumentName)?.Value() ?? "stryker-config.json";
        }

        public static bool GenerateConfigFile(string[] args, CommandLineApplication app)
        {
            RegisterCliInput(app, _generateJsonConfigInput);
            return app.Parse(args).SelectedCommand.Options.SingleOrDefault(o => o.LongName == _generateJsonConfigInput.ArgumentName)?.HasValue() ?? false;
        }

        public static void EnrichFromCommandLineArguments(this StrykerInputs inputs, string[] args, CommandLineApplication app)
        {
            foreach (var commandOption in app.Parse(args).SelectedCommand.Options.Where(option => option.HasValue()))
            {
                var input = _cliStrykerInputs[commandOption.LongName].Input;

                switch (input)
                {
                    case ConcurrencyInput concurrencyInput:
                        concurrencyInput.SuppliedInput = int.Parse(commandOption.Value());
                        inputs.Concurrency = concurrencyInput;
                        break;
                }

                _ = commandOption.OptionType switch
                {
                    CommandOptionType.NoValue => inputs.With(input, commandOption.HasValue()),
                    CommandOptionType.SingleOrNoValue => inputs.With(input, commandOption.HasValue(), commandOption.Value()),
                    CommandOptionType.SingleValue => inputs.With(input, commandOption.Value()),
                    CommandOptionType.MultipleValue => inputs.With(input, commandOption.Values),
                    _ => inputs
                };
            }
        }

        private static void PrepareCliOptions()
        {
            AddCliStrykerInput(new ConcurrencyInput(), "concurrency", "c", argumentHint: "number");

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

        private static void RegisterCliInput(CommandLineApplication app, CliInput option)
        {
            var argumentHint = option.OptionType switch
            {
                CommandOptionType.NoValue => "",
                CommandOptionType.SingleOrNoValue => $"[:<{option.ArgumentHint}>]",
                _ => $" <{option.ArgumentHint}>"
            };

            app.Option($"{option.ArgumentShortName}|{option.ArgumentName}{argumentHint}", option.Description, option.OptionType);
        }

        private static CliInput AddCliOnlyInput(string argumentName, string argumentShortName, string helpText,
            CommandOptionType optionType = CommandOptionType.SingleValue, string argumentHint = null)
        {
            var cliOption = new CliInput
            {
                ArgumentName = $"--{argumentName}",
                ArgumentShortName = $"-{argumentShortName}",
                Description = helpText,
                OptionType = optionType,
                ArgumentHint = argumentHint
            };

            _cliStrykerInputs[argumentName] = cliOption;

            return cliOption;
        }

        private static CliInput AddCliStrykerInput(IInputDefinition input, string argumentName, string argumentShortName,
            CommandOptionType optionType = CommandOptionType.SingleValue, string argumentHint = null)
        {
            var cliOption = new CliInput
            {
                Input = input,
                ArgumentName = $"--{argumentName}",
                ArgumentShortName = $"-{argumentShortName}",
                Description = input.HelpText,
                OptionType = optionType,
                ArgumentHint = argumentHint
            };

            _cliStrykerInputs[argumentName] = cliOption;

            return cliOption;
        }
    }
}
