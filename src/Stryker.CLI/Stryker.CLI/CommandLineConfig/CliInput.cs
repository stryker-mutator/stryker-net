using McMaster.Extensions.CommandLineUtils;
using Stryker.Configuration.Options;

namespace Stryker.CLI.CommandLineConfig;

public record CliInput
{
    public IInput Input { get; init; }
    public string ArgumentName { get; init; }
    public string ArgumentShortName { get; init; }
    public string ArgumentHint { get; init; }
    public string Description { get; init; }
    public CommandOptionType OptionType { get; init; }
    public InputCategory Category { get; init; }
};
