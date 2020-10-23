using Stryker.Core.Exceptions;
using Stryker.Core.Mutators;
using System;

namespace Stryker.Core.Options.Options
{
    public class MutationLevelOption : BaseStrykerOption<MutationLevel>
    {
        public MutationLevelOption(string mutationLevel)
        {
            if (mutationLevel is { })
            {
                if (Enum.TryParse(mutationLevel, true, out MutationLevel level))
                {
                    Value = level;
                }
                else
                {
                    throw new StrykerInputException($"The given mutation level ({mutationLevel}) is invalid.");
                }
            }
        }

        public override StrykerOption Type => StrykerOption.MutationLevel;

        public override string HelpText => "Specifies which mutations will be placed in your project";

        public override MutationLevel DefaultValue => MutationLevel.Standard;
    }
}
