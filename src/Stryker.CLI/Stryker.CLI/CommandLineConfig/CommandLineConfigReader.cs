using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Spectre.Console;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;

namespace Stryker.CLI.CommandLineConfig
{
    public class CommandLineConfigReader
    {
        private readonly IDictionary<string, CliInput> _cliInputs = new Dictionary<string, CliInput>();
        private readonly CliInput _configFileInput;
        private readonly IAnsiConsole _console;

        public CommandLineConfigReader(IAnsiConsole console = null) {
            _configFileInput = AddCliOnlyInput("config-file", "f", "Choose the file containing your stryker configuration relative to current working directory. Supports json and yaml formats. | default: stryker-config.json", argumentHint: "relative-path");
            _console = console ?? AnsiConsole.Console;
        }

        public void RegisterCommandLineOptions(CommandLineApplication app, IStrykerInputs inputs)
        {
            PrepareCliOptions(inputs);

            RegisterCliInputs(app);
        }

        public void RegisterInitCommand(CommandLineApplication app, IFileSystem fileSystem, IStrykerInputs inputs, string[] args)
        {
            app.Command("init", initCommandApp =>
            {
                RegisterCliInputs(initCommandApp);

                initCommandApp.OnExecute(() =>
                {
                    _console.WriteLine($"Initializing new config file.");
                    _console.WriteLine();

                    ReadCommandLineConfig(args[1..], initCommandApp, inputs);
                    var configOption = initCommandApp.Options.SingleOrDefault(o => o.LongName == _configFileInput.ArgumentName);
                    var basePath = fileSystem.Directory.GetCurrentDirectory();
                    var configFilePath = Path.Combine(basePath, configOption?.Value() ?? "stryker-config.json");

                    if (fileSystem.File.Exists(configFilePath))
                    {
                        _console.Write("Config file already exists at ");
                        _console.WriteLine(configFilePath, new Style(Color.Cyan1));
                        var overwrite = _console.Confirm($"Do you want to overwrite it?", false);
                        if (!overwrite)
                        {
                            return;
                        }
                        _console.WriteLine();
                    }

                    var config = FileConfigGenerator.GenerateConfigAsync(inputs);
                    fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(configFilePath));
                    fileSystem.File.WriteAllText(configFilePath, config);

                    _console.Write("Config file written to ");
                    _console.WriteLine(configFilePath, new Style(Color.Cyan1));
                    _console.Write("The file is populated with default values, remove the options you don't need and edit the options you want to use. For more information on configuring stryker see: ");
                    _console.WriteLine("https://stryker-mutator.io/docs/stryker-net/configuration", new Style(Color.Cyan1));
                });
            });
        }

        public CommandOption GetConfigFileOption(string[] args, CommandLineApplication app)
        {
            var commands = app.Parse(args);
            return commands.SelectedCommand.Options.SingleOrDefault(o => o.LongName == _configFileInput.ArgumentName);
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

        private void RegisterCliInputs(CommandLineApplication app)
        {
            foreach (var (_, value) in _cliInputs)
            {
                RegisterCliInput(app, value);
            }
        }

        private static void HandleNoValue(IInput<bool?> strykerInput) => strykerInput.SuppliedInput = true;

        private static void HandleSingleStringValue(CommandOption cliInput, IInput<string> strykerInput) => strykerInput.SuppliedInput = cliInput.Value();

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
                    inputs.SinceTargetInput.SuppliedInput = cliInput.Value();
                    break;

                case OpenReportInput openReportInput:
                    openReportInput.SuppliedInput = cliInput.Value();
                    inputs.OpenReportEnabledInput.SuppliedInput = true;
                    break;
            }
        }

        private static void HandleMultiValue(CommandOption cliInput, IInput<IEnumerable<string>> strykerInput) => strykerInput.SuppliedInput = cliInput.Values;

        private IInput GetStrykerInput(CommandOption cliInput) => _cliInputs[cliInput.LongName].Input;

        private void PrepareCliOptions(IStrykerInputs inputs)
        {
            // Category: Generic
            AddCliInput(inputs.ThresholdBreakInput, "break-at", "b", argumentHint: "0-100");
            AddCliInput(inputs.ThresholdHighInput, "threshold-high", "", argumentHint: "0-100");
            AddCliInput(inputs.ThresholdLowInput, "threshold-low", "", argumentHint: "0-100");
            AddCliInput(inputs.LogToFileInput, "log-to-file", "L", optionType: CommandOptionType.NoValue);
            AddCliInput(inputs.VerbosityInput, "verbosity", "V");
            AddCliInput(inputs.ConcurrencyInput, "concurrency", "c", argumentHint: "number");
            AddCliInput(inputs.DisableBailInput, "disable-bail", null, optionType: CommandOptionType.NoValue);
            // Category: Build
            AddCliInput(inputs.SolutionInput, "solution", "s", argumentHint: "file-path", category: InputCategory.Build);
            AddCliInput(inputs.SourceProjectNameInput, "project", "p", argumentHint: "project-name.csproj", category: InputCategory.Build);
            AddCliInput(inputs.TestProjectsInput, "test-project", "tp", CommandOptionType.MultipleValue, InputCategory.Build);
            AddCliInput(inputs.MsBuildPathInput, "msbuild-path", null, category: InputCategory.Build);
            AddCliInput(inputs.TargetFrameworkInput, "target-framework", null, optionType: CommandOptionType.SingleValue, category: InputCategory.Build);
            // Category: Mutation
            AddCliInput(inputs.MutateInput, "mutate", "m", optionType: CommandOptionType.MultipleValue, argumentHint: "glob-pattern", category: InputCategory.Mutation);
            AddCliInput(inputs.MutationLevelInput, "mutation-level", "l", category: InputCategory.Mutation);
            AddCliInput(inputs.SinceInput, "since", "", optionType: CommandOptionType.SingleOrNoValue, argumentHint: "committish", category: InputCategory.Mutation);
            AddCliInput(inputs.WithBaselineInput, "with-baseline", "", optionType: CommandOptionType.SingleOrNoValue, argumentHint: "committish", category: InputCategory.Mutation);
            // Category: Reporting
            AddCliInput(inputs.OpenReportInput, "open-report", "o", CommandOptionType.SingleOrNoValue, argumentHint: "report-type", category: InputCategory.Reporting);
            AddCliInput(inputs.ReportersInput, "reporter", "r", optionType: CommandOptionType.MultipleValue, category: InputCategory.Reporting);
            AddCliInput(inputs.ProjectVersionInput, "version", "v", category: InputCategory.Reporting);
            AddCliInput(inputs.DashboardApiKeyInput, "dashboard-api-key", null, category: InputCategory.Reporting);
            AddCliInput(inputs.AzureFileStorageSasInput, "azure-fileshare-sas", null, category: InputCategory.Reporting);
            AddCliInput(inputs.OutputPathInput, "output", "O", optionType: CommandOptionType.SingleValue, category: InputCategory.Reporting);
            // Category: Misc
            AddCliInput(inputs.BreakOnInitialTestFailureInput, "break-on-initial-test-failure", null, optionType: CommandOptionType.NoValue, category: InputCategory.Misc);
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

            app.AddOption(commandOption);
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
