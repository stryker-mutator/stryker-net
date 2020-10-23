using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.Options.Options
{
    class LanguageVersionOption : BaseStrykerOption<LanguageVersion>
    {
        public LanguageVersionOption(string languageVersion)
        {
            if (Enum.TryParse(languageVersion, true, out LanguageVersion result) && result != LanguageVersion.CSharp1)
            {
                Value = result;
            }
            else
            {
                throw new StrykerInputException(ErrorMessage,
                    $"The given c# language version ({languageVersion}) is invalid. Valid options are: [{string.Join(", ", ((IEnumerable<LanguageVersion>)Enum.GetValues(typeof(LanguageVersion))).Where(l => l != LanguageVersion.CSharp1))}]");
            }
        }

        public override StrykerOption Type => StrykerOption.LanguageVersion;
        public override string HelpText => "Set the c# version used to compile.";
        public override LanguageVersion DefaultValue => LanguageVersion.Latest;
    }
}
