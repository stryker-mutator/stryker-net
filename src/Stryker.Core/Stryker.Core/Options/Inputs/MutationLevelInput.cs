using Stryker.Core.Exceptions;
using Stryker.Core.Mutators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options.Inputs
{
    public class MutationLevelInput : ComplexStrykerInput<string, MutationLevel>
    {
        static MutationLevelInput()
        {
            Description = $"Specifies what mutations will be placed in your project. | { FormatOptions(DefaultInput, ((IEnumerable<MutationLevel>)Enum.GetValues(DefaultValue.GetType())).Select(x => x.ToString())) }";
            DefaultValue = MutationLevel.Standard;
        }

        public override StrykerInput Type => StrykerInput.MutationLevel;

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
