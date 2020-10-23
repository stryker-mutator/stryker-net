using Stryker.Core.Exceptions;
using Stryker.Core.Mutators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options.Options
{
    class ExcludedMutationsOption : BaseStrykerOption<IEnumerable<Mutator>>
    {
        public ExcludedMutationsOption(string[] excludedMutations)
        {
            if (excludedMutations != null && excludedMutations.Any())
            {
                var list = new List<Mutator>();

                // Get all mutatorTypes and their descriptions
                Dictionary<Mutator, string> typeDescriptions = Enum.GetValues(typeof(Mutator))
                    .Cast<Mutator>()
                    .ToDictionary(x => x, x => x.GetDescription());

                foreach (string excludedMutation in excludedMutations)
                {
                    // Find any mutatorType that matches the name passed by the user
                    var mutatorDescriptor = typeDescriptions.FirstOrDefault(
                        x => x.Value.ToString().ToLower().Contains(excludedMutation.ToLower()));
                    if (mutatorDescriptor.Value != null)
                    {
                        list.Add(mutatorDescriptor.Key);
                    }
                    else
                    {
                        throw new StrykerInputException(ErrorMessage,
                            $"Invalid excluded mutation ({excludedMutation}). The excluded mutations options are [{string.Join(", ", typeDescriptions.Select(x => x.Key))}]");
                    }
                }
                Value = list;
            }
        }

        public override StrykerOption Type => StrykerOption.ExcludedMutations;
        public override string HelpText => @"The given mutators will be excluded for this mutation testrun.
    This argument takes a json array as value. Example: ['string', 'logical']";
        public override IEnumerable<Mutator> DefaultValue => null;
    }
}
