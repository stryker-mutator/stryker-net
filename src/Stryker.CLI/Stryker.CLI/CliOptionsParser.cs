using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;

namespace Stryker.CLI
{
    public class CliOption
    {
        public StrykerInput InputType { get; set; }
        public string ArgumentName { get; set; }
        public string ArgumentShortName { get; set; }
        public string ArgumentHint { get; set; }
        public string Description { get; set; }
        public CommandOptionType OptionType { get; set; }
    }

    public static class CliOptionsParser
    {
        private static readonly IDictionary<string, CliOption> CliOptions = new Dictionary<string, CliOption>();
        private static readonly CliOption ConfigOption;

        static CliOptionsParser()
        {
            ConfigOption = AddCliOption(StrykerInput.None, "--config-file-path", "-cp",
                "Choose the file containing your stryker configuration relative to current working directory. | default: stryker-config.json", argumentHint: "file-path");
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
            return app.Parse(args).SelectedCommand.Options.SingleOrDefault(o => o.LongName == ConfigOption.ArgumentName)?.Value();
        }

        public static StrykerOptions EnrichWithCommandLineArguments(this StrykerOptions options, string[] args, CommandLineApplication app)
        {
            StrykerOptions enrichedOptions = options;
            foreach (var option in app.Parse(args).SelectedCommand.Options.Where(option => option.HasValue()))
            {
                var inputType = CliOptions[option.LongName].InputType;

                enrichedOptions = option.OptionType switch
                {
                    CommandOptionType.NoValue => enrichedOptions.With(inputType, option.HasValue()),
                    CommandOptionType.SingleOrNoValue => enrichedOptions.With(inputType, option.HasValue(), option.Value()),
                    CommandOptionType.SingleValue => enrichedOptions.With(inputType, option.Value()),
                    CommandOptionType.MultipleValue => enrichedOptions.With(inputType, option.Values),
                    _ => enrichedOptions
                };
            }

            return enrichedOptions;
        }

        private static void PrepareCliOptions()
        {
            AddCliOption(StrykerInput.ThresholdBreak, "break", "b", new ThresholdBreakInput().HelpText, argumentHint: "number");
            AddCliOption(StrykerInput.DevMode, "dev-mode", "dev", new DevModeInput().HelpText, optionType: CommandOptionType.NoValue);

            AddCliOption(StrykerInput.Mutate, "mutate", "m", new MutateInput().HelpText, optionType: CommandOptionType.MultipleValue, argumentHint: "glob-pattern");

            AddCliOption(StrykerInput.SolutionPath, "solution-path", "s", new SolutionPathInput().HelpText, argumentHint: "file-path");
            AddCliOption(StrykerInput.ProjectUnderTestName, "project-file", "p", new ProjectUnderTestNameInput().HelpText, argumentHint: "project-name");
            AddCliOption(StrykerInput.MutationLevel, "mutation-level", "level", new MutationLevelInput().HelpText);

            AddCliOption(StrykerInput.LogToFile, "log-file", "f", new LogToFileInput().HelpText, optionType: CommandOptionType.NoValue);
            AddCliOption(StrykerInput.LogLevel, "log-level", "l", new LogLevelInput().HelpText);
            AddCliOption(StrykerInput.Reporters, "reporter", "r", new ReportersInput().HelpText, optionType: CommandOptionType.MultipleValue);

            AddCliOption(StrykerInput.DiffCompare, "diff", "diff", new DiffCompareInput().HelpText, optionType: CommandOptionType.SingleOrNoValue, argumentHint: "comittish");
            AddCliOption(StrykerInput.DashboardCompare, "dashboard-compare", "compare", new DashboardCompareInput().HelpText, optionType: CommandOptionType.SingleOrNoValue, argumentHint: "comittish");

            AddCliOption(StrykerInput.DashboardApiKey, "dashboard-api-key", "dk", new DashboardApiKeyInput().HelpText);
            AddCliOption(StrykerInput.AzureFileStorageSas, "azure-storage-sas", "sas", new AzureFileStorageSasInput().HelpText);

            AddCliOption(StrykerInput.ProjectVersion, "dashboard-version", "dv", new ProjectVersionInput().HelpText);
            AddCliOption(StrykerInput.FallbackVersion, "fallback-version", "fv", new FallbackVersionInput().HelpText, argumentHint: "comittish");

            AddCliOption(StrykerInput.Concurrency, "concurrency", "c", new ConcurrencyInput().HelpText, argumentHint: "number");
        }

        private static void RegisterCliOption(CommandLineApplication app, CliOption option)
        {
            string argumentHint = option.OptionType switch
            {
                CommandOptionType.NoValue => "",
                CommandOptionType.SingleOrNoValue => $"[:<{option.ArgumentHint}>]",
                _ => $" <{option.ArgumentHint}>"
            };

            app.Option($"{option.ArgumentShortName}|{option.ArgumentName}{argumentHint}", option.Description, option.OptionType);
        }

        private static CliOption AddCliOption(StrykerInput inputType, string argumentName, string argumentShortName,
            string description, CommandOptionType optionType = CommandOptionType.SingleValue, string argumentHint = null)
        {
            var cliOption = new CliOption
            {
                InputType = inputType,
                ArgumentName = $"--{argumentName}",
                ArgumentShortName = $"-{argumentShortName}",
                Description = description,
                OptionType = optionType,
                ArgumentHint = argumentHint
            };

            CliOptions[argumentName] = cliOption;

            return cliOption;
        }
    }
}
