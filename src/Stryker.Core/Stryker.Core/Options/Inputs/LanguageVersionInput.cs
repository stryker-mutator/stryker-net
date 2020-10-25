using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options.Inputs
{
    public class LanguageVersionInput : ComplexStrykerInput<string, LanguageVersion>
    {
        static LanguageVersionInput()
        {
            HelpText = $"Set the c# version used to compile. | { FormatOptions(DefaultInput, ((IEnumerable<LanguageVersion>)Enum.GetValues(DefaultValue.GetType())).Where(l => l != LanguageVersion.CSharp1)) }";
            DefaultInput = "latest";
            DefaultValue = new LanguageVersionInput(DefaultInput).Value;
        }

        public override StrykerInput Type => StrykerInput.LanguageVersion;

        public LanguageVersionInput(string languageVersion)
        {
            if (languageVersion is { })
            {
                if (Enum.TryParse(languageVersion, true, out LanguageVersion result) && result != LanguageVersion.CSharp1)
                {
                    Value = result;
                }
                else
                {
                    throw new StrykerInputException($"The given c# language version ({languageVersion}) is invalid.");
                }
            }
        }
    }
}
