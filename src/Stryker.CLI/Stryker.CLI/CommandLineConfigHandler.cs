using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Stryker.Core.Exceptions;
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

    public class CommandLineConfigHandler
    {
        private readonly IDictionary<string, CliInput> _cliInputs = new Dictionary<string, CliInput>();
        private readonly CliInput _configFileInput;

        public CommandLineConfigHandler()
        {
            _configFileInput = AddCliOnlyInput("config-file", "f", "Choose the file containing your stryker configuration relative to current working directory. | default: stryker-config.json", argumentHint: "file-path");
        }

        public void RegisterCommandlineOptions(CommandLineApplication app, IStrykerInputs inputs)
        {
            PrepareCliOptions(inputs);

            foreach (var (_, value) in _cliInputs)
            {
                RegisterCliInput(app, value);
            }
        }

        public string ConfigFilePath(string[] args, CommandLineApplication app)
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

                switch(cliInput.OptionType)
                {
                    case CommandOptionType.NoValue:
                        HandleNoValue((IInputDefinition<bool?>)strykerInput);
                        break;
                    case CommandOptionType.MultipleValue:
                        HandleMultiValue(cliInput, (IInputDefinition<IEnumerable<string>>)strykerInput);
                        break;
                    case CommandOptionType.SingleOrNoValue:
                        HandleSingleOrNoValue(strykerInput, cliInput, inputs);
                        break;
                };

                switch(strykerInput)
                {
                    case IInputDefinition<string> stringInput:
                        HandleSingleStringValue(cliInput, stringInput);
                        break;
                    case IInputDefinition<int?> nullableIntInput:
                        HandleSingleIntValue(cliInput, nullableIntInput);
                        break;
                    case IInputDefinition<int> intInput:
                        HandleSingleIntValue(cliInput, (IInputDefinition<int?>)intInput);
                        break;
                };
            }
        }

        private static void HandleNoValue(IInputDefinition<bool?> strykerInput)
        {
            strykerInput.SuppliedInput = true;
        }

        private static void HandleSingleStringValue(CommandOption cliInput, IInputDefinition<string> strykerInput)
        {
            strykerInput.SuppliedInput = cliInput.Value();
        }

        private static void HandleSingleIntValue(CommandOption cliInput, IInputDefinition<int?> strykerInput)
        {
            if (int.TryParse(cliInput.Value(), out var value))
            {
                strykerInput.SuppliedInput = value;
            } else
            {
                throw new InputException($"Unexpected value for argument {cliInput.LongName}:{cliInput.Value()}. Expected type to be integer");
            }
        }

        private static void HandleSingleOrNoValue(IInputDefinition strykerInput, CommandOption cliInput, IStrykerInputs inputs)
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

        private static void HandleMultiValue(CommandOption cliInput, IInputDefinition<IEnumerable<string>> strykerInput)
        {
            strykerInput.SuppliedInput = cliInput.Values;
        }

        private IInputDefinition GetStrykerInput(CommandOption cliInput) => _cliInputs[cliInput.LongName].Input;

        private void PrepareCliOptions(IStrykerInputs inputs)
        {
            AddCliInput(inputs.DevModeInput, "dev-mode", null, optionType: CommandOptionType.NoValue);
            AddCliInput(inputs.ConcurrencyInput, "concurrency", "c", argumentHint: "number");
            AddCliInput(inputs.SolutionInput, "solution", "s", argumentHint: "file-path");

            AddCliInput(inputs.ReportersInput, "reporter", "r", optionType: CommandOptionType.MultipleValue);
            AddCliInput(inputs.MutateInput, "mutate", "m", optionType: CommandOptionType.MultipleValue, argumentHint: "glob-pattern");
            AddCliInput(inputs.ThresholdBreakInput, "break-at", "b", argumentHint: "0-100");

            AddCliInput(inputs.ProjectUnderTestNameInput, "project", "p", argumentHint: "project-name.csproj");
            AddCliInput(inputs.MutationLevelInput, "mutation-level", "l");

            AddCliInput(inputs.LogToFileInput, "log-to-file", "L", optionType: CommandOptionType.NoValue);
            AddCliInput(inputs.VerbosityInput, "verbosity", "V");

            AddCliInput(inputs.SinceInput, "since", "since", optionType: CommandOptionType.SingleOrNoValue, argumentHint: "comittish");
            AddCliInput(inputs.WithBaselineInput, "with-baseline", "baseline", optionType: CommandOptionType.SingleOrNoValue, argumentHint: "comittish");

            AddCliInput(inputs.DashboardApiKeyInput, "dashboard-api-key", null);
            AddCliInput(inputs.AzureFileStorageSasInput, "azure-fileshare-sas", null);
            AddCliInput(inputs.ProjectVersionInput, "version", "v");
        }

        private void RegisterCliInput(CommandLineApplication app, CliInput option)
        {
            var argumentHint = option.OptionType switch
            {
                CommandOptionType.NoValue => "",
                CommandOptionType.SingleOrNoValue => $"[:<{option.ArgumentHint}>]",
                _ => $" <{option.ArgumentHint}>"
            };

            app.Option($"-{option.ArgumentShortName}|--{option.ArgumentName}{argumentHint}", option.Description, option.OptionType);
        }

        private CliInput AddCliOnlyInput(string argumentName, string argumentShortName, string helpText,
            CommandOptionType optionType = CommandOptionType.SingleValue, string argumentHint = null)
        {
            var cliOption = new CliInput
            {
                ArgumentName = argumentName,
                ArgumentShortName = argumentShortName,
                Description = helpText,
                OptionType = optionType,
                ArgumentHint = argumentHint
            };

            _cliInputs[argumentName] = cliOption;

            return cliOption;
        }

        private CliInput AddCliInput(IInputDefinition input, string argumentName, string argumentShortName,
            CommandOptionType optionType = CommandOptionType.SingleValue, string argumentHint = null)
        {
            var cliOption = new CliInput
            {
                Input = input,
                ArgumentName = argumentName,
                ArgumentShortName = argumentShortName,
                Description = input.HelpText,
                OptionType = optionType,
                ArgumentHint = argumentHint
            };

            _cliInputs[argumentName] = cliOption;

            return cliOption;
        }
    }
}
