using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Exceptions;
using System;

namespace Stryker.Core.Options.Options
{
    public class LanguageVersionInput : ComplexStrykerInput<LanguageVersion, string>
    {
        static LanguageVersionInput()
        {
            HelpText = "Set the c# version used to compile.";
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
