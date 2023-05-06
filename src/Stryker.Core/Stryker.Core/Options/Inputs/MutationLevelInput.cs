using System;
using System.Collections.Generic;
using Stryker.Core.Exceptions;
using Stryker.Core.Mutators;

namespace Stryker.Core.Options.Inputs;

public class MutationLevelInput : Input<string>
{
    public override string Default => MutationLevel.Standard.ToString();

    protected override string Description => "Specify which mutation levels to place. Every higher level includes the mutations from the lower levels.";
    protected override IEnumerable<string> AllowedOptions => EnumToStrings(typeof(MutationLevel));

    public MutationLevel Validate()
    {
        if (SuppliedInput is null)
        {
            return MutationLevel.Standard;
        }
        else if (Enum.TryParse(SuppliedInput, true, out MutationLevel level))
        {
            return level;
        }
        else
        {
            throw new InputException($"The given mutation level ({SuppliedInput}) is invalid. Valid options are: [{string.Join(", ", ((IEnumerable<MutationLevel>)Enum.GetValues(typeof(MutationLevel))))}]");
        }
    }
}
