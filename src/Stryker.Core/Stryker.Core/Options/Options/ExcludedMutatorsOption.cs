using Stryker.Core.Exceptions;
using Stryker.Core.Mutators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options.Options
{
    public class ExcludedMutatorsOption : BaseStrykerOption<IEnumerable<Mutator>>
    {
        public ExcludedMutatorsOption(IEnumerable<string> mutatorsToExclude)
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

        public override StrykerOption Type => StrykerOption.ExcludedMutations;
        public override string HelpText => @"The given mutators will be excluded for this mutation testrun.";
        public override IEnumerable<Mutator> DefaultValue => Enumerable.Empty<Mutator>();
    }
}
