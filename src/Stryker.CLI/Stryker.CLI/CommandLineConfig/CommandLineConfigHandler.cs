using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;

namespace Stryker.CLI
{
    public class CommandLineConfigHandler
    {
        private readonly IDictionary<string, CliInput> _cliInputs = new Dictionary<string, CliInput>();
        private readonly CliInput _configFileInput;

        public CommandLineConfigHandler()
        {
            _configFileInput = AddCliOnlyInput("config-file", "f", "Choose the file containing your stryker configuration relative to current working directory. | default: stryker-config.json", argumentHint: "file-path");
        }

        public void RegisterCommandLineOptions(CommandLineApplication app, IStrykerInputs inputs)
        {
            PrepareCliOptions(inputs);

            foreach (var (_, value) in _cliInputs)
            {
                RegisterCliInput(app, value);
            }
        }

        public string GetConfigFilePath(string[] args, CommandLineApplication app)
        {
            var commands = app.Parse(args);
            var option = commands.SelectedCommand.Options.SingleOrDefault(o => o.LongName == _configFileInput.ArgumentName);
            return option?.Value() ?? "stryker-config.json";
        }

        public void ReadCommandLineConfig(string[] args, CommandLineApplication app, IStrykerInputs inputs)
        {
            foreach (var cliInput in app.Parse(args).SelectedCommand.Options.Where(option => option.HasValue()))
            {
                var strykerInput = GetStrykerInput(cliInput);

                switch (cliInput.OptionType)
                {
                    case CommandOptionType.NoValue:
                        HandleNoValue((IInput<bool?>)strykerInput);
                        break;
                    case CommandOptionType.MultipleValue:
                        HandleMultiValue(cliInput, (IInput<IEnumerable<string>>)strykerInput);
                        break;
                    case CommandOptionType.SingleOrNoValue:
                        HandleSingleOrNoValue(strykerInput, cliInput, inputs);
                        break;
                }

                switch (strykerInput)
                {
                    case IInput<string> stringInput:
                        HandleSingleStringValue(cliInput, stringInput);
                        break;
                    case IInput<int?> nullableIntInput:
                        HandleSingleIntValue(cliInput, nullableIntInput);
                        break;
                    case IInput<int> intInput:
                        HandleSingleIntValue(cliInput, (IInput<int?>)intInput);
                        break;
                }
            }
        }

        private static void HandleNoValue(IInput<bool?> strykerInput)
        {
            strykerInput.SuppliedInput = true;
        }

        private static void HandleSingleStringValue(CommandOption cliInput, IInput<string> strykerInput)
        {
            strykerInput.SuppliedInput = cliInput.Value();
        }

        private static void HandleSingleIntValue(CommandOption cliInput, IInput<int?> strykerInput)
        {
            if (int.TryParse(cliInput.Value(), out var value))
            {
                strykerInput.SuppliedInput = value;
            }
            else
            {
                throw new InputException($"Unexpected value for argument {cliInput.LongName}:{cliInput.Value()}. Expected type to be integer");
            }
        }

        private static void HandleSingleOrNoValue(IInput strykerInput, CommandOption cliInput, IStrykerInputs inputs)
        {
            switch (strykerInput)
            {
                // handle single or no value inputs
                case SinceInput sinceInput:
                    sinceInput.SuppliedInput = true;
                    inputs.SinceTargetInput.SuppliedInput = cliInput.Value();
                    break;
                case WithBaselineInput withBaselineInput:
                    withBaselineInput.SuppliedInput = true;
                    inputs.BaselineProviderInput.SuppliedInput = cliInput.Value();
                    break;
            }
        }

        private static void HandleMultiValue(CommandOption cliInput, IInput<IEnumerable<string>> strykerInput)
        {
            strykerInput.SuppliedInput = cliInput.Values;
        }

        private IInput GetStrykerInput(CommandOption cliInput) => _cliInputs[cliInput.LongName].Input;

        private void PrepareCliOptions(IStrykerInputs inputs)
        {
            AddCliInput(inputs.ThresholdBreakInput, "break-at", "b", argumentHint: "0-100");
            AddCliInput(inputs.LogToFileInput, "log-to-file", "L", optionType: CommandOptionType.NoValue);
            AddCliInput(inputs.VerbosityInput, "verbosity", "V");
            AddCliInput(inputs.ConcurrencyInput, "concurrency", "c", argumentHint: "number");

            AddCliInput(inputs.SolutionInput, "solution", "s", argumentHint: "file-path", category: InputCategory.Build);
            AddCliInput(inputs.ProjectUnderTestNameInput, "project", "p", argumentHint: "project-name.csproj", category: InputCategory.Build);
            AddCliInput(inputs.MsBuildPathInput, "msbuild-path", null, category: InputCategory.Build);


            AddCliInput(inputs.MutateInput, "mutate", "m", optionType: CommandOptionType.MultipleValue, argumentHint: "glob-pattern", category: InputCategory.Mutation);
            AddCliInput(inputs.MutationLevelInput, "mutation-level", "l", category: InputCategory.Mutation);
            AddCliInput(inputs.SinceInput, "since", "", optionType: CommandOptionType.SingleOrNoValue, argumentHint: "comittish", category: InputCategory.Mutation);
            AddCliInput(inputs.WithBaselineInput, "with-baseline", "", optionType: CommandOptionType.SingleOrNoValue, argumentHint: "comittish", category: InputCategory.Mutation);

            AddCliInput(inputs.ReportersInput, "reporter", "r", optionType: CommandOptionType.MultipleValue, category: InputCategory.Reporting);
            AddCliInput(inputs.ProjectVersionInput, "version", "v", category: InputCategory.Reporting);
            AddCliInput(inputs.DashboardApiKeyInput, "dashboard-api-key", null, category: InputCategory.Reporting);
            AddCliInput(inputs.AzureFileStorageSasInput, "azure-fileshare-sas", null, category: InputCategory.Reporting);

            AddCliInput(inputs.DevModeInput, "dev-mode", null, optionType: CommandOptionType.NoValue, category: InputCategory.Misc);
        }

        private void RegisterCliInput(CommandLineApplication app, CliInput option)
        {
            var argumentHint = option.OptionType switch
            {
                CommandOptionType.NoValue => "",
                CommandOptionType.SingleOrNoValue => $"[:<{option.ArgumentHint}>]",
                _ => $" <{option.ArgumentHint}>"
            };

            var commandOption = new StrykerInputOption($"-{option.ArgumentShortName}|--{option.ArgumentName}{argumentHint}", option.OptionType, option.Category)
            {
                Description = option.Description
            };

            app.Options.Add(commandOption);
        }

        private CliInput AddCliOnlyInput(string argumentName, string argumentShortName, string helpText,
            CommandOptionType optionType = CommandOptionType.SingleValue, InputCategory category = InputCategory.Generic, string argumentHint = null)
        {
            var cliOption = new CliInput
            {
                ArgumentName = argumentName,
                ArgumentShortName = argumentShortName,
                Description = helpText,
                OptionType = optionType,
                ArgumentHint = argumentHint,
                Category = category
            };

            _cliInputs[argumentName] = cliOption;

            return cliOption;
        }

        private void AddCliInput(IInput input, string argumentName, string argumentShortName,
            CommandOptionType optionType = CommandOptionType.SingleValue, InputCategory category = InputCategory.Generic, string argumentHint = null)
        {
            var cliOption = new CliInput
            {
                Input = input,
                ArgumentName = argumentName,
                ArgumentShortName = argumentShortName,
                Description = input.HelpText,
                OptionType = optionType,
                Category = category,
                ArgumentHint = argumentHint
            };

            _cliInputs[argumentName] = cliOption;
        }
    }

    public class StrykerInputOption : CommandOption
    {
        public InputCategory Category { get; private set; }

        public StrykerInputOption(string template, CommandOptionType optionType, InputCategory category) : base(template, optionType) => Category = category;
    }
}
