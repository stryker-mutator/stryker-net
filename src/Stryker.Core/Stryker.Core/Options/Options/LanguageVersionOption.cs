using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Exceptions;
using System;

namespace Stryker.Core.Options.Options
{
    public class LanguageVersionOption : BaseStrykerOption<LanguageVersion>
    {
        static LanguageVersionOption()
        {
            HelpText = "Set the c# version used to compile.";
            DefaultValue = LanguageVersion.Latest;
        }

        public override StrykerOption Type => StrykerOption.LanguageVersion;

        public LanguageVersionOption(string languageVersion)
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
