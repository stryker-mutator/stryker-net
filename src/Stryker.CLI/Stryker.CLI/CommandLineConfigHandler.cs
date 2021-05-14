using System;
using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
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

                // the switch expression must return a value, as a workaround, return a bool and discard
                _ = cliInput.OptionType switch
                {
                    CommandOptionType.NoValue => HandleNoValue((IInputDefinition<bool?>)strykerInput),
                    CommandOptionType.MultipleValue => HandleMultiValue(cliInput, (IInputDefinition<IEnumerable<string>>)strykerInput),
                    CommandOptionType.SingleOrNoValue => HandleSingleOrNoValue(strykerInput, cliInput, inputs),
                    _ => true
                };

                _ = strykerInput switch
                {
                    IInputDefinition inputDefinition when inputDefinition.GetType().BaseType.GetGenericArguments()[0] == typeof(string) => HandleSingleStringValue(cliInput, (IInputDefinition<string>)inputDefinition),
                    IInputDefinition inputDefinition when inputDefinition.GetType().BaseType.GetGenericArguments()[0] == typeof(int?) => HandleSingleIntValue(cliInput, (IInputDefinition<int?>)inputDefinition),
                    IInputDefinition inputDefinition when inputDefinition.GetType().BaseType.GetGenericArguments()[0] == typeof(int) => HandleSingleIntValue(cliInput, (IInputDefinition<int?>)inputDefinition),
                    _ => true
                };
            }
        }

        private bool HandleNoValue(IInputDefinition<bool?> strykerInput)
        {
            strykerInput.SuppliedInput = true;
            return true;
        }

        private bool HandleSingleStringValue(CommandOption cliInput, IInputDefinition<string> strykerInput)
        {
            strykerInput.SuppliedInput = cliInput.Value();
            return true;
        }

        private bool HandleSingleIntValue(CommandOption cliInput, IInputDefinition<int?> strykerInput)
        {
            if (int.TryParse(cliInput.Value(), out int value))
            {
                strykerInput.SuppliedInput = value;
            } else
            {
                throw new StrykerInputException($"Unexpected value for argument {cliInput.LongName}:{cliInput.Value()}. Expected type to be integer");
            }
            return true;
        }

        private bool HandleSingleOrNoValue(IInputDefinition strykerInput, CommandOption cliInput, IStrykerInputs inputs)
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
            return true;
        }

        private bool HandleMultiValue(CommandOption cliInput, IInputDefinition<IEnumerable<string>> strykerInput)
        {
            strykerInput.SuppliedInput = cliInput.Values;

            return true;
        }

        private IInputDefinition GetStrykerInput(CommandOption cliInput) => _cliInputs[cliInput.LongName].Input;

        private void PrepareCliOptions(IStrykerInputs inputs)
        {
            AddCliInput(inputs.DevModeInput, "dev-mode", null, optionType: CommandOptionType.NoValue);
            AddCliInput(inputs.ConcurrencyInput, "concurrency", "c", argumentHint: "number");
            AddCliInput(inputs.SolutionPathInput, "solution", "s", argumentHint: "file-path");

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
