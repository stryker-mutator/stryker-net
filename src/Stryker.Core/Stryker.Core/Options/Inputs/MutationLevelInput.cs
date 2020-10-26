using Stryker.Core.Exceptions;
using Stryker.Core.Mutators;
using System;

namespace Stryker.Core.Options.Inputs
{
    public class MutationLevelInput : ComplexStrykerInput<string, MutationLevel>
    {
        public override StrykerInput Type => StrykerInput.MutationLevel;
        public override string DefaultInput => DefaultValue.ToString();
        public override MutationLevel DefaultValue => MutationLevel.Standard;

        protected override string Description => "Specify which mutation levels to place. Every higher level includes the mutations from the lower levels.";
        protected override string HelpOptions => FormatEnumHelpOptions();

        public MutationLevelInput(string mutationLevel)
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
    }
}
