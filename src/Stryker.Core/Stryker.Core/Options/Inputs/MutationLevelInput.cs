using System;
using Stryker.Core.Exceptions;
using Stryker.Core.Mutators;

namespace Stryker.Core.Options.Inputs
{
    public class MutationLevelInput : InputDefinition<string, MutationLevel>
    {
        public override string Default => MutationLevel.Standard.ToString();

        protected override string Description => "Specify which mutation levels to place. Every higher level includes the mutations from the lower levels.";
        protected override string HelpOptions => FormatEnumHelpOptions();

        public MutationLevel Validate()
        {
            if (Enum.TryParse(SuppliedInput, true, out MutationLevel level))
            {
                return level;
            }
            else
            {
                throw new StrykerInputException($"The given mutation level ({SuppliedInput}) is invalid.");
            }
        }
    }
}
