using Stryker.Core.Exceptions;
using Stryker.Core.Mutators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options.Inputs
{
    public class ExcludedMutatorsInput : ComplexStrykerInput<IEnumerable<string>, IEnumerable<Mutator>>
    {
        static ExcludedMutatorsInput()
        {
            HelpText = @"The given mutators will be excluded for this mutation testrun.
    This argument takes a json array as value. Example: ['string', 'logical']";
            DefaultValue = new ExcludedMutatorsInput(DefaultInput).Value;
        }

        public override StrykerInput Type => StrykerInput.ExcludedMutators;

        public ExcludedMutatorsInput(IEnumerable<string> mutatorsToExclude)
        {
            if (mutatorsToExclude is { } && mutatorsToExclude.Any())
            {
                var excludedMutators = new List<Mutator>();

                // Get all mutatorTypes and their descriptions
                Dictionary<Mutator, string> typeDescriptions = Enum.GetValues(typeof(Mutator))
                    .Cast<Mutator>()
                    .ToDictionary(x => x, x => x.GetDescription());

                foreach (string mutatorToExclude in mutatorsToExclude)
                {
                    // Find any mutatorType that matches the name passed by the user
                    var mutatorDescriptor = typeDescriptions.FirstOrDefault(
                        x => x.Value.ToString().ToLower().Contains(mutatorToExclude.ToLower()));
                    if (mutatorDescriptor.Value is { })
                    {
                        excludedMutators.Add(mutatorDescriptor.Key);
                    }
                    else
                    {
                        throw new StrykerInputException($"Invalid excluded mutator ({mutatorToExclude}).");
                    }
                }

                Value = excludedMutators;
            }
        }
    }
}
