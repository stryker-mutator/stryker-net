using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options.Inputs;

public class LanguageVersionInput : Input<string>
{
    public override string Default => "latest";

    protected override string Description => $"The c# version used in compilation.";
    protected override IEnumerable<string> AllowedOptions => Enum.GetNames(typeof(LanguageVersion)).Where(l => LanguageVersion.CSharp1.ToString() != l);

    public LanguageVersion Validate()
    {
        if (SuppliedInput is { })
        {
            if (Enum.TryParse(SuppliedInput, true, out LanguageVersion result) && result != LanguageVersion.CSharp1)
            {
                return result;
            }
            else
            {
                throw new InputException($"The given c# language version ({SuppliedInput}) is invalid. Valid options are: [{string.Join(", ", ((IEnumerable<LanguageVersion>)Enum.GetValues(typeof(LanguageVersion))).Where(l => l != LanguageVersion.CSharp1))}]");
            }
        }
        return LanguageVersion.Default;
    }
}
