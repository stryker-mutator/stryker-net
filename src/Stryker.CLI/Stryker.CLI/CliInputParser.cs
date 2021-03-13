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

    public static class CliInputParser
    {
        private static readonly IDictionary<string, CliInput> _cliInputs = new Dictionary<string, CliInput>();
        private static StrykerInputs _strykerInputs;
        private static readonly CliInput _configFileInput;
        private static readonly CliInput _generateConfigFileInput;

        static CliInputParser()
        {
            _configFileInput = AddCliOnlyInput("config-file", "f", "Choose the file containing your stryker configuration relative to current working directory. | default: stryker-config.json", argumentHint: "file-path");
            _generateConfigFileInput = AddCliOnlyInput("init", "i", "Generate a stryker config file with selected plus default options where no option is selected.", optionType: CommandOptionType.SingleOrNoValue, argumentHint: "file-path");
        }

        public static StrykerInputs RegisterStrykerInputs(CommandLineApplication app, ILogger logger)
        {
            _strykerInputs = new StrykerInputs(null, logger)
            {
                AdditionalTimeoutMsInput = new AdditionalTimeoutMsInput(),
                AzureFileStorageSasInput = new AzureFileStorageSasInput(),
                AzureFileStorageUrlInput = new AzureFileStorageUrlInput(),
                BaselineProviderInput = new BaselineProviderInput(),
                BasePathInput = new BasePathInput(),
                ConcurrencyInput = new ConcurrencyInput(),
                DashboardApiKeyInput = new DashboardApiKeyInput(),
                DashboardUrlInput = new DashboardUrlInput(),
                DevModeInput = new DevModeInput(),
                DiffIgnoreFilePatternsInput = new DiffIgnoreFilePatternsInput(),
                DisableAbortTestOnFailInput = new DisableAbortTestOnFailInput(),
                DisableSimultaneousTestingInput = new DisableSimultaneousTestingInput(),
                ExcludedMutatorsInput = new ExcludedMutatorsInput(),
                FallbackVersionInput = new FallbackVersionInput(),
                IgnoredMethodsInput = new IgnoredMethodsInput(),
                LanguageVersionInput = new LanguageVersionInput(),
                VerbosityInput = new VerbosityInput(),
                LogToFileInput = new LogToFileInput(),
                ModuleNameInput = new ModuleNameInput(),
                MutateInput = new MutateInput(),
                MutationLevelInput = new MutationLevelInput(),
                OptimizationModeInput = new OptimizationModeInput(),
                OutputPathInput = new OutputPathInput(),
                ProjectNameInput = new ProjectNameInput(),
                ProjectUnderTestNameInput = new ProjectUnderTestNameInput(),
                ProjectVersionInput = new ProjectVersionInput(),
                ReportersInput = new ReportersInput(),
                SinceInput = new SinceInput(),
                SinceBranchInput = new SinceBranchInput(),
                SinceCommitInput = new SinceCommitInput(),
                SolutionPathInput = new SolutionPathInput(),
                TestProjectsInput = new TestProjectsInput(),
                ThresholdBreakInput = new ThresholdBreakInput(),
                ThresholdHighInput = new ThresholdHighInput(),
                ThresholdLowInput = new ThresholdLowInput(),
                WithBaselineInput = new WithBaselineInput()
            };

            PrepareCliOptions();

            foreach (var (_, value) in _cliInputs)
            {
                RegisterCliInput(app, value);
            }

            return _strykerInputs;
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
                var strykerInput = GetStrykerInput(cliInput);

                // the switch expression must return a value, as a workaround, return a bool and discard
                _ = cliInput.OptionType switch
                {
                    CommandOptionType.NoValue => ParseNoValue((IInputDefinition<bool>)strykerInput),
                    CommandOptionType.MultipleValue => ParseMultiValue(cliInput, (IInputDefinition<IEnumerable<string>>)strykerInput),
                    CommandOptionType.SingleOrNoValue => ParseSingleOrNoValue(strykerInput, cliInput, strykerInputs),
                    _ => true
                };

                _ = strykerInput switch
                {
                    IInputDefinition inputDefinition when inputDefinition.GetType().BaseType.GetGenericArguments()[0] == typeof(string) => ParseSingleStringValue(cliInput, (IInputDefinition<string>)inputDefinition),
                    IInputDefinition inputDefinition when inputDefinition.GetType().BaseType.GetGenericArguments()[0] == typeof(int) => ParseSingleIntValue(cliInput, (IInputDefinition<int>)inputDefinition),
                    _ => true
                };
            }
        }

        private static bool ParseNoValue(IInputDefinition<bool> strykerInput)
        {
            strykerInput.SuppliedInput = true;
            return true;
        }

        private static bool ParseSingleStringValue(CommandOption cliInput, IInputDefinition<string> strykerInput)
        {
            strykerInput.SuppliedInput = cliInput.Value();
            return true;
        }

        private static bool ParseSingleIntValue(CommandOption cliInput, IInputDefinition<int> strykerInput)
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

        private static bool ParseSingleOrNoValue(IInputDefinition strykerInput, CommandOption cliInput, StrykerInputs strykerInputs)
        {
            //switch (strykerInput)
            //{
            //    // handle single or no value inputs
            //}
            return true;
        }

        private static bool ParseMultiValue(CommandOption cliInput, IInputDefinition<IEnumerable<string>> strykerInput)
        {
            strykerInput.SuppliedInput = cliInput.Values;

            return true;
        }

        private static IInputDefinition GetStrykerInput(CommandOption cliInput) => _cliInputs[cliInput.LongName].Input;

        private static void PrepareCliOptions()
        {
            AddCliInput(_strykerInputs.DevModeInput, "dev-mode", null, optionType: CommandOptionType.NoValue);
            AddCliInput(_strykerInputs.ConcurrencyInput, "concurrency", "c", argumentHint: "number");
            AddCliInput(_strykerInputs.SolutionPathInput, "solution", "s", argumentHint: "file-path");

            AddCliInput(_strykerInputs.ReportersInput, "reporter", "r", optionType: CommandOptionType.MultipleValue);
            AddCliInput(_strykerInputs.MutateInput, "mutate", "m", optionType: CommandOptionType.MultipleValue, argumentHint: "glob-pattern");
            AddCliInput(_strykerInputs.ThresholdBreakInput, "break", "b", argumentHint: "0-100");

            AddCliInput(_strykerInputs.ProjectUnderTestNameInput, "project", "p", argumentHint: "project-name.csproj");
            AddCliInput(_strykerInputs.MutationLevelInput, "mutation-level", "l");

            AddCliInput(_strykerInputs.LogToFileInput, "log-to-file", "L", optionType: CommandOptionType.NoValue);
            AddCliInput(_strykerInputs.VerbosityInput, "verbosity", "V");

            AddCliInput(_strykerInputs.SinceInput, "since", "since", optionType: CommandOptionType.NoValue, argumentHint: "comittish");
            AddCliInput(_strykerInputs.WithBaselineInput, "with-baseline", "baseline", optionType: CommandOptionType.SingleOrNoValue, argumentHint: "comittish");

            AddCliInput(_strykerInputs.DashboardApiKeyInput, "dashboard-api-key", null);
            AddCliInput(_strykerInputs.AzureFileStorageSasInput, "azure-fileshare-sas", null);
            AddCliInput(_strykerInputs.ProjectVersionInput, "version", "v");
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

            _cliInputs[argumentName] = cliOption;

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

            _cliInputs[argumentName] = cliOption;

            return cliOption;
        }
    }
}
