using Stryker.Core.Exceptions;
using Stryker.Core.Mutators;
using System;
using System.Collections.Generic;

namespace Stryker.Core.Options.Options
{
    public class MutationLevelOption : BaseStrykerOption<MutationLevel>
    {
        public MutationLevelOption(string mutationLevel)
        {
            if (Enum.TryParse(mutationLevel, true, out MutationLevel level))
            {
                Value = level;
            }
            else
            {
                throw new StrykerInputException(ErrorMessage,
                    $"The given mutation level ({mutationLevel}) is invalid. Valid options are: [{ string.Join(", ", (IEnumerable<MutationLevel>)Enum.GetValues(typeof(MutationLevel))) }]");
            }
        }

        public override StrykerOption Type => StrykerOption.MutationLevel;

        public override string HelpText => "Specifies which mutations will be placed in your project";

        public override MutationLevel DefaultValue => MutationLevel.Standard;
    }
}
