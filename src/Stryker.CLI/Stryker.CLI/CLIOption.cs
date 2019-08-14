using McMaster.Extensions.CommandLineUtils;

namespace Stryker.CLI
{
    public class CLIOption<T>
    {
        public string ArgumentName { get; set; }
        public string ArgumentShortName { get; set; }
        public string ArgumentDescription { get; set; }
        public T DefaultValue { get; set; }
        public string JsonKey { get; set; }
        public bool IsDeprecated { get; set; }
        public string DeprecatedMessage { get; set; }
        public CommandOptionType ValueType { get; set; } = CommandOptionType.SingleValue;
    }
}
