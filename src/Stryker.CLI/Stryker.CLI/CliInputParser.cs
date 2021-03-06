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
        private static readonly IDictionary<string, CliInput> _strykerInputs = new Dictionary<string, CliInput>();
        private static readonly CliInput _configFileInput;
        private static readonly CliInput _generateConfigFileInput;

        static CliInputParser()
        {
            _configFileInput = AddCliOnlyInput("config-file", "f", "Choose the file containing your stryker configuration relative to current working directory. | default: stryker-config.json", argumentHint: "file-path");
            _generateConfigFileInput = AddCliOnlyInput("init", "i", "Generate a stryker config file with selected plus default options where no option is selected.", optionType: CommandOptionType.SingleOrNoValue, argumentHint: "file-path");

            PrepareCliOptions();
        }

        public static void RegisterStrykerInputs(CommandLineApplication app)
        {
            foreach (var (_, value) in _strykerInputs)
            {
                RegisterCliInput(app, value);
            }
        }

        public static string ConfigFilePath(string[] args, CommandLineApplication app)
        {
            RegisterCliInput(app, _configFileInput);
            return app.Parse(args).SelectedCommand.Options.SingleOrDefault(o => o.LongName == _configFileInput.ArgumentName)?.Value() ?? "stryker-config.json";
        }

        public static bool GenerateConfigFile(string[] args, CommandLineApplication app)
        {
            RegisterCliInput(app, _generateConfigFileInput);
            return app.Parse(args).SelectedCommand.Options.SingleOrDefault(o => o.LongName == _generateConfigFileInput.ArgumentName)?.HasValue() ?? false;
        }

        public static void EnrichFromCommandLineArguments(this StrykerInputs strykerInputs, string[] args, CommandLineApplication app)
        {
            foreach (var cliInput in app.Parse(args).SelectedCommand.Options.Where(option => option.HasValue()))
            {
                strykerInputs = cliInput.OptionType switch
                {
                    CommandOptionType.NoValue => ParseNoValue(cliInput, strykerInputs),
                    CommandOptionType.SingleOrNoValue => ParseSingleOrNoValue(cliInput, strykerInputs),
                    CommandOptionType.SingleValue => ParseSingleValue(cliInput, strykerInputs),
                    CommandOptionType.MultipleValue => ParseMultiValue(cliInput, strykerInputs),
                    _ => strykerInputs
                };
            }
        }

        private static StrykerInputs ParseNoValue(CommandOption cliInput, StrykerInputs strykerInputs)
        {
            var strykerInput = GetStrykerInput(cliInput);
            switch (strykerInput)
            {
                case DevModeInput devModeInput:
                    devModeInput.SuppliedInput = true;
                    strykerInputs.DevMode = devModeInput;
                    break;
            }
            return strykerInputs;
        }

        private static StrykerInputs ParseSingleValue(CommandOption cliInput, StrykerInputs strykerInputs)
        {
            var strykerInput = GetStrykerInput(cliInput);
            switch (strykerInput)
            {
                case ConcurrencyInput concurrencyInput:
                    concurrencyInput.SuppliedInput = int.Parse(cliInput.Value());
                    strykerInputs.Concurrency = concurrencyInput;
                    break;
            }
            return strykerInputs;
        }

        private static StrykerInputs ParseSingleOrNoValue(CommandOption cliInput, StrykerInputs strykerInputs)
        {
            var strykerInput = GetStrykerInput(cliInput);
            switch (strykerInput)
            {
                case SinceInput sinceInput:
                    sinceInput.SuppliedInput = true;
                    strykerInputs.SinceInput = sinceInput;
                    strykerInputs.SinceTargetInput = new SinceTargetInput();
                    strykerInputs.SinceTargetInput.SuppliedInput = cliInput.Value();
                    break;
            }
            return strykerInputs;
        }

        private static StrykerInputs ParseMultiValue(CommandOption cliInput, StrykerInputs strykerInputs)
        {
            var strykerInput = GetStrykerInput(cliInput);
            switch (strykerInput)
            {
                case ReportersInput reportersInput:
                    reportersInput.SuppliedInput = cliInput.Values;
                    strykerInputs.ReportersInput = reportersInput;
                    break;
            }
            return strykerInputs;
        }

        private static IInputDefinition GetStrykerInput(CommandOption cliInput) => _strykerInputs[cliInput.LongName].Input;

        private static void PrepareCliOptions()
        {
            AddCliInput(new ConcurrencyInput(), "concurrency", "c", argumentHint: "number");

            AddCliInput(new ThresholdBreakInput(), "break", "b", argumentHint: "0-100");

            AddCliInput(new MutateInput(), "mutate", "m", optionType: CommandOptionType.MultipleValue, argumentHint: "glob-pattern");

            AddCliInput(new SolutionPathInput(), "solution", "s", argumentHint: "file-path");
            AddCliInput(new ProjectUnderTestNameInput(), "project", "p", argumentHint: "project-name.csproj");
            AddCliInput(new ProjectVersionInput(), "version", "v");
            AddCliInput(new MutationLevelInput(), "mutation-level", "l");

            AddCliInput(new LogToFileInput(), "log-to-file", "L", optionType: CommandOptionType.NoValue);
            AddCliInput(new LogLevelInput(), "verbosity", "V");
            AddCliInput(new ReportersInput(), "reporter", "r", optionType: CommandOptionType.MultipleValue);

            AddCliInput(new SinceInput(), "since", "since", optionType: CommandOptionType.SingleOrNoValue, argumentHint: "comittish");
            AddCliInput(new WithBaselineInput(), "with-baseline", "baseline", optionType: CommandOptionType.SingleOrNoValue, argumentHint: "comittish");

            AddCliInput(new DashboardApiKeyInput(), "dashboard-api-key", null);
            AddCliInput(new AzureFileStorageSasInput(), "azure-fileshare-sas", null);
            AddCliInput(new DevModeInput(), "dev-mode", null, optionType: CommandOptionType.NoValue);
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

            _strykerInputs[argumentName] = cliOption;

            return cliOption;
        }

        private static CliInput AddCliInput(IInputDefinition input, string argumentName, string argumentShortName,
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

            _strykerInputs[argumentName] = cliOption;

            return cliOption;
        }
    }
}
