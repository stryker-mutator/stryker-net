using McMaster.Extensions.CommandLineUtils;
using Stryker.Core.Options;

namespace Stryker.CLI
{
    public class CliInput
    {
        public IInput Input { get; set; }
        public string ArgumentName { get; set; }
        public string ArgumentShortName { get; set; }
        public string ArgumentHint { get; set; }
        public string Description { get; set; }
        public CommandOptionType OptionType { get; set; }
    }
}
